# Titanosoft.EfConfiguration

### What is EfConfiguration?

EfConfiguration is a dotnet Standard 2.0 library for pulling configuration settings from a database. With support for auto reloading of settings.

### How do I get started?

First install the nuget package:

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

If you need to change the connection string name or how often to check for changes, you can specify it in the options:
```csharp
    var builder = new ConfigurationBuilder()
        ...
        .AddEntityFameworkValues(options => {
            options.ConnectionStringName = "DefaultConnection";
            options.PollingInterval = 1000;
        });
```

### How do I use the settings?

These settings are used like any other setting in dotnet core. First you can inject ```IConfgiuration``` and access items by key. Or you can use ```IOptions```. What sets this library appart from others is that it supports ```IOptionsSnapshot``` so your application can have its settings changed at runtime. You can read more about these configuration options here.

[Configuration in ASP.Net Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration)

### What if I don't want to give full db access to the user?

Don't worry, you don't have to. You can just give full schema access. Use the following sql script to create the user for the connection given to the configuration connection string. This will isolate your other schemas from being affected by the library:

```sql
CREATE USER [AspConfiguration] WITH PASSWORD = 'strong_password_for_config'
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE [name] = 'cfg') EXEC ('CREATE SCHEMA [cfg]')
GO

ALTER AUTHORIZATION ON SCHEMA::[cfg] TO [AspConfiguration]
GO

GRANT CREATE TABLE TO [AspConfiguration]
GO
```

### What if I don't want to automigrate at all?

You also don't have to do this, but it is a little more work. Inside of the nuget package is a "tools" folder. In that folder is a "Migrations.sql" file. If you retreive that file and run it as part of your deployment, your database will always be up to date. Note: the user running the deployment script will have to have the above access.
