using Microsoft.AspNetCore.Mvc;

namespace RCS.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CourseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
