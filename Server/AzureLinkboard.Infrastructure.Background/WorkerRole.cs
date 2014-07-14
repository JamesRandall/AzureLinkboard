using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using AccidentalFish.ApplicationSupport.Core.Components;
using AccidentalFish.ApplicationSupport.Core.Email.Providers;
using AccidentalFish.ApplicationSupport.Core.Logging;
using AccidentalFish.ApplicationSupport.Core.Runtime;
using AzureLinkboard.Domain;
using Microsoft.Practices.Unity;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

namespace AzureLinkboard.Infrastructure.Background
{
    public class WorkerRole : RoleEntryPoint
    {
        private IUnityContainer _container;
        private ILogger _logger;

        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("AzureLinkboard.Infrastructure.Background entry point called");
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            _logger.Information("Starting diagnostic role");

            IComponentHost componentHost = _container.Resolve<IComponentHost>();
            componentHost.Start(new StaticComponentHostConfigurationProvider(new List<ComponentConfiguration>
            {
                new ComponentConfiguration
                {
                    ComponentIdentity = ComponentIdentities.PostedUrlProcessor,
                    Instances = 1,
                    RestartEvaluator = (ex, retryCount) => retryCount < 5
                }
            }), cancellationTokenSource);

            while (true)
            {
                Thread.Sleep(10000);
                Trace.TraceInformation("Working");
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            _container = new UnityContainer();
            AccidentalFish.ApplicationSupport.Core.Bootstrapper.RegisterDependencies(_container);
            AccidentalFish.ApplicationSupport.Azure.Bootstrapper.RegisterDependencies(_container);
            Domain.Bootstrapper.RegisterDependencies(_container);
            Domain.Bootstrapper.RegisterInfrastructure(_container);
            AccidentalFish.ApplicationSupport.Processes.Bootstrapper.RegisterDependencies(_container);

            ILoggerFactory loggerFactory = _container.Resolve<ILoggerFactory>();
            _logger = loggerFactory.CreateLongLivedLogger(new LoggerSource("com.accidentalfish.azurelinkboard.background.worker-role"));
            _logger.Information("Starting background worker role instance");

            return base.OnStart();
        }
    }
}
