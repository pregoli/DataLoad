using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using PaoloTestMS.Services;
using PaoloTestMS.Services.Interfaces;

namespace PaoloTestMS.App_Start
{
    public class IocConfig
    {
        public static void RegisterDependencies()
        {
            #region Create the builder
            var builder = new ContainerBuilder();
            #endregion
            
            #region register services
            builder.RegisterType<FilesService>().As<IFilesService>();
            builder.RegisterType<DownloadService>().As<IDownloadService>();
            #endregion

            #region Register all controllers for the assembly

            builder.RegisterControllers(typeof(MvcApplication).Assembly)
                   .InstancePerRequest();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            #endregion

            #region Register modules
            builder.RegisterAssemblyModules(typeof(MvcApplication).Assembly);
            #endregion
            
            #region Set the api dependency resolver to use Autofac
            var container = builder.Build();
            
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            #endregion
        }
    }
}