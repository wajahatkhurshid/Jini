using Gyldendal.Jini.Services.Contracts;
using Gyldendal.Jini.Web.Facade;
using Gyldendal.Jini.Web.Properties;
using System.Linq;
using System.Web.Mvc;

namespace Gyldendal.Jini.Web.Controllers
{
    [Authorize]
    public class JiniController : Controller
    {
        // GET: Home
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.BodyClass = "main-grid";
            return View();
        }

        public JsonResult GetDepartmentsAndEditorial()
        {
            var departments = new JiniFacade().GetDepartmentsAndEditorials<Department>();
            var deptResult = departments.Select(a => new
            {
                Id = a.Code,
                Text = a.Name,
                HasChildren = a.Sections.Any(),
                Items = a.Sections.OrderBy(s => s.SectionName).Select(r => new
                {
                    Text = r.SectionName,
                    Id = r.SectionCode
                }).ToList()
            }).OrderBy(o => o.Text);
            var result = new[]
            {
                        new
                        {
                            Id = "-1",
                            Text = Settings.Default.CheckAllText,
                            HasChildren = deptResult.Any(),
                            Items = deptResult.ToList()
                        }
                    };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMediaAndMaterialeTypes()
        {
            var mediaTreeResult = new JiniFacade().GetMediaAndMaterialeTypes<MediaType>().Select(m => new
            {
                Id = m.MediaTypeCode,
                Text = m.MediaTypeName,
                Expanded = true,
                HasChildren = m.ListOfMaterialTypes.Any(),
                Checked = true,
                Items = m.ListOfMaterialTypes.OrderBy(mt => mt.MaterialTypeName).Select(r => new
                {
                    Text = r.MaterialTypeName,
                    Id = r.MaterialTypeCode,
                    Checked = true,
                    Expanded = true
                }).ToList()
            }).OrderBy(o => o.Text);
            var result = new[]
            {
                        new
                        {
                            Id = "-1",
                            Text = Settings.Default.CheckAllText,
                            HasChildren = mediaTreeResult.Any(),
                            Items = mediaTreeResult.ToList()
                        }
                    };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveFilters(string filter)
        {
            Session["GridFilters"] = filter;
            return new EmptyResult();
        }
        public ActionResult LoadFilters()
        {
            if (Session["GridFilters"] == null)
                return Json("No Filters", JsonRequestBehavior.AllowGet);
            return Json(Session["GridFilters"], JsonRequestBehavior.AllowGet);
        }
    }
}