using System.Web.Mvc;
using System.Linq;
using Gyldendal.Jini.Services.Contracts;
using Gyldendal.Jini.Web.Facade;
using Gyldendal.Jini.Web.Properties;

namespace Gyldendal.Jini.Web.Controllers
{
    [Authorize]
    public class BulkController : Controller
    {
        // GET: Home
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.BodyClass = "main-grid";
            return View();
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