namespace BinaryCalendar.Web.Site.Controllers
{
    #region Using Directives

    using BinaryCalendar.ORM;
    using BinaryCalendar.Utilities.Data;
    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;

    #endregion //Using Directives

    public class BinaryController : Controller
    {
        #region Methods

        protected string GetCurrentActionName()
        {
            string result = this.ControllerContext.RouteData.Values["action"].ToString();
            return result;
        }

        protected string GetCurrentControllerName()
        {
            string result = this.ControllerContext.RouteData.Values["controller"].ToString();
            return result;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string actionName = filterContext.ActionDescriptor.ActionName;
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            if (actionName.Contains("Dialog") || actionName.ToLower().Contains("dialog"))
            {
                return;
            }
            //LogHeaders();
            base.OnActionExecuting(filterContext);
        }

        protected void LogHeaders()
        {
            string allHeadersFullString = GetAllHeadersFullString();
            string allHeadersFormatted = GetAllHeadersFormatted();
            GOC.Instance.Logger.LogMessage(new LogMessage(allHeadersFullString, LogMessageType.Information, LoggingLevel.Maximum));
        }

        public void SetViewBagErrorMessage(string errorMessage)
        {
            ViewBag.ErrorMessage = errorMessage;
        }

        public JsonResult GetJsonResult(bool success)
        {
            return Json(new { Success = success, ErrorMsg = string.Empty });
        }

        public JsonResult GetJsonResult(bool success, string errorMessage)
        {
            return Json(new { Success = success, ErrorMsg = errorMessage });
        }

        public JsonResult GetJsonFileResult(bool success, string fileName)
        {
            return Json(new { Success = success, FileName = fileName });
        }

        public RedirectToRouteResult RedirectToHome()
        {
            return RedirectToAction("Index", "Home");
        }

        public RedirectToRouteResult RedirectToError(string message)
        {
            return RedirectToAction("Error", "Information", new { errorMessage = message });
        }

        public RedirectToRouteResult RedirectToInformation(string message)
        {
            return RedirectToAction("Information", "Information", new { informationMessage = message });
        }

        public RedirectToRouteResult RedirectToIndex()
        {
            return RedirectToAction("Index");
        }

        public User GetUser(string userName, BcEntityContext context, bool throwExceptionOnNotFound)
        {
            if (context == null)
            {
                context = BcEntityContext.Create();
            }
            return context.GetUser(userName, false, throwExceptionOnNotFound);
        }

        public User GetCurrentUser(BcEntityContext context)
        {
            return GetUser(this.User.Identity.Name, context, true);
        }

        public bool IsCurrentUserOfRole(BcRole roleToCheck, BcEntityContext context)
        {
            if (context == null)
            {
                context = BcEntityContext.Create();
            }
            return context.IsUserOfRole(this.User.Identity.Name, roleToCheck);
        }

        public bool IsCurrentUserSystemAdministrator(BcEntityContext context)
        {
            if (context == null)
            {
                context = BcEntityContext.Create();
            }
            return context.IsUserOfRole(this.User.Identity.Name, BcRole.Administrator);
        }

        #region Header Methods

        public string GetAllHeadersFullString()
        {
            return Request.Headers.ToString();
        }

        public string GetAllHeadersFormatted()
        {
            StringBuilder result = new StringBuilder();
            foreach (var key in Request.Headers.AllKeys)
            {
                result.AppendLine(string.Format("{0}={1}", key, Request.Headers[key]));
            }
            return result.ToString();
        }

        public string GetHeader(string key, bool throwExceptionOnNotFound)
        {
            string result = string.Empty;
            if (Request != null && Request.Headers.HasKeys())
            {
                result = Request.Headers.Get(key);
            }
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find HTTP Header with key {0}.", key));
            }
            return result;
        }

        public FileContentResult GetCsvFileResult<E>(EntityCache<Guid, E> cache) where E : class
        {
            string filePath = Path.GetTempFileName();
            cache.ExportToCsv(filePath, null, false, false);
            string downloadFileName = string.Format("{0}-{1}.csv", typeof(E).Name, DateTime.Now);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileBytes, "text/plain", downloadFileName);
        }

        protected void GetConfirmationModelFromSearchParametersString(
            string searchParametersString,
            out string[] searchParameters,
            out string searchText)
        {
            searchText = string.Empty;
            searchParameters = searchParametersString.Split('|');
            if (!string.IsNullOrEmpty(searchParametersString) && searchParameters.Length > 0)
            {
                searchText = searchParameters[0];
            }
        }

        protected void GetConfirmationModelFromSearchParametersString(
            string searchParametersString,
            out string[] searchParameters,
            out string searchText,
            out Nullable<DateTime> startDate,
            out Nullable<DateTime> endDate)
        {
            searchText = string.Empty;
            startDate = null;
            endDate = null;
            searchParameters = searchParametersString.Split('|');
            if (!string.IsNullOrEmpty(searchParametersString) && searchParameters.Length >= 3)
            {
                searchText = searchParameters[0];
                DateTime startDateParsed;
                DateTime endDateParsed;
                if (DateTime.TryParse(searchParameters[1], out startDateParsed))
                {
                    startDate = startDateParsed;
                }
                if (DateTime.TryParse(searchParameters[2], out endDateParsed))
                {
                    endDate = endDateParsed;
                }
            }
        }

        #endregion //Header Methods

        #endregion //Methods
    }
}
