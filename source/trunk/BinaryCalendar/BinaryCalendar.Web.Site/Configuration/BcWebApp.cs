namespace BinaryCalendar.Web.Site.Configuration
{
    #region Using Directives

    using BinaryCalendar.ORM;
    using BinaryCalendar.Utilities.Email;
    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web;

    #endregion //Using Directives

    public class BcWebApp
    {
        #region Singleton Setup

        private static volatile BcWebApp _instance;
        private static object _syncRoot = new object();

        public static BcWebApp Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new BcWebApp();
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion //Singleton Setup

        #region Constructors

        private BcWebApp()
        {
        }

        #endregion //Constructors

        #region Fields

        private BcWebSettings _settings;

        #endregion //Fields

        #region Properties

        public BcWebSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = GOC.Instance.GetSettings<BcWebSettings>(true, true);
                }
                return _settings;
            }
        }

        #endregion //Properties

        #region Methods

        public void Initialize()
        {
            BcWebSettings settings = Settings;
            GOC.Instance.ApplicationName = settings.ApplicationName;

            GOC.Instance.Logger = new Logger(//Creates a global Logger to be used throughout the application to be stored in the Global Object Cache which is a singleton.
                settings.LogToFile,
                settings.LogToWindowsEventLog,
                settings.LogToConsole,
                settings.LoggingLevel,
                settings.LogFileName,
                settings.EventSourceName,
                settings.EventLogName);
            GOC.Instance.JsonSerializer.IncludeOrmTypeNamesInJsonResponse = true;

            LinqFunnelSettings linqFunnelSettings = new LinqFunnelSettings(settings.DatabaseConnectionString, settings.DatabaseCommandTimeout);
            GOC.Instance.AddByTypeName(linqFunnelSettings); //Adds an object to Global Object Cache with the key being the name of the type.
            string linqToSqlAssemblyFilePath = Path.Combine(Information.GetExecutingDirectory(), settings.LinqToSQLClassesAssemblyFileName);
            GOC.Instance.Logger.LogMessage(new LogMessage(string.Format("Attemping to load {0}", settings.LinqToSQLClassesAssemblyFileName), LogMessageType.SuccessAudit, LoggingLevel.Maximum));

            //Grab the LinqToSql context from the specified assembly and load it in the GOC to be used from anywhere in the application.
            //The point of doing this is that you can rebuild the context if the schema changes and reload without having to reinstall the web service. You just stop the service and overwrite the EOH.RainMaker.ORM.dll with the new one.
            //It also allows the Figlut Service Toolkit to be business data agnostic.
            GOC.Instance.LinqToClassesAssembly = Assembly.LoadFrom(linqToSqlAssemblyFilePath);
            GOC.Instance.LinqToSQLClassesNamespace = settings.LinqToSQLClassesNamespace;
            GOC.Instance.SetLinqToSqlDataContextType<BinaryCalendarDataContext>(); //This is the wrapper context that can also call the Rain Maker specific sprocs.

            EmailNotifier.Instance.Initialize(new EmailSender(
                settings.EmailNotificationsEnabled,
                settings.EmailType,
                settings.LocalSmtpServer,
                settings.LocalSmtpUserName,
                settings.LocalSmtpPassword,
                settings.LocalSmtpPort,
                settings.LocalSmtpEnableSsl,
                settings.GMailSMTPServer,
                settings.GMailSmtpUserName,
                settings.GMailSmtpPassword,
                settings.GMailSmtpPort,
                settings.SenderEmailAddress,
                settings.SenderDisplayName));

            GOC.Instance.Logger.LogMessage(new LogMessage("Application Initialized.", LogMessageType.SuccessAudit, LoggingLevel.Normal));
        }

        #endregion //Methods
    }
}