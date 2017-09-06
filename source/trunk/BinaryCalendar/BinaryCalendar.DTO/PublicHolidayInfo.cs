namespace BinaryCalendar.DTO
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class PublicHolidayInfo
    {
        #region Properties

        public string CountryCode { get; set; }

        public string CountryName { get; set; }

        public string EventName { get; set; }

        public string DateIdentifier { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public int Day { get; set; }

        public DateTime HolidayDate { get; set; }

        #endregion //Properties
    }
}