namespace BinaryCalendar.Web.Service.Configuration
{
    #region Using Directives

    using BinaryCalendar.ORM;
    using BinaryCalendar.Utilities.Email;
    using BinaryCalendar.Web.Service.Rest;
    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Server.Toolkit.Web.Service.Inspector;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class BcServiceApp
    {
        #region Singleton Setup

        private static BcServiceApp _instance;

        public static BcServiceApp Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BcServiceApp();
                }
                return _instance;
            }
        }

        #endregion //Singleton Setup

        #region Constructors

        private BcServiceApp()
        {
            _lock = new object();
        }

        #endregion //Constructors

        #region Fields

        private BcServiceSettings _settings;
        private Dictionary<string, object> _administratorTables;
        private object _lock;

        #endregion //Fields

        #region Properties

        public BcServiceSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = GOC.Instance.GetSettings<BcServiceSettings>(true, true);
                }
                return _settings;
            }
        }

        public Dictionary<string, object> AdministratorTables
        {
            get
            {
                if (_administratorTables == null)
                {
                    _administratorTables = new Dictionary<string, object>();
                    _administratorTables.Add(typeof(User).Name.ToLower(), null);
                    _administratorTables.Add(typeof(ServerAction).Name.ToLower(), null);
                    _administratorTables.Add(typeof(PublicHoliday).Name.ToLower(), null);
                    _administratorTables.Add(typeof(ServerError).Name.ToLower(), null);
                    _administratorTables.Add(typeof(EmailNotification).Name.ToLower(), null);
                    _administratorTables.Add(typeof(EmailNotificationRecipient).Name.ToLower(), null);
                    _administratorTables.Add(typeof(EmailNotificationAttachment).Name.ToLower(), null);
                }
                return _administratorTables;
            }
        }

        #endregion //Properties

        #region Methods

        internal void Initialize(bool startWebService)
        {
            GOC.Instance.ShowMessageBoxOnException = false;
            BcServiceSettings settings = Settings;

            GOC.Instance.ApplicationName = settings.ApplicationName;

            GOC.Instance.Logger = new Logger(//Creates a global Logger to be used throughout the application to be stored in the Global Object Cache which is a singleton.
                settings.LogToFile,
                settings.LogToWindowsEventLog,
                settings.LogToConsole,
                settings.LoggingLevel,
                settings.LogFileName,
                settings.EventSourceName,
                settings.EventLogName);
            GOC.Instance.JsonSerializer.IncludeOrmTypeNamesInJsonResponse = false;
            GOC.Instance.SetEncoding(settings.TextResponseEncoding);

            LinqFunnelSettings linqFunnelSettings = new LinqFunnelSettings(settings.DatabaseConnectionString, settings.DatabaseCommandTimeout);
            GOC.Instance.AddByTypeName(linqFunnelSettings); //Adds an object to Global Object Cache with the key being the name of the type.
            string linqToSqlAssemblyFilePath = Path.Combine(Information.GetExecutingDirectory(), settings.LinqToSQLClassesAssemblyFileName);
            GOC.Instance.Logger.LogMessage(new LogMessage(string.Format("Attemping to load {0}", settings.LinqToSQLClassesAssemblyFileName), LogMessageType.Information, LoggingLevel.Maximum));
            GOC.Instance.UserLinqToSqlType = typeof(User);
            GOC.Instance.ServerActionLinqToSqlType = typeof(ServerAction);
            GOC.Instance.ServerErrorLinqToSqlType = typeof(ServerError);

            //Grab the LinqToSql context from the specified assembly and load it in the GOC to be used from anywhere in the application.
            //The point of doing this is that you can rebuild the context if the schema changes and reload without having to reinstall the web service. You just stop the service and overwrite the EOH.RainMaker.ORM.dll with the new one.
            //It also allows the Figlut Service Toolkit to be business data agnostic.
            GOC.Instance.LinqToClassesAssembly = Assembly.LoadFrom(linqToSqlAssemblyFilePath);
            GOC.Instance.LinqToSQLClassesNamespace = settings.LinqToSQLClassesNamespace;
            GOC.Instance.SetLinqToSqlDataContextType<BinaryCalendarDataContext>();

            EmailNotifier.Instance.Initialize(new EmailSender(
                settings.EnableEmailNotifications,
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

            if (startWebService)
            {
                InitializeServiceHost(settings);
            }
        }

        private void InitializeServiceHost(BcServiceSettings settings)
        {
            WebHttpBinding binding = new WebHttpBinding()
            {
                MaxBufferPoolSize = settings.MaxBufferPoolSize,
                MaxBufferSize = Convert.ToInt32(settings.MaxBufferSize),
                MaxReceivedMessageSize = settings.MaxReceivedMessageSize
            };
            if (settings.UseAuthentication)
            {
                binding.Security.Mode = WebHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            }
            ServiceHost serviceHost = new ServiceHost(typeof(BinaryCalendarRestService));
            string address = string.Format("http://127.0.0.1:{0}/{1}", settings.PortNumber, settings.HostAddressSuffix);
            ServiceDebugBehavior debugBehaviour = serviceHost.Description.Behaviors.Find<ServiceDebugBehavior>();
            if (debugBehaviour == null) //This should never be, but just in case.`
            {
                debugBehaviour = new ServiceDebugBehavior();
                serviceHost.Description.Behaviors.Add(debugBehaviour);
            }
            debugBehaviour.IncludeExceptionDetailInFaults = settings.IncludeExceptionDetailInResponse;
            ServiceEndpoint httpEndpoint = serviceHost.AddServiceEndpoint(typeof(IBinaryCalendarRestService), binding, address);
            httpEndpoint.Behaviors.Add(new WebHttpBehavior());
            httpEndpoint.EndpointBehaviors.Add(new ServiceMessageInspectorBehavior(settings.TraceHttpMessages, settings.TraceHttpMessageHeaders));
            if (settings.UseAuthentication)
            {
                serviceHost.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = System.ServiceModel.Security.UserNamePasswordValidationMode.Custom;
                serviceHost.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new UserValidator();
            }
            if (GOC.Instance.GetByTypeName<ServiceHost>() != null)
            {
                GOC.Instance.DeleteByTypeName<ServiceHost>();
            }
            GOC.Instance.AddByTypeName(serviceHost); //The service's stop method will access it from the GOC to close the service host.
            serviceHost.Open();

            GOC.Instance.Logger.LogMessage(new LogMessage(
                string.Format("Binary Calendar Web Service started: {0}", address),
                LogMessageType.Information,
                LoggingLevel.Minimum));
        }

        public bool IsAdministratorTable(string tableName)
        {
            bool result = AdministratorTables.ContainsKey(tableName.ToLower());
            return result;
        }

        #endregion //Methods
    }
}