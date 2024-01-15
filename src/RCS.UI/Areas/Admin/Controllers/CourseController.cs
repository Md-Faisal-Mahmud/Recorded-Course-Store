using Microsoft.AspNetCore.Mvc;

namespace RCS.UI.Areas.Admin.Controllers
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
