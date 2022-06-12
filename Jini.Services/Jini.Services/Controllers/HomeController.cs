using System.Web.Mvc;

namespace Gyldendal.Jini.Services.Controllers
{
    /// <summary>
    /// Landing Page
    /// </summary>
    public class HomeController : Controller
    {
        // GET: Home
        /// <summary>
        /// Landing Page
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}