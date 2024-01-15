using Autofac;
using Microsoft.AspNetCore.Mvc;
using RCS.UI.Areas.Admin.Models;
using RCS.UI.Models;
using RCS.UI.Utilities;
using System.Data;

namespace RCS.UI.Areas.Admin.Controllers
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
            return View();
        }

        public async Task<IActionResult> Create()
        {
            var model = _scope.Resolve<CourseCreateModel>();

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseCreateModel model)
        {
            model.ResolveDependency(_scope);
            if (ModelState.IsValid)
            {
                try
                {
                    
                    await model.CreateCourseAsync();
                    TempData.Put<ResponseModel>("ResponseMessage", new ResponseModel
                    {
                        Message = "Successfully added a new course.",
                        Type = ResponseTypes.Success
                    });
                    return RedirectToAction("Index");
                }
                catch (DuplicateNameException ex)
                {
                    _logger.LogError(ex, ex.Message);
                    TempData.Put<ResponseModel>("ResponseMessage", new ResponseModel
                    {
                        Message = ex.Message,
                        Type = ResponseTypes.Danger
                    });
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Server Error");

                    TempData.Put<ResponseModel>("ResponseMessage", new ResponseModel
                    {
                        Message = "There was a problem in creating course.",
                        Type = ResponseTypes.Danger
                    });
                }
            }

            return View(nameof(Index));
        }
    }
}
