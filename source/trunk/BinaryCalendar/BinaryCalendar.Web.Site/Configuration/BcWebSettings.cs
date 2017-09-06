namespace BinaryCalendar.Web.Site.Configuration
{
    #region Using Directives

    using BinaryCalendar.Utilities.Email;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Server.Toolkit.Utilities.SettingsFile;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class BcWebSettings : Settings
    {
        #region Constructors

        public BcWebSettings()
            : base()
        {
        }

        public BcWebSettings(string filePath)
            : base(filePath)
        {
        }

        public BcWebSettings(string name, string filePath)
            : base(name, filePath)
        {
        }

        #endregion //Constructors

        #region Properties

        #region Application

        public string ApplicationName { get; set; }

        public string RestWebServiceBaseUri { get; set; }

        #endregion //Application

        #region Email

        public bool EmailNotificationsEnabled { get; set; }

        public bool EmailDatabaseLoggingEnabled { get; set; }

        public bool EmailDatabaseLogMessageContents { get; set; }

        public EmailType EmailType { get; set; }

        public string LocalSmtpServer { get; set; }

        public string LocalSmtpUserName { get; set; }

        public string LocalSmtpPassword { get; set; }

        public int LocalSmtpPort { get; set; }

        public bool LocalSmtpEnableSsl { get; set; }

        public string GMailSMTPServer { get; set; }

        public string GMailSmtpUserName { get; set; }

        public string GMailSmtpPassword { get; set; }

        public int GMailSmtpPort { get; set; }

        public string SenderEmailAddress { get; set; }

        public string SenderDisplayName { get; set; }

        #endregion //Email

        #region Logging

        public bool LogToFile { get; set; }

        public bool LogToWindowsEventLog { get; set; }

        public bool LogToConsole { get; set; }

        public string LogFileName { get; set; }

        public string EventSourceName { get; set; }

        public string EventLogName { get; set; }

        public LoggingLevel LoggingLevel { get; set; }

        #endregion //Logging

        #region Database

        public string DatabaseConnectionString { get; set; }

        public int DatabaseCommandTimeout { get; set; }

        public string LinqToSQLClassesAssemblyFileName { get; set; }

        public string LinqToSQLClassesNamespace { get; set; }

        #endregion //Database

        #endregion //Properties
    }
}