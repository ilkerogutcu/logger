using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Business.Abstract;
using Business.Concrete;
using Castle.DynamicProxy;
using Core.Utilities.Interceptors;
using DataAccess.Concrete.EntityFramework.Contexts;
using Module = Autofac.Module;

namespace Business.DependencyResolvers
{
    public class AutofacBusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AuthenticationManager>().As<IAuthenticationService>().SingleInstance();
            builder.RegisterType<EmployeeManager>().As<IEmployeeService>().SingleInstance();
            builder.RegisterType<ElasticSearchLogManager>().As<IElasticSearchLogService>().SingleInstance();
            builder.RegisterType<FileLogManager>().As<IFileLogService>().SingleInstance();
            builder.RegisterType<MongoDbLogManager>().As<IMongoDbLogService>().SingleInstance();

            builder.RegisterType<ApplicationDbContext>().InstancePerLifetimeScope();

            var assembly = Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces()
                .EnableInterfaceInterceptors(new ProxyGenerationOptions
                {
                    Selector = new AspectInterceptorSelector()
                }).SingleInstance();
        }
    }
}