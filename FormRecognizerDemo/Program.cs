using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(builder =>
    {
        builder.AddConfiguration(GetConfigurationBuilder());
    })
    .ConfigureServices(s =>
    {
        s.AddOptions<FormRecognizerOptions>()
        .Configure<IConfiguration>((settings, configuration) =>
        {
            configuration.GetSection(FormRecognizerOptions.GetName()).Bind(settings);
        });
    })
    .Build();

host.Run();

static IConfiguration GetConfigurationBuilder() =>
    new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddUserSecrets<Program>()
        .AddEnvironmentVariables()
        .Build();
