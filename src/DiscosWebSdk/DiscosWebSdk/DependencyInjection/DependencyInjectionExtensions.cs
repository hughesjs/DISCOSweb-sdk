using DiscosWebSdk.Clients;
using DiscosWebSdk.Exceptions.DependencyInjection;
using DiscosWebSdk.Interfaces.BulkFetching;
using DiscosWebSdk.Interfaces.Clients;
using DiscosWebSdk.Interfaces.Queries;
using DiscosWebSdk.Options;
using DiscosWebSdk.Queries.Builders;
using DiscosWebSdk.Services.BulkFetching;
using DiscosWebSdk.Services.Queries;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscosWebSdk.DependencyInjection;

public static class DependencyInjectionExtensions
{
	public static IServiceCollection AddDiscosServices(this IServiceCollection services, IConfiguration configuration, bool usePolly = false, bool logToConsole = false) 
		=> services.RegisterEverything(configuration, usePolly, logToConsole);

	[UsedImplicitly]
	public static IServiceCollection AddDiscosServices(this IServiceCollection services, string apiUrl, string apiKey, bool usePolly = false, bool logToConsole = false)
		=> services.RegisterEverything(BuildConfiguration(apiUrl, apiKey), usePolly, logToConsole);

	[UsedImplicitly]
	private static IServiceCollection RegisterEverything(this IServiceCollection services, IConfiguration configuration, bool usePolly = false, bool logToConsole = false)
	{
		IConfigurationSection configSection = configuration.GetSection(nameof(DiscosOptions));
		DiscosOptions opt = configSection.Get<DiscosOptions>() ?? throw new InvalidDiscosConfigurationException("DISCOS configuration not found");

		if (opt.DiscosApiKey is null || opt.DiscosApiUrl is null)
		{
			throw new InvalidDiscosConfigurationException("DISCOS API key and URL not set");
		}

		services.Configure<DiscosOptions>(configSection);

		services.AddTransient(typeof(IDiscosQueryBuilder<>), typeof(DiscosQueryBuilder<>));
		services.AddTransient(typeof(IBulkFetchService<>), typeof(ImmediateBulkFetchService<>));

		services.AddTransient<IDiscosQueryBuilder, DiscosQueryBuilder>();
		services.AddTransient<IBulkFetchService, ImmediateBulkFetchService>();

		services.AddTransient<IQueryVerificationService, QueryErrataVerificationService>();

		services.AddHttpClient<IDiscosClient, DiscosClient>(c =>
		{
			c.BaseAddress = new(opt.DiscosApiUrl);
			c.DefaultRequestHeaders.Authorization = new("bearer", opt.DiscosApiKey);
		});
		

		return services;
	}

	private static IConfiguration BuildConfiguration(string apiUrl, string apiKey)
	{
		if (string.IsNullOrWhiteSpace(apiUrl) || string.IsNullOrWhiteSpace(apiKey))
		{
			throw new InvalidDiscosConfigurationException("DISCOS API key and URL not set");
		}

		IConfigurationBuilder builder = new ConfigurationBuilder()
			.AddInMemoryCollection(new Dictionary<string, string>
			{
				{ "DiscosOptions:DiscosApiKey", apiKey },
				{ "DiscosOptions:DiscosApiUrl", apiUrl }
			});

		return builder.Build();
	}
}