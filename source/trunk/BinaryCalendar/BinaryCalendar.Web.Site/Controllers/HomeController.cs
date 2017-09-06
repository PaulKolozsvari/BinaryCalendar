namespace BinaryCalendar.Web.Site.Controllers
{
    #region Using Directives

    using Figlut.Server.Toolkit.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    #endregion //Using Directives

    public class HomeController : BinaryController
    {
        #region Actions

        public ActionResult Index()
        {
            try
            {
                ViewBag.Message = "REST web service API for querying public holidays";
                return View();
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult About()
        {
            try
            {
                ViewBag.Message = "Binary Calendar";
                return View();
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult Contact()
        {
            try
            {
                ViewBag.Message = "Binary Chef";
                return View();
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
