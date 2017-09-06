namespace BinaryCalendar.Utilities.Data
{
    #region Using Directives

    using BinaryCalendar.DTO;
    using BinaryCalendar.ORM;
    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Server.Toolkit.Data.iCalendar;
    using Figlut.Server.Toolkit.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;

    #endregion //Using Directives

    public class BcEntityContext : EntityContext
    {
        #region Constructors

        public BcEntityContext(
            DataContext db,
            LinqFunnelSettings settings,
            bool handleExceptions,
            Type userLinqToSqlType,
            Type serverActionLinqToSqlType,
            Type serverErrorLinqToSqlType)
            : base(db, settings, handleExceptions, userLinqToSqlType, serverActionLinqToSqlType, serverErrorLinqToSqlType)
        {
        }

        #endregion //Constructors

        #region Methods


        public static BcEntityContext Create()
        {
            return new BcEntityContext(
                GOC.Instance.GetNewLinqToSqlDataContext(),
                GOC.Instance.GetByTypeName<LinqFunnelSettings>(),
                false,
                GOC.Instance.UserLinqToSqlType,
                GOC.Instance.ServerActionLinqToSqlType,
                GOC.Instance.ServerErrorLinqToSqlType);
        }

        #region User

        public User Login(string userIdentifier, string password)
        {
            string userIdentifierLower = userIdentifier.ToLower();
            List<User> queryResult = (from u in DB.GetTable<User>()
                                      where (u.UserName.ToLower() == userIdentifierLower || u.EmailAddress.ToLower() == userIdentifierLower) && u.Password == password
                                      select u).ToList();
            return queryResult.Count < 1 ? null : queryResult.First();
        }

        public User GetUser(Guid userId, bool throwExceptionOnNotFound)
        {
            List<User> queryResult = (from u in DB.GetTable<User>()
                                      where u.UserId == userId
                                      orderby u.UserName
                                      select u).ToList();
            User result = queryResult.Count < 1 ? null : queryResult.First();
            if (throwExceptionOnNotFound && result == null)
            {
                throw new NullReferenceException(string.Format(
                    "Could not find {0} with {1} of '{2}'.",
                    typeof(User).Name,
                    EntityReader<User>.GetPropertyName(p => p.UserId, false),
                    userId.ToString()));
            }
            return result;
        }

        public User GetUser(string userName, bool caseSensitive, bool throwExceptionOnNotFound)
        {
            string userNameToSearch = caseSensitive ? userName : userName.Trim().ToLower();
            List<User> queryResult = (from u in DB.GetTable<User>()
                                      where u.UserName.ToLower() == userNameToSearch
                                      orderby u.UserName
                                      select u).ToList();
            User result = queryResult.Count < 1 ? null : queryResult.First();
            if (throwExceptionOnNotFound && result == null)
            {
                throw new NullReferenceException(string.Format(
                    "Could not find {0} with {1} of '{2}'.",
                    typeof(User).Name,
                    EntityReader<User>.GetPropertyName(p => p.UserName, false),
                    userNameToSearch));
            }
            return result;
        }

        public User GetUserByEmailAddress(string emailAddress, bool caseSensitive, bool throwExceptionOnNotFound)
        {
            string emailAddressToSearch = caseSensitive ? emailAddress : emailAddress.Trim().ToLower();
            List<User> queryResult = (from u in DB.GetTable<User>()
                                      where u.EmailAddress.ToLower() == emailAddressToSearch
                                      orderby u.EmailAddress
                                      select u).ToList();
            User result = queryResult.Count < 1 ? null : queryResult.First();
            if(throwExceptionOnNotFound && result == null)
            {
                throw new NullReferenceException(string.Format(
                    "Could not find {0} with {1} of '{2}'.",
                    typeof(User).Name,
                    EntityReader<User>.GetPropertyName(p => p.EmailAddress, false),
                    emailAddressToSearch));
            }
            return result;
        }

        public bool IsUserOfRole(string userName, BcRole roleToCheck)
        {
            User user = GetUser(userName, false, true);
            BcRole userRole = (BcRole)user.Role;
            return (userRole & roleToCheck) != BcRole.None;
        }

        public bool IsUserSystemAdministrator(string userName)
        {
            User user = GetUser(userName, false, true);
            BcRole role = (BcRole)user.Role;
            bool result = (role & BcRole.Administrator) == BcRole.Administrator;
            return result;
        }

        public bool RegisterUser(User user, out string errorMessage)
        {
            errorMessage = null;
            using (TransactionScope t = new TransactionScope())
            {
                User originalUser = GetUser(user.UserName, false, false);
                if (originalUser != null)
                {
                    errorMessage = string.Format("{0} with {1} of '{2}' already exists.",
                        typeof(User).Name,
                        EntityReader<User>.GetPropertyName(p => p.UserName, true),
                        user.UserName);
                    return false;
                }
                originalUser = GetUserByEmailAddress(user.EmailAddress, false, false);
                if (originalUser != null)
                {
                    errorMessage = string.Format("{0} with {1} of '{2}' already exists.",
                        typeof(User).Name,
                        EntityReader<User>.GetPropertyName(p => p.EmailAddress, true),
                        user.EmailAddress);
                    return false;
                }
                DB.GetTable<User>().InsertOnSubmit(user);
                DB.SubmitChanges();
                t.Complete();
            }
            return true;
        }

        #endregion //User

        #region Public Holidays

        public PublicHoliday GetPublicHoliday(Guid publicHolidayId, bool throwExceptionOnNotFound)
        {
            List<PublicHoliday> queryResult = (from p in DB.GetTable<PublicHoliday>()
                                               where p.PublicHolidayId == publicHolidayId
                                               select p).ToList();
            PublicHoliday result = queryResult.Count < 1 ? null : queryResult.First();
            if (result == null && throwExceptionOnNotFound)
            {
                throw new ArgumentNullException(string.Format("Could not find {0} with {1} of {2}.",
                    typeof(PublicHoliday).Name,
                    EntityReader<PublicHoliday>.GetPropertyName(p => p.PublicHolidayId, false),
                    publicHolidayId));
            }
            return result;
        }

        public PublicHoliday GetPublicHoliday(string countryCode, string dateIdentifier, bool throwExceptionOnNotFound)
        {
            string countryCodeLower = countryCode.ToLower();
            List<PublicHoliday> queryResult = (from p in DB.GetTable<PublicHoliday>()
                                               where p.CountryCode == countryCodeLower && p.DateIdentifier == dateIdentifier
                                               select p).ToList();
            PublicHoliday result = queryResult.Count < 1 ? null : queryResult.First();
            if (result == null && throwExceptionOnNotFound)
            {
                throw new ArgumentNullException(string.Format("Could not find {0} with {1} of {2} and {3} of {4}.",
                    typeof(PublicHoliday).Name,
                    EntityReader<PublicHoliday>.GetPropertyName(p => p.CountryCode, false),
                    countryCode,
                    EntityReader<PublicHoliday>.GetPropertyName(p => p.DateIdentifier, false),
                    dateIdentifier));
            }
            return result;
        }

        public List<CountryInfo> GetCountryInfos()
        {
            List<CountryInfo> result = (from p in DB.GetTable<PublicHoliday>()
                                        orderby p.CountryName, p.HolidayDate
                                        select new CountryInfo() { CountryCode = p.CountryCode, CountryName = p.CountryName }).Distinct().ToList();
            return result;
        }

        public List<PublicHolidayInfo> GetPublicHolidayInfos(string countryCode, int year, Nullable<int> month, Nullable<int> day)
        {
            string countryCodeLower = countryCode.ToLower();
            List<PublicHolidayInfo> result = null;
            Func<PublicHoliday, bool> whereClause = null;
            if (month.HasValue && day.HasValue)
            {
                whereClause = p => (p.CountryCode.ToLower() == countryCodeLower &&  p.Year == year && p.Month == month.Value &&  p.Day == day.Value);
            }
            else if (month.HasValue)
            {
                whereClause = p => (p.CountryCode.ToLower() == countryCodeLower && p.Year == year && p.Month == month.Value);
            }
            else if (day.HasValue)
            {
                whereClause = p => (p.CountryCode.ToLower() == countryCodeLower && p.Year == year && p.Day == day.Value);
            }
            else
            {
                whereClause = p => (p.CountryCode.ToLower() == countryCodeLower && p.Year == year);
            }
            result = (from p in DB.GetTable<PublicHoliday>().Where(whereClause)
                      orderby p.HolidayDate
                      select new PublicHolidayInfo()
                      {
                          CountryCode = p.CountryCode,
                          CountryName = p.CountryName,
                          EventName = p.EventName,
                          DateIdentifier = p.DateIdentifier,
                          Year = p.Year,
                          Month = p.Month,
                          Day = p.Day,
                          HolidayDate = p.HolidayDate
                      }).ToList();
            return result;
        }

        public void SaveICalCalendar(ICalCalendar calendar)
        {
            using (TransactionScope t = new TransactionScope())
            {
                foreach (ICalPublicHoliday p in calendar.PublicHolidays)
                {
                    PublicHoliday original = GetPublicHoliday(calendar.CountryCode, p.DateIdentifier, false);
                    if (original == null)
                    {
                        DB.GetTable<PublicHoliday>().InsertOnSubmit(new PublicHoliday()
                        {
                            PublicHolidayId = Guid.NewGuid(),
                            CountryCode = calendar.CountryCode,
                            CountryName = calendar.CountryName,
                            EventName = p.EventName,
                            DateIdentifier = p.DateIdentifier,
                            Year = p.Year,
                            Month = p.Month,
                            Day = p.Day,
                            HolidayDate = p.GetDate(),
                            DateCreated = DateTime.Now
                        });
                    }
                    else
                    {
                        original.CountryCode = calendar.CountryCode;
                        original.CountryName = calendar.CountryName;
                        original.EventName = p.EventName;
                        original.DateIdentifier = p.DateIdentifier;
                        original.Year = p.Year;
                        original.Month = p.Month;
                        original.Day = p.Day;
                        original.HolidayDate = p.GetDate();
                    }
                    DB.SubmitChanges();
                }
                t.Complete();
            }
        }

        #endregion //Public Holidays

        #endregion //Methods
    }
}