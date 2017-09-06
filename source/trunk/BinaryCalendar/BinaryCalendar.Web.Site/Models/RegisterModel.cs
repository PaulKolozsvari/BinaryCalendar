namespace BinaryCalendar.Web.Site.Models
{
    #region Using Directives

    using BinaryCalendar.ORM;
using BinaryCalendar.Utilities.Data;
using Figlut.Server.Toolkit.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

    #endregion //Using Directives

    public class RegisterModel
    {
        #region Properties

        public string UserName { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string EmailAddress { get; set; }

        #endregion //Properties

        #region Methods

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(this.UserName))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RegisterModel>.GetPropertyName(p => p.UserName, true));
            }
            else if (string.IsNullOrEmpty(this.Password))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RegisterModel>.GetPropertyName(p => p.Password, true));
            }
            else if (string.IsNullOrEmpty(this.ConfirmPassword))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RegisterModel>.GetPropertyName(p => p.ConfirmPassword, true));
                return false;
            }
            else if (this.Password != this.ConfirmPassword)
            {
                errorMessage = string.Format("{0} and {1} do not match.", 
                    EntityReader<RegisterModel>.GetPropertyName(p => p.Password, true), 
                    EntityReader<RegisterModel>.GetPropertyName(p => p.ConfirmPassword, true));
            }
            else if (string.IsNullOrEmpty(this.EmailAddress))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RegisterModel>.GetPropertyName(p => p.EmailAddress, true));
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        public User CreateUser(BcRole role)
        {
            return new User()
            {
                UserId = Guid.NewGuid(),
                UserName = this.UserName,
                Password = this.Password,
                EmailAddress = this.EmailAddress,
                Role = (int)role,
                DateCreated = DateTime.Now
            };
        }

        #endregion //Methods
    }
}