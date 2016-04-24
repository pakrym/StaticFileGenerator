using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MusicStore
{
    public static class StaticFileGeneratorServerExtensions
    {
        public static IWebHostBuilder UseStaticFileGenerator(this IWebHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices(services =>
            {
                services.AddTransient<IConfigureOptions<StaticFileGeneratorOptions>, StaticFileGeneratorOptionsSetup>();
                services.AddSingleton<IServer, StaticFileGeneratorServer>();
                services.AddSingleton<IUrlHelperFactory, StaticFileGeneratorUrlHelperFactory>();
            });
        }

        public static IWebHostBuilder UseStaticFileGenerator(this IWebHostBuilder hostBuilder, Action<StaticFileGeneratorOptions> options)
        {
            return hostBuilder.UseStaticFileGenerator().ConfigureServices(services =>
            {
                services.Configure(options);
            });
        }
    }
}