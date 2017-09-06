namespace BinaryCalendar.Web.Service.Rest
{
    #region Using Directives

    using Figlut.Server.Toolkit.Web.Service.REST;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    [ServiceContract]
    public interface IBinaryCalendarRestService : IRestService
    {
        #region Methods

        [OperationContract]
        [WebGet(UriTemplate = "/Login")]
        Stream Login();

        [OperationContract]
        [WebGet(UriTemplate = "/CountryInfo/{format}")]
        Stream GetCountryInfos(string format);

        [OperationContract]
        [WebGet(UriTemplate = "/PublicHolidayInfo/{countryCode}/{format}/searchBy?year={year}&month={month}&day={day}")]
        Stream GetPublicHolidays(string countryCode, string format, string year, string month, string day);

        #endregion //Methods
    }
}
