

namespace PrimeEwsi
{
    using System;
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    using SimpleInjector;
    using SimpleInjector.Integration.Web;
    using SimpleInjector.Integration.Web.Mvc;
    using Infrastructure.Jira;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var container = new Container();

            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());

            container.Register<PrimeEwsiContext>(() => new PrimeEwsiContext(), Lifestyle.Scoped);

            container.Register<IPrimeEwsiDbApi, PrimeEwsiDbApi>(Lifestyle.Scoped);

            container.Register<IJiraApi, JiraApi>(Lifestyle.Scoped);

            container.Register<IPackApi, PackApi>(Lifestyle.Scoped);

            container.RegisterMvcIntegratedFilterProvider();
#if DEBUG
            container.Verify();
#endif
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

          

            AppDomain.CurrentDomain.SetData("DataDirectory", HttpContext.Current.Server.MapPath("~"));

        }
    }
}
