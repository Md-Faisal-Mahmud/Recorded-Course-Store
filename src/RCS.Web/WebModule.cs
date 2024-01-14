using Autofac;
using RCS.Web.Areas.Admin.Models;

namespace RCS.Web
{
    public class WebModule : Module
    {
        public WebModule()
        { }

        protected override void Load(ContainerBuilder builder)
        {
           
            builder.RegisterType<CourseCreateModel>().AsSelf();
            base.Load(builder);
        }
    }
}
