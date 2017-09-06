namespace BinaryCalendar.Web.Service.Rest
{
    #region Using Directives

    using BinaryCalendar.DTO;
    using BinaryCalendar.ORM;
    using BinaryCalendar.Utilities.Data;
    using BinaryCalendar.Web.Service.Configuration;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Server.Toolkit.Utilities.Serialization;
    using Figlut.Server.Toolkit.Web.Client;
    using Figlut.Server.Toolkit.Web.Service.REST;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class BinaryCalendarRestService : RestService, IBinaryCalendarRestService
    {
        #region Constructors

        public BinaryCalendarRestService()
        {
        }

        #endregion //Constructors

        #region Methods

        #region Utility Methods

        internal virtual Stream GetStreamFromObject(object obj, SerializerType serializerType)
        {
            Stream result = null;
            switch (serializerType)
            {
                case SerializerType.XML:
                    result = StreamHelper.GetStreamFromString(GOC.Instance.XmlSerializer.SerializeToText(obj), GOC.Instance.Encoding);
                    break;
                case SerializerType.JSON:
                    result = StreamHelper.GetStreamFromString(GOC.Instance.JsonSerializer.SerializeToText(obj), GOC.Instance.Encoding);
                    break;
                case SerializerType.CSV:
                    result = StreamHelper.GetStreamFromString(GOC.Instance.CSVSerializer.SerializeToText(obj), GOC.Instance.Encoding);
                    break;
                default:
                    return StreamHelper.GetStreamFromString(GOC.Instance.JsonSerializer.SerializeToText(obj), GOC.Instance.Encoding);
            }
            return result;
        }

        protected User GetCurrentUser()
        {
            User result = null;
            if (ServiceSecurityContext.Current != null && !string.IsNullOrEmpty(ServiceSecurityContext.Current.PrimaryIdentity.Name))
            {
                string userName = ServiceSecurityContext.Current.PrimaryIdentity.Name;
                result = BcEntityContext.Create().GetUser(userName, false, true);
            }
            return result;
        }

        protected bool IsCurrentUserSystemAdministrator()
        {
            if (ServiceSecurityContext.Current != null && !string.IsNullOrEmpty(ServiceSecurityContext.Current.PrimaryIdentity.Name))
            {
                string userName = ServiceSecurityContext.Current.PrimaryIdentity.Name;
                bool result = BcEntityContext.Create().IsUserSystemAdministrator(userName);
                return result;
            }
            return false;
        }

        protected void AuditServiceCall(string message)
        {
            if (BcServiceApp.Instance.Settings.AuditServiceCalls)
            {
                GOC.Instance.Logger.LogMessage(new LogMessage(message, LogMessageType.SuccessAudit, LoggingLevel.Maximum));
            }
        }

        #endregion //Utility Methods

        #region Rest Services

        #region Base Methods

        public override Stream AllURIs()
        {
            try
            {
                string allHeadersFullString = GetAllHeadersFullString();
                StringBuilder message = new StringBuilder();
                string applicationName = string.IsNullOrEmpty(GOC.Instance.ApplicationName) ? "REST Web Service" : GOC.Instance.ApplicationName;
                message.AppendLine(applicationName);
                message.AppendLine();
                message.AppendLine("About: a REST web service API for querying public holidays in specific countries");
                message.AppendLine("Author: Paul Kolozsvari @ Binary Chef (www.binarychef.com)");
                message.AppendLine("Supported Data Formats: JSON, XML and CSV");
                message.AppendLine();
                message.AppendLine("HTTP Headers:");
                message.AppendLine();
                message.AppendLine(allHeadersFullString);
                message.AppendLine();

                message.AppendLine("API Guide:");
                message.AppendLine();
                message.AppendLine("*** Countries ***");
                message.AppendLine(string.Format("\tQuery String: /{0}", "CountryInfo/{format}"));
                message.AppendLine(string.Format("\tURL : {0}{1}", base.GetCurrentRequestUri(), "CountryInfo/{format}"));
                message.AppendLine("\tComments: Get a list of supported countries and their respective country codes.");
                message.AppendLine();

                message.AppendLine("*** Public Holidays ***");
                message.AppendLine(string.Format("\tQuery String: {0}", "PublicHolidayInfo/{countryCode}/{format}/searchBy?year={year}&month={month}&day={day}"));
                message.AppendLine(string.Format("\tURL: {0}{1}", base.GetCurrentRequestUri(), "PublicHolidayInfo/{countryCode}/{format}/searchBy?year={year}&month={month}&day={day}"));
                message.AppendLine("\tComments: Get public holiday details for a specific country (code). Can filter by year, month and day.");
                message.AppendLine("\tExamples:");
                message.AppendLine(string.Format("\t\t{0}{1}", base.GetCurrentRequestUri(), "PublicHolidayInfo/ZA/json/searchBy?year=&month=&day="));
                message.AppendLine("\t\tNotes: Gets all public holidays for South Africa (Country Code = ZA) in current year. Returns result in JSON format");
                message.AppendLine();
                message.AppendLine(string.Format("\t\t{0}{1}", base.GetCurrentRequestUri(), "PublicHolidayInfo/ZA/xml/searchBy?year=2016&month=&day="));
                message.AppendLine("\t\tNotes: Gets all public holidays for South Africa (Country Code = ZA) in 2016. Returns result in XML format");
                message.AppendLine();
                message.AppendLine(string.Format("\t\t{0}{1}", base.GetCurrentRequestUri(), "PublicHolidayInfo/ZA/csv/searchBy?year=2016&month=1&day="));
                message.AppendLine("\t\tNotes: Gets all public holidays for South Africa (Country Code = ZA) in 2016, January (1). Returns result in CSV format");
                message.AppendLine();
                message.AppendLine(string.Format("\t\t{0}{1}", base.GetCurrentRequestUri(), "PublicHolidayInfo/ZA/json/searchBy?year=2016&month=&day=1"));
                message.AppendLine("\t\tNotes: Gets all public holidays for South Africa (Country Code = ZA) in 2016 that are on the 1st of every month.");

                return StreamHelper.GetStreamFromString(
                    message.ToString(),
                    GOC.Instance.Encoding);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                UpdateHttpStatusOnException(ex);
                throw ex;
            }
        }

        public override Stream GetEntities(string entityName)
        {
            try
            {
                if (BcServiceApp.Instance.IsAdministratorTable(entityName) && !IsCurrentUserSystemAdministrator())
                {
                    return StreamHelper.GetStreamFromString("Unauthorized.", GOC.Instance.Encoding);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                UpdateHttpStatusOnException(ex);
                throw ex;
            }
            return base.GetEntities(entityName);
        }

        #endregion //Base Methods

        public Stream Login()
        {
            try
            {
                ValidateRequestMethod(HttpVerb.GET);
                User u = GetCurrentUser();
                AuditServiceCall(string.Format("Login: {0}", u.UserName));
                return GetStreamFromObject(u);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                UpdateHttpStatusOnException(ex);
                throw ex;
            }
        }

        public Stream GetCountryInfos()
        {
            return GetCountryInfos(null);
        }

        private SerializerType GetSerializerTypeFromFormat(string format, out Stream errorStream)
        {
            errorStream = null;
            SerializerType serializerType;
            if (!Enum.TryParse<SerializerType>(format.Trim().ToUpper(), out serializerType))
            {
                errorStream = StreamHelper.GetStreamFromString(string.Format(
                    "Could not parse format '{0}' to a valid serializer type.", format),
                    GOC.Instance.Encoding);
            }
            return serializerType;
        }

        public Stream GetCountryInfos(string format)
        {
            try
            {
                ValidateRequestMethod(HttpVerb.GET);
                User u = GetCurrentUser();
                AuditServiceCall(string.Format("GetCountryCodes: {0}", u.UserName));
                List<CountryInfo> result = BcEntityContext.Create().GetCountryInfos();
                if (string.IsNullOrEmpty(format))
                {
                    return GetStreamFromObject(result);
                }
                Stream errorStream = null;
                SerializerType serializerType = GetSerializerTypeFromFormat(format, out errorStream);
                if (errorStream != null)
                {
                    return errorStream;
                }
                return GetStreamFromObject(result, serializerType);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                UpdateHttpStatusOnException(ex);
                throw ex;
            }
        }

        public Stream GetPublicHolidays(string countryCode, string format, string year, string month, string day)
        {
            try
            {
                ValidateRequestMethod(HttpVerb.GET);
                User u = GetCurrentUser();
                AuditServiceCall(string.Format("GetPublicHolidays: {0}", u.UserName));

                int yearInt;
                Nullable<int> monthInt = null;
                Nullable<int> dayInt = null;
                if (string.IsNullOrEmpty(year) || year == "*")
                {
                    yearInt = DateTime.Now.Year;
                }
                else
                {
                    if (!int.TryParse(year, out yearInt))
                    {
                        return StreamHelper.GetStreamFromString(string.Format(
                            "Could not parse Year value '{0}' to an integer.", year), 
                            GOC.Instance.Encoding);
                    }
                }
                if (string.IsNullOrEmpty(month) || month == "*")
                {
                    monthInt = null;
                }
                else
                {
                    int monthResult;
                    if (!int.TryParse(month, out monthResult))
                    {
                        return StreamHelper.GetStreamFromString(string.Format(
                            "Could not parse MOnth value '{0}' to an integer.", month),
                            GOC.Instance.Encoding);
                    }
                    monthInt = monthResult;
                }
                if (string.IsNullOrEmpty(day) || day == "*")
                {
                    dayInt = null;
                }
                else
                {
                    int dayResult;
                    if (!int.TryParse(day, out dayResult))
                    {
                        return StreamHelper.GetStreamFromString(string.Format(
                            "Could not parse MOnth value '{0}' to an integer.", day),
                            GOC.Instance.Encoding);
                    }
                    dayInt = dayResult;
                }
                List<PublicHolidayInfo> result = BcEntityContext.Create().GetPublicHolidayInfos(countryCode, yearInt, monthInt, dayInt);
                if (string.IsNullOrEmpty(format))
                {
                    return GetStreamFromObject(result);
                }
                Stream errorStream = null;
                SerializerType serializerType = GetSerializerTypeFromFormat(format, out errorStream);
                if (errorStream != null)
                {
                    return errorStream;
                }
                return GetStreamFromObject(result, serializerType);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                UpdateHttpStatusOnException(ex);
                throw ex;
            }
        }

        #endregion //Rest Services

        #endregion //Methods
    }
}
