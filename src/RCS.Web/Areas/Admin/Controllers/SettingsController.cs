using Autofac;
using Microsoft.AspNetCore.Mvc;
using RCS.Web.Areas.Admin.Models;

namespace RCS.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SettingsController : Controller
    {
        private readonly ILifetimeScope _scope;

        public SettingsController(ILifetimeScope scope)
        {
            _scope = scope;
        }

        public IActionResult Roles()
        {
            return View();
        }

        public async Task<IActionResult> CreateRole()
        {
            var model = _scope.Resolve<RoleCreateModel>();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(RoleCreateModel model)
        {
            if (ModelState.IsValid)
            {
                model.ResolveDependency(_scope);
                await model.CreateRole();
            }
            return RedirectToAction(nameof(Roles));
        }

    }
}
