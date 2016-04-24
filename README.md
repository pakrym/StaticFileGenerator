Generate static files site out of your MVC application.

## Usage

```
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
```