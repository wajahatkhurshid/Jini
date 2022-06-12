using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gyldendal.Jini.Web.Controllers.Version2
{
    [Authorize]
    public class BulkV2Controller : Controller
    {
        // GET: BulkV2
        [Authorize]
        public ActionResult Index()
        {
            return View(@"~\Views\V2\Bulk\Index.cshtml");
        }
        [HttpPost]
        public ActionResult SaveFilters(string filter)
        {
            Session["BulkGridFilters"] = filter;
            return new EmptyResult();
        }
        public ActionResult LoadFilters()
        {
            if (Session["BulkGridFilters"] == null)
                return Json("No Filters", JsonRequestBehavior.AllowGet);
            return Json(Session["BulkGridFilters"], JsonRequestBehavior.AllowGet);
        }
    }
}