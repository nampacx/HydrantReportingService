
using HydrantReportingService.Services.BingMaps;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
[assembly: FunctionsStartup(typeof(HydrantReportingService.Functions.Startup))]
namespace HydrantReportingService.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            builder.ConfigurationBuilder.AddEnvironmentVariables();
            builder.ConfigurationBuilder.AddUserSecrets<Startup>(true);
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddTransient<BingMapClient>();
        }
    }
}
