using Autofac;
using RCS.UI.Areas.Admin.Models;

namespace RCS.UI
{
    public class WebModule : Module
    {


        public WebModule()
        {
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CourseCreateModel>().AsSelf();
            base.Load(builder);
        }
    }
}
