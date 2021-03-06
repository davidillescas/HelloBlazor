using System;
using System.Globalization;
using System.Threading.Tasks;

using HelloBlazor.Common.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace HelloBlazor.App
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            #region OAuth + OIDC
            builder.Services.AddOidcAuthentication(options =>
            {
                //builder.Configuration.Bind("Identity", options.ProviderOptions);
            });
            #endregion

            #region Typed HttpClient
            builder.Services.AddHttpClient<IWeatherService, HttpWeatherService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            //.AddHttpMessageHandler(sp =>
            //{
            //    var handler = sp.GetService<AuthorizationMessageHandler>()
            //        .ConfigureHandler(
            //            authorizedUrls: new[] { "https://localhost:5001" },
            //            scopes: new[] { "HelloBlazor.Server.Read" });
            //    return handler;
            //});
            #endregion

            #region Localization
            //builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            #endregion

            #region Javascript storage helper
            var host = builder.Build();
            var jsInterop = host.Services.GetRequiredService<IJSRuntime>();
            var result = await jsInterop.InvokeAsync<string>("blazorCulture.get");
            if (result != null)
            {
                var culture = new CultureInfo(result);
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
            }

            await host.RunAsync();
            #endregion

            #region A failed experiment
            builder.Services.AddHttpClient<INotificationService, NotificationService>(client =>
                {
                    client.BaseAddress = new Uri("https://localhost:5001/");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                }).AddHttpMessageHandler(sp =>
                  {
                      var handler = sp.GetService<AuthorizationMessageHandler>()
                          .ConfigureHandler(
                              authorizedUrls: new[] { "https://localhost:5001" },
                              scopes: new[] { "HelloBlazor.Server.Read" });
                      return handler;
                  }); 
            #endregion

            await builder.Build().RunAsync();
        }
    }
}
