namespace BinaryCalendar.Web.Service
{
    #region Using Directives

    using BinaryCalendar.Web.Service.Configuration;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceProcess;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public partial class BinaryCalendarService : ServiceBase
    {
        #region Constructors

        public BinaryCalendarService()
        {
            InitializeComponent();
        }

        #endregion //Constructors

        #region Methods

        protected override void OnStart(string[] args)
        {
            try
            {
                Start(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                throw ex; 
            }
        }

        protected override void OnStop()
        {
            try
            {
                Stop();
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                throw ex;
            }
        }

        internal static void Start(bool startWebService)
        {
            BcServiceApp.Instance.Initialize(startWebService);
        }

        internal static new void Stop()
        {
            GOC.Instance.GetByTypeName<ServiceHost>().Close();
            GOC.Instance.Logger.LogMessage(new LogMessage(
                "Binary Calendar Service stopped.",
                LogMessageType.Information,
                LoggingLevel.Minimum));
        }

        #endregion //Methods
    }
}