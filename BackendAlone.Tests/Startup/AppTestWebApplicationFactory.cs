using System;
using System.Collections.Generic;
using System.Text;
using BackendAlone.Tests.ThirdPartyApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using ThirdPartyApi.Api;

namespace BackendAlone.Tests.Startup
{
    public class AppTestWebApplicationFactory<TThirdPartyApi> : WebApplicationFactory<BackendAlone.Startup>
    where TThirdPartyApi : IThirdPartyApi
    {
        public readonly TThirdPartyApi _thirdPartyApiInstance;
        public AppTestWebApplicationFactory(TThirdPartyApi thirdPartyApiInstance)
        {
            _thirdPartyApiInstance = thirdPartyApiInstance;
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureServices(services =>
            {
                services.AddScoped(typeof(IThirdPartyApi), x => _thirdPartyApiInstance);
            });
        }
    }
}
