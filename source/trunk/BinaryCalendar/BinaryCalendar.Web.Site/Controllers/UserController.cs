namespace BinaryCalendar.Web.Site.Controllers
{
    #region Using Directives

    using BinaryCalendar.ORM;
    using BinaryCalendar.Utilities.Data;
    using BinaryCalendar.Web.Site.Models;
    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    #endregion //Using Directives

    public class UserController : BinaryController
    {
        #region Actions

        public ActionResult Register()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            try
            {
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                User user = model.CreateUser(BcRole.RestApiUser);
                BcEntityContext context = BcEntityContext.Create();
                errorMessage = null;
                if (!context.RegisterUser(user, out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                FormsAuthentication.SetAuthCookie(user.UserName, true);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult Login()
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                return View();
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Login(User model)
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                if (string.IsNullOrEmpty(model.UserName))
                {
                    return GetJsonResult(false, string.Format("{0} not entered.", EntityReader<User>.GetPropertyName(p => p.UserName, true)));
                }
                BcEntityContext context = BcEntityContext.Create();
                User user = context.Login(model.UserName, model.Password);
                if (user == null)
                {
                    return GetJsonResult(false, "Invalid user name or password.");
                }
                FormsAuthentication.SetAuthCookie(user.UserName, true);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult LogOff()
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                FormsAuthentication.SignOut();
                return RedirectToHome();
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                return RedirectToError(ex.Message);
            }
        }

        #endregion //Actions
    }
}
