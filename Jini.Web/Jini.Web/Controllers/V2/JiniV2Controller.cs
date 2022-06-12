using System.Linq;
using System.Web.Mvc;
using Gyldendal.Jini.Services.Contracts;
using Gyldendal.Jini.Web.Facade;
using Gyldendal.Jini.Web.Facade.V2;
using Gyldendal.Jini.Web.Properties;

namespace Gyldendal.Jini.Web.Controllers.V2
{
    [Authorize]
    public class JiniV2Controller : Controller
    {
        // GET: jiniV2d
        [Authorize]
        public ActionResult Index()
        {
            return View(@"~\Views\V2\Jini\Index.cshtml");
        }
        public JsonResult GetDepartmentsAndEditorial()
        {
            var departments = new JiniFacadeV2().GetDepartmentsAndEditorials<Department>();
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

        public JsonResult GetMediaAndMaterialTypes()
        {
            var mediaTreeResult = new JiniFacadeV2().GetMediaAndMaterialTypes<MediaType>().Select(m => new
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