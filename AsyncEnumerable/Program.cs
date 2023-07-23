using AsyncEnumerable;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<ConsoleRunner>();

        services.AddHttpClient<IPokemonClient, PokemonClient>(client =>
        {
            client.BaseAddress = new Uri("https://pokeapi.co/api/v2/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });
    })
    .UseConsoleLifetime()
    .Build()
    .RunAsync();