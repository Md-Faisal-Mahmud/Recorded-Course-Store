using Autofac;

namespace RCS.Web.Models
{
    public abstract class BaseModel
    {
        protected ILifetimeScope _scope;
        protected IHttpContextAccessor _httpContextAccessor;

        public BaseModel()
        {

        }

        public virtual void ResolveDependency(ILifetimeScope scope)
        {
            _scope = scope;
            _httpContextAccessor = _scope.Resolve<IHttpContextAccessor>();
        }
    }
}
