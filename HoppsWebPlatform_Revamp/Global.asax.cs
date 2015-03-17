using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Ninject.Web.Common;
using Ninject;
using System.Reflection;
using HoppsWebPlatform_Revamp.DataAccess.Interfaces;
using HoppsWebPlatform_Revamp.DataAccess;

namespace HoppsWebPlatform_Revamp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : NinjectHttpApplication  //System.Web.HttpApplication
    {

        private NLog.Logger _logger;

        public MvcApplication()
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        protected override IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            BindInterfaces(kernel);

            return kernel;
        }

        private void BindInterfaces(StandardKernel kernel)
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
        }

        protected override void OnApplicationStarted()
        {
            base.OnApplicationStarted();
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }

        protected void Application_EndRequest()
        {
            HttpContext.Current.Response.AppendHeader("User-Agent", "Contact: Natalie Cruella");
        }

        protected void Session_Start()
        {
            _logger.Info(string.Format("User visited website with User-Agent: {0}", Request.UserAgent));
        }

        //Not used due to Ninject
        //protected void Application_Start()
        //{
        //    AreaRegistration.RegisterAllAreas();

        //    WebApiConfig.Register(GlobalConfiguration.Configuration);
        //    FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        //    RouteConfig.RegisterRoutes(RouteTable.Routes);
        //    BundleConfig.RegisterBundles(BundleTable.Bundles);
        //    AuthConfig.RegisterAuth();
        //}

        
    }
}