namespace BinaryCalendar.Web.Service.Configuration
{
    #region Using Directives

    using BinaryCalendar.Utilities.Email;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Server.Toolkit.Utilities.SettingsFile;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class BcServiceSettings : Settings
    {
        #region Constructors

        public BcServiceSettings()
            : base()
        {
        }

        public BcServiceSettings(string filePath)
            : base(filePath)
        {
        }

        public BcServiceSettings(string name, string filePath)
            : base(name, filePath)
        {
        }

        #endregion //Constructors

        #region Properties

        #region Application Settings

        /// <summary>
        /// The name of the application which will be displayed on the root URI.
        /// </summary>
        [SettingInfo("Application", AutoFormatDisplayName = true, Description = "The name of the application which will be displayed on the root URI.", CategorySequenceId = 0)]
        public string ApplicationName { get; set; }

        /// <summary>
        /// The file path of the logo to use in the generated PDF documents.
        /// </summary>
        [SettingInfo("Application", AutoFormatDisplayName = true, Description = "The file path of the logo to use in the generated PDF documents.", CategorySequenceId = 1)]
        public string LogoFilePath { get; set; }

        /// <summary>
        /// A list of import directories for each supported country.
        /// </summary>
        [SettingInfo("Application", AutoFormatDisplayName = true, Description = "A list of import directories for each supported country.", CategorySequenceId = 2)]
        public List<CountryCalendarImportInfo> CountryCalendarImportDirectories { get; set; }

        /// <summary>
        /// Whether or not to delete the .ics iCalendar files after importing them.
        /// </summary>
        [SettingInfo("Application", AutoFormatDisplayName = true, Description = "Whether or not to delete the .ics iCalendar files after importing them.", CategorySequenceId = 3)]
        public bool DeleteICalendarFilesAfterImport { get; set; }

        #endregion //Application Settings

        #region Logging

        /// <summary>
        /// Whether or not to log to a text log file in the executing directory.
        /// </summary>
        [SettingInfo("Logging", DisplayName = "To File", Description = "Whether or not to log to a text log file in the executing directory.", CategorySequenceId = 0)]
        public bool LogToFile { get; set; }

        /// <summary>
        /// Whether or not to log to the Windows Event Log.
        /// </summary>
        [SettingInfo("Logging", DisplayName = "To Windows Event Log", Description = "Whether or not to log to the Windows Event Log.", CategorySequenceId = 1)]
        public bool LogToWindowsEventLog { get; set; }

        /// <summary>
        /// Whether or not to log to the console.
        /// </summary>
        [SettingInfo("Logging", DisplayName = "To Console", Description = "Whether or not to log to the console.", CategorySequenceId = 2)]
        public bool LogToConsole { get; set; }

        /// <summary>
        /// The name of the text log file to log to. The log file is written in the executing directory.
        /// </summary>
        [SettingInfo("Logging", AutoFormatDisplayName = true, Description = "The name of the text log file to log to. The log file is written in the executing directory.", CategorySequenceId = 3)]
        public string LogFileName { get; set; }

        /// <summary>
        /// The name of the event source to use when logging to the Windows Event Log.
        /// </summary>
        [SettingInfo("Logging", AutoFormatDisplayName = true, Description = "The name of the event source to use when logging to the Windows Event Log.", CategorySequenceId = 4)]
        public string EventSourceName { get; set; }

        [SettingInfo("Logging", AutoFormatDisplayName = true, Description = "The name of the Windows Event Log to log to.", CategorySequenceId = 5)]
        public string EventLogName { get; set; }

        /// <summary>
        /// The extent of messages being logged: None = logging is disabled, Minimum = logs server start/stop and exceptions, Normal = logs additional information messages, Maximum = logs all requests and responses to the server.
        /// </summary>
        [SettingInfo("Logging", AutoFormatDisplayName = true, Description = "The extent of messages being logged: None = logging is disabled, Minimum = logs server start/stop and exceptions, Normal = logs additional information messages, Maximum = logs all requests and responses to the server.", CategorySequenceId = 6)]
        public LoggingLevel LoggingLevel { get; set; }

        #endregion //Logging

        #region Database

        /// <summary>
        /// The connection string to the SQL server database.
        /// </summary>
        [SettingInfo("Database", AutoFormatDisplayName = true, Description = "The connection string to the SQL server database.", CategorySequenceId = 0)]
        public string DatabaseConnectionString { get; set; }

        /// <summary>
        /// The timeout in milliseconds of command to the database.
        /// </summary>
        [SettingInfo("Database", AutoFormatDisplayName = true, Description = "The timeout in milliseconds of command to the database.", CategorySequenceId = 1)]
        public int DatabaseCommandTimeout { get; set; }

        /// <summary>
        /// Assembly containing the LINQ to SQL ORM classes.
        /// </summary>
        [SettingInfo("Database", AutoFormatDisplayName = true, Description = "Assembly containing the LINQ to SQL ORM classes.", CategorySequenceId = 2)]
        public string LinqToSQLClassesAssemblyFileName { get; set; }

        /// <summary>
        /// Namespace of the LINQ to SQL ORM classes inside the assembly.
        /// </summary>
        [SettingInfo("Database", AutoFormatDisplayName = true, Description = "Namespace of the LINQ to SQL ORM classes inside the assembly.", CategorySequenceId = 3)]
        public string LinqToSQLClassesNamespace { get; set; }

        #endregion //Database

        #region Service

        /// <summary>
        /// The suffix to append to the URI on which this Web Service will be accessed i.e. http://localhost:{port_number}/{suffix} e.g. http://localhost:8889/MyService.
        /// </summary>
        [SettingInfo("Service", AutoFormatDisplayName = true, Description = "The suffix to append to the URI on which this Web Service will be accessed i.e. http://localhost:{port_number}/{suffix} e.g. http://localhost:8889/MyService.", CategorySequenceId = 0)]
        public string HostAddressSuffix { get; set; }

        /// <summary>
        /// The port number on which this Web Service should listen for requests from clients i.e. http://localhost:{port_number}/{suffix} e.g. http://localhost:2983/MyService.
        /// </summary>
        [SettingInfo("Service", AutoFormatDisplayName = true, Description = "The port number on which this Web Service should listen for requests from clients i.e. http://localhost:{port_number}/{suffix} e.g. http://localhost:2983/MyService.", CategorySequenceId = 1)]
        public int PortNumber { get; set; }

        /// <summary>
        /// Whether or not the service should authenticate clients attempting to consume the service.
        /// </summary>
        [SettingInfo("Service", AutoFormatDisplayName = true, Description = "Whether or not the service should authenticate clients attempting to consume the service.", CategorySequenceId = 2)]
        public bool UseAuthentication { get; set; }

        /// <summary>
        /// Whether or not to include the exception details including the stack trace in the web response when an unhandled exception occurs.
        /// </summary>
        [SettingInfo("Service", AutoFormatDisplayName = true, Description = "Whether or not to include the exception details including the stack trace in the web response when an unhandled exception occurs.", CategorySequenceId = 3)]
        public bool IncludeExceptionDetailInResponse { get; set; }

        /// <summary>
        /// Encoding to used on the text response from the service.
        /// </summary>
        [SettingInfo("Service", AutoFormatDisplayName = true, Description = "Encoding to used on the text response from the service.", CategorySequenceId = 4)]
        public TextEncodingType TextResponseEncoding { get; set; }

        /// <summary>
        /// The maximum amount of memory allocated, in bytes, for the buffer manager that manages the buffers required by endpoints that use this binding.
        /// </summary>
        [SettingInfo("Service", AutoFormatDisplayName = true, Description = "The maximum amount of memory allocated, in bytes, for the buffer manager that manages the buffers required by endpoints that use this binding.", CategorySequenceId = 5)]
        public long MaxBufferPoolSize { get; set; }

        /// <summary>
        /// The maximum amount of memory allocated, in bytes, for use by the manager of the message buffers that receive messages from the channel.
        /// </summary>
        [SettingInfo("Service", AutoFormatDisplayName = true, Description = "The maximum amount of memory allocated, in bytes, for use by the manager of the message buffers that receive messages from the channel.", CategorySequenceId = 6)]
        public long MaxBufferSize { get; set; }

        /// <summary>
        /// The maximum size, in bytes, for a message that can be processed by the binding.
        /// </summary>
        [SettingInfo("Service", AutoFormatDisplayName = true, Description = "The maximum size, in bytes, for a message that can be processed by the binding.", CategorySequenceId = 7)]
        public long MaxReceivedMessageSize { get; set; }

        /// <summary>
        /// Whether to trace and log HTTP messages.
        /// </summary>
        [SettingInfo("Service", AutoFormatDisplayName = true, Description = "Whether to trace and log HTTP messages.", CategorySequenceId = 8)]
        public bool TraceHttpMessages { get; set; }

        /// <summary>
        /// Whether to trace and log HTTP headers.
        /// </summary>
        [SettingInfo("Service", AutoFormatDisplayName = true, Description = "Whether to trace and log HTTP headers.", CategorySequenceId = 9)]
        public bool TraceHttpMessageHeaders { get; set; }

        /// <summary>
        /// Whether to audit (log) calls on this web service.
        /// </summary>
        [SettingInfo("Service", AutoFormatDisplayName = true, Description = "Whether to audit (log) calls on this web service.", CategorySequenceId = 10)]
        public bool AuditServiceCalls { get; set; }

        #endregion //Service

        #region Emails

        /// <summary>
        /// Whether to enable sending of emails.
        /// </summary>
        [SettingInfo("Emails", AutoFormatDisplayName = true, Description = "Whether to enable sending of emails.", CategorySequenceId = 0)]
        public bool EnableEmailNotifications { get; set; }

        /// <summary>
        /// The type of SMTP server to use to send emails i.e. Exchange or GMail.
        /// </summary>
        [SettingInfo("Emails", AutoFormatDisplayName = true, Description = "The type of SMTP server to use to send emails i.e. Exchange or GMail.", CategorySequenceId = 1)]
        public EmailType EmailType { get; set; }

        /// <summary>
        /// The hostname or IP address of the Exchange SMTP server.
        /// </summary>
        [SettingInfo("Emails", AutoFormatDisplayName = true, Description = "The hostname or IP address of the Exchange SMTP server.", CategorySequenceId = 2)]
        public string LocalSmtpServer { get; set; }

        /// <summary>
        /// The user name to use to login to the Exchange SMTP server.
        /// </summary>
        [SettingInfo("Emails", AutoFormatDisplayName = true, Description = "The user name to use to login to the Exchange SMTP server.", CategorySequenceId = 3)]
        public string LocalSmtpUserName { get; set; }

        /// <summary>
        /// The password to use to login to the Exchange SMTP server.
        /// </summary>
        [SettingInfo("Emails", AutoFormatDisplayName = true, Description = "The password to use to login to the Exchange SMTP server.", CategorySequenceId = 4)]
        public string LocalSmtpPassword { get; set; }

        /// <summary>
        /// The port to use to connect to the Exchange SMTP server.
        /// </summary>
        [SettingInfo("Emails", AutoFormatDisplayName = true, Description = "The port to use to connect to the Exchange SMTP server.", CategorySequenceId = 5)]
        public int LocalSmtpPort { get; set; }

        /// <summary>
        /// Whether to use SSL when connecting to the Exchange SMTP server.
        /// </summary>
        [SettingInfo("Emails", AutoFormatDisplayName = true, Description = "Whether to use SSL when connecting to the Exchange SMTP server.", CategorySequenceId = 6)]
        public bool LocalSmtpEnableSsl { get; set; }

        /// <summary>
        /// The hostname or IP address of the GMail SMTP server.
        /// </summary>
        [SettingInfo("Emails", AutoFormatDisplayName = true, Description = "The hostname or IP address of the GMail SMTP server.", CategorySequenceId = 7)]
        public string GMailSMTPServer { get; set; }

        /// <summary>
        /// The username to use to login to the GMail SMTP server.
        /// </summary>
        [SettingInfo("Emails", AutoFormatDisplayName = true, Description = "The user name to use to login to the GMail SMTP server.", CategorySequenceId = 8)]
        public string GMailSmtpUserName { get; set; }

        /// <summary>
        /// The password to use to login to the GMail SMTP server.
        /// </summary>
        [SettingInfo("Emails", AutoFormatDisplayName = true, Description = "The password to use to login to the GMail SMTP server.", CategorySequenceId = 9)]
        public string GMailSmtpPassword { get; set; }

        /// <summary>
        /// The port to use to connect to the GMail SMTP server.
        /// </summary>
        [SettingInfo("Emails", AutoFormatDisplayName = true, Description = "The port to use to connect to the GMail SMTP server.", CategorySequenceId = 10)]
        public int GMailSmtpPort { get; set; }

        /// <summary>
        /// The email address to display of the email sender.
        /// </summary>
        [SettingInfo("Emails", AutoFormatDisplayName = true, Description = "The email address to display of the email sender.", CategorySequenceId = 11)]
        public string SenderEmailAddress { get; set; }

        /// <summary>
        /// The display name of the email sender.
        /// </summary>
        [SettingInfo("Emails", AutoFormatDisplayName = true, Description = "The display name of the email sender.", CategorySequenceId = 12)]
        public string SenderDisplayName { get; set; }

        /// <summary>
        /// Whether to save emails in the database which will be sent later.
        /// </summary>
        [SettingInfo("Emails", AutoFormatDisplayName = true, Description = "Whether to save emails in the database which will be sent later.", CategorySequenceId = 13)]
        public bool SendEmailsLater { get; set; }

        /// <summary>
        /// Whether to capture and send error email notifications.
        /// </summary>
        [SettingInfo("Emails", AutoFormatDisplayName = true, Description = "Whether to capture and send error email notifications.", CategorySequenceId = 14)]
        public bool EnableErrorEmailNotifications { get; set; }

        /// <summary>
        /// The email address to which error emails should be sent to.
        /// </summary>
        [SettingInfo("Emails", AutoFormatDisplayName = true, Description = "The email address to which error emails should be sent to.", CategorySequenceId = 15)]
        public string ErrorEmailNotificationAddress { get; set; }

        /// <summary>
        /// The directory where email attachments will be saved when they are scheduled to be sent later.
        /// </summary>
        [SettingInfo("Emails", AutoFormatDisplayName = true, Description = "The directory where email attachments will be saved when they are scheduled to be sent later.", CategorySequenceId = 16)]
        public string EmailAttachmentsDirectory { get; set; }

        #endregion //Emails

        #endregion //Properties
    }
}