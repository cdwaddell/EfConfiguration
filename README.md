# Titanosoft.EfConfiguration

### What is EfConfiguration?

EfConfiguration is a dotnet Standard 2.0 library for pulling configuration settings from a database. With support for auto reloading of settings.

### How do I get started?

First install the nuget package (coming soon)

```
PM> Install-Package Titanosoft.EfConfiguration
```

Then add your this as a step in setting up your configuration (in Startup.cs):

```csharp
public Startup(IHostingEnvironment env)
{
    CurrentEnvironment = env;

    var builder = new ConfigurationBuilder()
        .SetBasePath(env.ContentRootPath)
        .AddJsonFile("appsettings.json", false)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
        .AddEnvironmentVariables("CoreBlogger_")
        //One of your settings before this must contain your connection string 
        //DefaultConnection is used if none is specified
        .AddEntityFameworkValues();

    if (!env.IsProduction())
    {
        builder.AddApplicationInsightsSettings(true);
    }

    Configuration = builder.Build();
}
```

### What about advanced configuration?

If you need to change the connection string name, you can specify it in the options:
```csharp
    var builder = new ConfigurationBuilder()
        ...
        .AddEntityFameworkValues(options => {
            options.
        });
```
