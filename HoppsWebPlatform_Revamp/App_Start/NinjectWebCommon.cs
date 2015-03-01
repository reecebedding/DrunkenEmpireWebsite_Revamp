using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(HoppsWebPlatform_Revamp.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(HoppsWebPlatform_Revamp.App_Start.NinjectWebCommon), "Stop")]



namespace HoppsWebPlatform_Revamp.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using HoppsWebPlatform_Revamp.DataAccess.Interfaces;
    using HoppsWebPlatform_Revamp.DataAccess;
    using HoppsWebPlatform_Revamp.Models;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IAltRepository>().To<AltRepository>();
            kernel.Bind<IApiRepository>().To<APIRepository>();
            kernel.Bind<IAppSettingsRepository>().To<AppSettingsRepository>();
            kernel.Bind<ICorpMemberRepository>().To<CorpMemberRepository>();
            kernel.Bind<IEveDBRepository>().To<EveDBRepository>();
            kernel.Bind<ILogRepository>().To<LogRepository>();
            kernel.Bind<IRecruitmentRepository>().To<RecruitmentRepository>();
            kernel.Bind<ILotteryRepository>().To<LotteryRepository>();
            kernel.Bind<IContractRepository>().To<ContractRepository>();

            kernel.Bind<ApplicationUserManager>().To<ApplicationUserManager>();
            kernel.Bind<IUserStore<ApplicationUser>>().ToMethod(x => new UserStore<ApplicationUser>(new ApplicationDbContext()));
        }        
    }
}


