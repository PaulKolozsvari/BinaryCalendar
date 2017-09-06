namespace BinaryCalendar.Web.Service.Configuration
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class CountryCalendarImportInfo
    {
        #region Properties

        public string CountryCode { get; set; }

        public string CountryName { get; set; }

        public string ICalendarImportDirectory { get; set; }

        #endregion //Properties
    }
}