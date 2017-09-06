namespace BinaryCalendar.Web.Site.Controllers
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    #endregion //Using Directives

    public class DocumentationController : BinaryController
    {
        #region Indexers Region

        public ActionResult Index()
        {
            return View();
        }

        #endregion //Indexers Region
    }
}
