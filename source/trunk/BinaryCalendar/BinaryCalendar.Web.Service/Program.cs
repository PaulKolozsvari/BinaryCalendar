namespace BinaryCalendar.Web.Service
{
    #region Using Directives

    using BinaryCalendar.Utilities.Data;
    using BinaryCalendar.Utilities.Email;
    using BinaryCalendar.Web.Service.Configuration;
    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Data.iCalendar;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Ical.Net;
    using Ical.Net.DataTypes;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.ServiceProcess;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    static class Program
    {
        #region Constants

        private const string HELP_ARGUMENT = "/help";
        private const string HELP_QUESTION_MARK_ARGUMENT = "/?";
        private const string RESET_SETTINGS_ARGUMENT = "/reset_settings";
        private const string START_TEST_MODE_ARGUMENT = "/start";
        private const string IMPORT_ICALENDAR_FILES = "/import_icalendar";

        #endregion //Constants

        #region Fields

        private static bool _startInTestMode;

        #endregion //Fields

        #region Methods

        private static bool ParseArguments(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                string a = args[i];
                string aLower = a.ToLower();
                switch (aLower)
                {
                    case HELP_ARGUMENT:
                        DisplayHelp();
                        return false;
                    case HELP_QUESTION_MARK_ARGUMENT:
                        DisplayHelp();
                        return false;
                    case RESET_SETTINGS_ARGUMENT:
                        ResetSettings();
                        return false;
                    case START_TEST_MODE_ARGUMENT:
                        _startInTestMode = true;
                        return true;
                    case IMPORT_ICALENDAR_FILES:
                        ImportICalendarFiles();
                        return false;
                    default:
                        throw new ArgumentException(string.Format("Invalid argument '{0}'.", a));
                }
            }
            return true;
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("*** Binary Calendar Web Service Usage ***");
            Console.WriteLine();
            Console.WriteLine("{0} or {1} : Display usage (service is not started).", HELP_ARGUMENT, HELP_QUESTION_MARK_ARGUMENT);
            Console.WriteLine("{0} : Resets the service's settings file with the default settings (server is not started).", RESET_SETTINGS_ARGUMENT);
            Console.WriteLine("{0} : Starts the service as a console application instead of a windows service.", START_TEST_MODE_ARGUMENT);
            Console.WriteLine("{0} : Imports .ics iCalendar files into the database based on import directoties set in settings.", IMPORT_ICALENDAR_FILES);
            Console.WriteLine();
            Console.WriteLine("N.B. Executing without any parameters runs the server as a windows service.");
        }

        private static void ResetSettings()
        {
            BcServiceSettings s = new BcServiceSettings()
            {
                ShowMessageBoxOnException = true,
                LogToFile = true,
                LogToWindowsEventLog = true,
                LogToConsole = true,
                LogFileName = "BCalendar.Log.txt",
                EventSourceName = "BCalendar.Source",
                EventLogName = "BCalendar.Log",
                LoggingLevel = LoggingLevel.Normal,
                DatabaseConnectionString = "<Enter DB connection string here>",
                DatabaseCommandTimeout = 30000,
                LinqToSQLClassesAssemblyFileName = "BinaryCalendar.ORM.dll",
                LinqToSQLClassesNamespace = "BinaryCalendar.ORM",
                HostAddressSuffix = "PublicHolidaysAPI",
                PortNumber = 2985,
                UseAuthentication = true,
                IncludeExceptionDetailInResponse = false,
                MaxBufferPoolSize = 2147483647,
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647,
                TraceHttpMessages = false,
                TraceHttpMessageHeaders = false,
                AuditServiceCalls = true,
                EnableEmailNotifications = true,
                EmailType = EmailType.GMail,
                LocalSmtpServer = "127.0.0.1",
                LocalSmtpUserName = @"domain\username",
                LocalSmtpPassword = "mypassword",
                LocalSmtpPort = 21,
                LocalSmtpEnableSsl = false,
                GMailSMTPServer = "smtp.gmail.com",
                GMailSmtpUserName = "username@gmail.com",
                GMailSmtpPassword = "mypassword",
                GMailSmtpPort = 587,
                SenderEmailAddress = "username@gmail.com",
                SenderDisplayName = "Repeat Notifications",
                SendEmailsLater = true,
                EnableErrorEmailNotifications = true,
                ErrorEmailNotificationAddress = "paul.kolozsvari@binarychef.com",
                EmailAttachmentsDirectory = @".\EmailAttachments"
            };
            s.CountryCalendarImportDirectories = new List<CountryCalendarImportInfo>();
            s.CountryCalendarImportDirectories.Add(new CountryCalendarImportInfo()
            {
                CountryCode = "ZA",
                CountryName = "South Africa",
                ICalendarImportDirectory = @"C:\Calendar\South Africa"
            });
            Console.Write("Reset settings file {0}, are you sure (Y/N)?", s.FilePath);
            string response = Console.ReadLine();
            if (response.Trim().ToLower() != "y")
            {
                return;
            }
            s.SaveToFile();
            Console.WriteLine("Settings file reset successfully.");
            Console.Read();
        }

        private static void ImportICalendarFiles()
        {
            BcServiceApp.Instance.Initialize(false);
            BcEntityContext context = BcEntityContext.Create();

            foreach (CountryCalendarImportInfo c in BcServiceApp.Instance.Settings.CountryCalendarImportDirectories)
            {
                if (string.IsNullOrEmpty(c.CountryCode) || string.IsNullOrEmpty(c.CountryName))
                {
                    throw new NullReferenceException(string.Format("{0} or {1} not specified on {2} in {3}.",
                        EntityReader<CountryCalendarImportInfo>.GetPropertyName(p => p.CountryCode, false),
                        EntityReader<CountryCalendarImportInfo>.GetPropertyName(p => p.CountryName, false),
                        typeof(CountryCalendarImportInfo).Name,
                        BcServiceApp.Instance.Settings.FilePath));
                }
                if (!Directory.Exists(c.ICalendarImportDirectory))
                {
                    throw new DirectoryNotFoundException(string.Format("Could not find directory {0} for country {1} ({2}).",
                        c.ICalendarImportDirectory,
                        c.CountryCode,
                        c.CountryName));
                }
                string[] filePaths = Directory.GetFiles(c.ICalendarImportDirectory, "*.ics");
                foreach (string f in filePaths)
                {
                    GOC.Instance.Logger.LogMessage(new LogMessage(string.Format("Parsing iCalendar file: {0} ...", f), LogMessageType.Information, LoggingLevel.Normal));
                    ICalCalendar calendar = ICalPublicHolidayParser.ParseICalendarFile(f, c.CountryCode, c.CountryName);

                    GOC.Instance.Logger.LogMessage(new LogMessage(string.Format("Saving iCalendar to DB: {0} ...", f), LogMessageType.Information, LoggingLevel.Normal));
                    context.SaveICalCalendar(calendar);

                    if (BcServiceApp.Instance.Settings.DeleteICalendarFilesAfterImport)
                    {
                        GOC.Instance.Logger.LogMessage(new LogMessage(string.Format("Deleting iCalendar file: {0} ...", f), LogMessageType.Information, LoggingLevel.Normal));
                        File.Delete(f);
                    }

                    GOC.Instance.Logger.LogMessage(new LogMessage(string.Format("Completed iCalendar file import: {0} ...", f), LogMessageType.SuccessAudit, LoggingLevel.Minimum));
                }
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            try
            {
                if (!ParseArguments(args))
                {
                    return;
                }
                if (_startInTestMode)
                {
                    Console.WriteLine("Starting Binary Calendar Web Service ... ");
                    BinaryCalendarService.Start(true);

                    Console.WriteLine("Press any key to stop the service ...");
                    Console.Read();
                    BinaryCalendarService.Stop();
                    return;
                }
                ServiceBase.Run(new ServiceBase[] { new BinaryCalendarService() });
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                throw ex;
            }
        }

        #endregion //Methods
    }
}