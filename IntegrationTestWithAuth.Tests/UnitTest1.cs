using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTestWithAuth.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var factory = new MyAppFactory();
            var client = factory.CreateClient();
            var authResponse = await client.GetAsync("test/getauth");
            Assert.Equal(HttpStatusCode.OK, authResponse.StatusCode);

            var response = await client.GetAsync("api/values");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }


    public class MyAppFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder webHostBuilder)
        {
            base.ConfigureWebHost(webHostBuilder);
            webHostBuilder.ConfigureAppConfiguration((context, builder) =>
            {
                context.Configuration[Startup.UnitTestKey] = "true";
                Dictionary<string, string> config = new Dictionary<string, string>()
                {
                    { "Logging:LogLevel:Default", "Error"},
                    { "Logging:LogLevel:System", "Error"},
                    { "Logging:LogLevel:Microsoft", "Error"},
                };
                var memoryConfigurationSource = new MemoryConfigurationSource { InitialData = config };
                builder.Add(memoryConfigurationSource);
            });

            webHostBuilder.ConfigureServices((context, services) =>
            {
               services.AddMvc()
                   .PartManager.ApplicationParts.Add(new AssemblyPart(GetType().Assembly));
                services.AddAuthentication(o =>
                        {
                            o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                            o.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        })
                    .AddCookie(o =>
                    {
                        o.Events.OnRedirectToLogin = (c) =>
                    {
                        c.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    };
                    });
            });
        }
    }
}
