namespace BinaryCalendar.Utilities.Email
{
    #region Using Directives

    using BinaryCalendar.ORM;
    using BinaryCalendar.Utilities.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class EmailSender
    {
        #region Constructors

        public EmailSender(
            bool enabled,
            EmailType emailType,
            string localSmtpServer,
            string localSmtpUserName,
            string localSmtpPassword,
            int localSmtpPort,
            bool localSmtpEnableSsl,
            string gmailSmtpServer,
            string gmailSmtpUserName,
            string gmailSmtpPassword,
            int gmailSmtpPort,
            string senderEmailAddress,
            string senderDisplayName)
        {
            _enabled = enabled;
            _emailType = emailType;
            _localSmtpServer = localSmtpServer;
            _localSmtpUserName = localSmtpUserName;
            _localSmtpPassword = localSmtpPassword;
            _localSmtpPort = localSmtpPort;
            _localSmtpEnableSsl = localSmtpEnableSsl;
            _gmailSmtpServer = gmailSmtpServer;
            _gmailSmtpUserName = gmailSmtpUserName;
            _gmailSmtpPassword = gmailSmtpPassword;
            _gmailSmtpPort = gmailSmtpPort;
            _senderEmailAddress = senderEmailAddress;
            _senderDisplayName = senderDisplayName;
            InitializeSmtpClient();
        }

        #endregion //Constructors

        #region Fields

        private bool _enabled;
        private EmailType _emailType;
        private string _localSmtpServer;
        private string _localSmtpUserName;
        private string _localSmtpPassword;
        private int _localSmtpPort;
        private bool _localSmtpEnableSsl;
        private string _gmailSmtpServer;
        private string _gmailSmtpUserName;
        private string _gmailSmtpPassword;
        private int _gmailSmtpPort;
        private string _senderEmailAddress;
        private string _senderDisplayName;
        private SmtpClient _smtpClient;

        #endregion //Fields

        #region Properties

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = false; }
        }

        public EmailType EmailType
        {
            get { return _emailType; }
            set { _emailType = value; }
        }

        public string LocalSmtpServer
        {
            get { return _localSmtpServer; }
            set { _localSmtpServer = value; }
        }

        public string LocalSmtpUserName
        {
            get { return _localSmtpUserName; }
            set { _localSmtpUserName = value; }
        }

        public string LocalSmtpPassword
        {
            get { return _localSmtpPassword; }
            set { _localSmtpPassword = value; }
        }

        public int LocalSmtpPort
        {
            get { return _localSmtpPort; }
            set { _localSmtpPort = value; }
        }

        public bool LocalSmtpEnableSsl
        {
            get { return _localSmtpEnableSsl; }
            set { _localSmtpEnableSsl = value; }
        }

        public string GMailSMTPServer
        {
            get { return _gmailSmtpServer; }
            set { _gmailSmtpServer = value; }
        }

        public string GMailSmtpUserName
        {
            get { return _gmailSmtpUserName; }
            set { _gmailSmtpPassword = value; }
        }

        public string GMailSmtpPassword
        {
            get { return _gmailSmtpPassword; }
            set { _gmailSmtpPassword = value; }
        }

        public int GMailSmtpPort
        {
            get { return _gmailSmtpPort; }
            set { _gmailSmtpPort = value; }
        }

        public string SenderEmailAddress
        {
            get { return _senderEmailAddress; }
            set { _senderEmailAddress = value; }
        }

        public string SenderDisplayName
        {
            get { return _senderDisplayName; }
            set { _senderDisplayName = value; }
        }

        #endregion //Properties

        #region Methods

        public void SendEmailNotification(
            string subject,
            string body,
            List<string> fileAttachments,
            List<string> emailRecipients,
            bool sendAsync,
            bool sendLater)
        {
            if (!_enabled)
            {
                return;
            }
            MailMessage email = GetMailMessage(subject, body);
            if (emailRecipients != null)
            {
                emailRecipients.ForEach(p => email.To.Add(new MailAddress(p)));
            }
            List<MemoryStream> streams = null;
            try
            {
                if (fileAttachments != null)
                {
                    streams = new List<MemoryStream>();
                    foreach (string filePath in fileAttachments)
                    {
                        FileSystemHelper.ValidateFileExists(filePath);
                        byte[] fileBytes = File.ReadAllBytes(filePath);
                        MemoryStream ms = new MemoryStream(fileBytes);
                        streams.Add(ms);
                        string fileName = Path.GetFileName(filePath);
                        email.Attachments.Add(new Attachment(ms, fileName, MediaTypeNames.Text.Plain));
                    }
                }
                if (sendLater)
                {
                    SaveToDB(email, fileAttachments);
                    return;
                }
                StringBuilder logMessage = new StringBuilder();
                logMessage.AppendLine("Sending Notification Email");
                logMessage.AppendLine(string.Format("Subject: {0}", subject));
                email.To.ToList().ForEach(p => logMessage.AppendLine(p.Address));
                GOC.Instance.Logger.LogMessage(new LogMessage(logMessage.ToString(), LogMessageType.Information, LoggingLevel.Maximum));
                if (sendAsync)
                {
                    _smtpClient.SendAsync(email, email);
                }
                else
                {
                    _smtpClient.Send(email);
                }
            }
            finally
            {
                if (streams != null)
                {
                    foreach (MemoryStream ms in streams)
                    {
                        ms.Close();
                        ms.Dispose();
                    }
                }
            }
        }

        private void SaveToDB(MailMessage mailMessage, List<string> fileAttachments)
        {
            EmailNotification e = new EmailNotification() 
            { 
                EmailNotificationId = Guid.NewGuid(), 
                Subject = mailMessage.Subject,
                Body = mailMessage.Body,
                DateCreated = DateTime.Now 
            };
            mailMessage.To.ToList().ForEach(p => e.EmailNotificationRecipients.Add(new EmailNotificationRecipient()
            {
                EmailNotificationRecipientId = Guid.NewGuid(),
                EmailNotificationId = e.EmailNotificationId,
                RecipientEmailAddress = p.Address,
                DateCreated = DateTime.Now
            }));
            if (fileAttachments != null)
            {
                fileAttachments.ForEach(p => e.EmailNotificationAttachments.Add(new EmailNotificationAttachment()
                {
                    EmailNotificationAttachmentId = Guid.NewGuid(),
                    EmailNotificationId = e.EmailNotificationId,
                    AttachmentFilePath = p,
                    DateCreated = DateTime.Now
                }));
            }
            BcEntityContext.Create().Save<EmailNotification>(e, true);
        }

        private MailMessage GetMailMessage(string subject, string body)
        {
            MailMessage result = new MailMessage()
            {
                Sender = new MailAddress(_senderEmailAddress, _senderDisplayName),
                From = new MailAddress(_senderEmailAddress, _senderDisplayName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };
            return result;
        }

        private void InitializeSmtpClient()
        {
            if (_smtpClient == null)
            {
                _smtpClient = new SmtpClient();
            }
            switch (_emailType)
            {
                case EmailType.Exchange:
                    _smtpClient.Host = _localSmtpServer;
                    _smtpClient.Credentials = new NetworkCredential(_localSmtpUserName, _localSmtpPassword);
                    _smtpClient.Port = _localSmtpPort;
                    _smtpClient.EnableSsl = _localSmtpEnableSsl;
                    break;
                case EmailType.GMail:
                    _smtpClient.Host = _gmailSmtpServer;
                    _smtpClient.Credentials = new NetworkCredential(_gmailSmtpUserName, _gmailSmtpPassword);
                    _smtpClient.Port = _gmailSmtpPort;
                    _smtpClient.EnableSsl = true;
                    break;
                default:
                    break;
            }
            _smtpClient.SendCompleted += _smtpClient_SendCompleted;
        }

        private void _smtpClient_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("Async Email Send Response:");
            if(e.Error != null)
            {
                message.AppendLine(string.Format("Exception: {0}", e.Error.Message));
                if(e.Error.InnerException != null)
                {
                    message.AppendLine(string.Format("Inner Exception: {0}", e.Error.InnerException.Message));
                }
                message.AppendLine("Stack Trace:");
                message.AppendLine(e.Error.StackTrace);
                return;
            }
            MailMessage email = (MailMessage)e.UserState;
            message.AppendLine("Recipients:");
            foreach (MailAddress address in email.To)
            {
                message.AppendLine(address.Address);
            }
            message.AppendLine(string.Format("Subject: {0}", email.Subject));
            message.AppendLine("Body:");
            message.AppendLine(email.Body);
            GOC.Instance.Logger.LogMessage(new LogMessage(message.ToString(), LogMessageType.Information, LoggingLevel.Maximum));
        }

        #endregion //Methods
    }
}