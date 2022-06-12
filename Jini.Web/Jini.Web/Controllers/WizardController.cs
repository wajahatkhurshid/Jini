using System.Web.Mvc;
using Gyldendal.Jini.Web.Models;

namespace Gyldendal.Jini.Web.Controllers
{
    [Authorize]
    public class WizardController : Controller
    {
        #region Views

        public ActionResult History()
        {
            return View("~/Views/Wizard/History/History.cshtml");
        }
        public ActionResult salesetup()
        {
            return View("~/Views/Wizard/Product/salesetup.cshtml");
        }
        public ActionResult accessprovider()
        {
            return View("~/Views/Wizard/Product/accessprovider.cshtml");
        }
        public ActionResult revisionHistory()
        {
            return View("~/Views/Wizard/History/RevisionHistory.cshtml");
        }
        public ActionResult guaRevisionHistory()
        {
            return View("~/Views/Wizard/History/GuaRevisionHistory.cshtml");
        }

        public ActionResult SalesConfiguration(DigitalTitle digitalTitle)
        {
            if (digitalTitle != null)
            {
                RedirectToAction("Index", "Jini");
            }
            if (digitalTitle.SubTitle == "null") digitalTitle.SubTitle = null;
            return View(digitalTitle);
        }

        public ActionResult Create()
        {
            return View("~/Views/Wizard/Create/Create.cshtml");
        }

        [HttpPost]
        public ActionResult Create(DigitalTitle digitalTitle)
        {
            return View("~/Views/Wizard/Create/Create.cshtml");
        }

        public ActionResult Config()
        {
            return View("~/Views/Wizard/Config/Config.cshtml");
        }

        public ActionResult Approve()
        {
            return View("~/Views/Wizard/Approve/Approve.cshtml");
        }

        public ActionResult Publish()
        {

            return View();
        }
        #endregion Views

        #region PartialViews

        #region Shared
        public ActionResult InfoBar()
        {
            return View();
        }

        public ActionResult WizardActionBar()
        {
            return View("~/Views/Wizard/Shared/WizardActionBar.cshtml");
        }

        public ActionResult WizardProgress()
        {
            return View();
        }

        #endregion Shared

        #region Tab1

        public ActionResult ProgressBar_Create()
        {
            return View("~/Views/Wizard/Create/ProgressBar.cshtml");
        }

        public ActionResult Dashboard()
        {
            return View("~/Views/Wizard/Product/ProductDashboard.cshtml");
        }


        #endregion Tab1

        #region Tab2
        public ActionResult TeacherLicense()
        {
            return View();
        }
        public ActionResult ClassLicense()
        {
            return View();
        }

        public ActionResult SchoolLicense()
        {
            return View("~/Views/Wizard/Tab2/SchoolLicense.cshtml");
        }

        public ActionResult SingleUserLicense()
        {
            return View();
        }

        public ActionResult ContactSalesLicense()
        {
            return View();
        }

        public ActionResult DefaultText()
        {
            return View("~/Views/Wizard/Tab2/DefaultText.cshtml");
        }
        public ActionResult ProgressBar_Config()
        {
            return View("~/Views/Wizard/Config/ProgressBar.cshtml");
        }

        public ActionResult Trial()
        {
            return View("~/Views/Wizard/Trial/Trial.cshtml");
        }

        public ActionResult ProgressBar_Trial()
        {
            return View("~/Views/Wizard/Trial/ProgressBar_Trial.cshtml");
        }

        #endregion Tab2

        #region Directives

        public ActionResult CreateLink()
        {
            return View("~/Views/Wizard/Trial/CreateLink.cshtml");
        }

        #endregion Directives

        #region Tab3

        public ActionResult ReviewSchoolLicense()
        {
            return View();
        }
        public ActionResult ReviewTeacherLicense()
        {
            return View();
        }
        public ActionResult ReviewClassLicense()
        {
            return View();
        }
        public ActionResult ReviewSingleUserLicense()
        {
            return View();
        }
        
            public ActionResult PriceInfoText()
        {
            return View();
        }

        public ActionResult ReviewContactSalesLicense()
        {
            return View();
        }
        public ActionResult ProgressBar_Approve()
        {
            return View("~/Views/Wizard/Approve/ProgressBar.cshtml");
        }

        #endregion Tab3

        #region History

        public ActionResult DraftPanel()
        {
            return View("~/Views/Wizard/History/DraftPanel.cshtml");
        }

        public ActionResult NewPanel()
        {
            return View("~/Views/Wizard/History/NewPanel.cshtml");
        }
        #endregion History

        #endregion PartialViews
    }
}