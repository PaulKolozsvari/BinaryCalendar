namespace BinaryCalendar.Utilities.Email
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;

    #endregion //Using Directives

    public class EmailNotifier
    {
        #region Singleton Setup

        private static EmailNotifier _instance;

        public static EmailNotifier Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EmailNotifier();
                }
                return _instance;
            }
        }

        #endregion //Singleton Setup

        #region Constructors

        private EmailNotifier()
        {
        }

        #endregion //Constructors

        #region Fields

        private EmailSender _emailSender;

        #endregion //Fields

        #region Properties

        public EmailSender EmailSender
        {
            get
            {
                if (_emailSender == null)
                {
                    throw new NullReferenceException(string.Format("{0} not initialized.", typeof(EmailSender).Name));
                }
                return _emailSender;
            }
        }

        #endregion //Properties

        #region Methods

        public void Initialize(EmailSender emailSender)
        {
            if (emailSender == null)
            {
                throw new NullReferenceException(string.Format("{0} may not be null when initializing the {1}.", typeof(EmailSender).Name, typeof(EmailNotifier)));
            }
            _emailSender = emailSender;
        }

        public void SendErrorNotification(Exception ex, string process, bool sendLater, string errorEmailNotificationAddress)
        {
            StringBuilder errorMessage = new StringBuilder();
            if (!string.IsNullOrEmpty(process))
            {
                errorMessage.AppendLine(string.Format("Process: {0}", process));
            }
            errorMessage.AppendLine(string.Format("Date: {0}", DateTime.Now));
            errorMessage.AppendLine(ex.Message);
            if (ex.InnerException != null)
            {
                errorMessage.AppendLine(string.Format("Inner Exception: {0}", ex.InnerException.Message));
            }
            errorMessage.AppendLine("Stack Trace:");
            errorMessage.AppendLine(ex.StackTrace);
            EmailNotifier.Instance.EmailSender.SendEmailNotification(
                "Binary Calendar Technical Error Alert",
                errorMessage.ToString(),
                null,
                new List<string>() { errorEmailNotificationAddress },
                false,
                sendLater);
        }

        #endregion //Methods
    }
}