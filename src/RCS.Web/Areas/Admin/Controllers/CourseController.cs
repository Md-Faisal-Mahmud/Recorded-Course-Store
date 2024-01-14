using Autofac;
using Microsoft.AspNetCore.Mvc;

namespace RCS.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CourseController : Controller
    {
        ILifetimeScope _scope;
        ILogger<CourseController> _logger;


        public CourseController(ILifetimeScope scope, ILogger<CourseController> logger)
        {
            _scope = scope;
            _logger = logger;
        }
        public IActionResult Index()
        {
            _logger.LogInformation("Visited Course Index");
            return View();
        }
    }
}
