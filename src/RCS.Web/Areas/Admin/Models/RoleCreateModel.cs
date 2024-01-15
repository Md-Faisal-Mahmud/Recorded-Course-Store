using Autofac;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RCS.Web.Areas.Admin.Models
{
    public class RoleCreateModel
    {
        [Required]
        public string Name { get; set; }

        private RoleManager<IdentityRole> _roleManager;
        private UserManager<IdentityUser> _userManager;

        public RoleCreateModel()
        {

        }

        public RoleCreateModel(RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        internal void ResolveDependency(ILifetimeScope scope)
        {
            _roleManager = scope.Resolve<RoleManager<IdentityRole>>();
            _userManager = scope.Resolve<UserManager<IdentityUser>>();
        }

        internal async Task CreateRole()
        {
            if (!string.IsNullOrWhiteSpace(Name))
            {
               // await _roleManager.CreateAsync(new ApplicationRole(Name));
            }
        }
    }
}
