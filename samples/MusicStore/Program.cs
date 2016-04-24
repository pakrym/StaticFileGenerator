// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace MusicStore
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseDefaultHostingConfiguration(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseStaticFileGenerator(o =>
                {
                    o.RootPages.Add(new StaticFileGeneratorPage("/", "/Index.html"));
                    o.OutputPath = "d:/temp/ms";
                })
                .Build();

            host.Run();
        }
    }
}
