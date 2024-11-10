using System.Diagnostics;
using anqtnbot.command;
using anqtnbot.command.standard;
using anqtnbot.config;
using anqtnbot.database;
using anqtnbot.database.mysql;
using anqtnbot.handler;
using anqtnbot.handler.sub;
using anqtnbot.locale;
using anqtnbot.utils;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace anqtnbot;

public static class Program
{
	public static async Task Main()
	{
		var stopwatch = new Stopwatch();
		
		stopwatch.Start();
		
		var serviceProvider = ConfigureServices();
		await using var database = serviceProvider.GetRequiredService<IDatabase>();

		await database.InitAsync();
		RegisterCommands(serviceProvider);

		StartReceiving(serviceProvider);
		
		stopwatch.Stop();
		Console.WriteLine($@"Started in {stopwatch.Elapsed.TotalSeconds:F3} seconds");
		
		await Task.Delay(Timeout.Infinite);
	}

	private static ServiceProvider ConfigureServices()
	{
		var services = new ServiceCollection();

		services.AddSingleton<ConfigurationService>();
		services.AddSingleton<IConfig>(serviceProvider =>
		{
			var config = serviceProvider.GetRequiredService<ConfigurationService>().Configuration;

			return new Config
			{
				ConnectionString = config["ConnectionString"],
				Token = config["Token"],
				EncryptionKey = config["EncryptionKey"]
			};
		});

		services.AddSingleton<ITelegramBotClient>(serviceProvider =>
		{
			var config = serviceProvider.GetRequiredService<IConfig>();

			return new TelegramBotClient(config.Token);
		});

		services.AddSingleton<IDatabase, MySqlDatabase>();
		services.AddSingleton<IQueries, MySqlQueries>();
		services.AddSingleton<IProvider, MySqlProvider>();

		services.AddTransient<UpdateHandler>();
		services.AddTransient<PollingErrorHandler>();
		services.AddTransient<CommandHandler>();

		services.AddTransient<MessageHandler>();
		services.AddTransient<CallbackQueryHandler>();

		services.AddSingleton<ICommandRegistry, CommandRegistry>();

		services.AddTransient<StartCommand>();
		services.AddTransient<ChangeLanguageCommand>();

		services.AddTransient<Encryption>();
		services.AddTransient<Locale>();

		return services.BuildServiceProvider();
	}

	private static void StartReceiving(IServiceProvider serviceProvider)
	{
		var botClient = serviceProvider.GetRequiredService<ITelegramBotClient>();
		var updateHandler = serviceProvider.GetRequiredService<UpdateHandler>();
		var pollingErrorHandler = serviceProvider.GetRequiredService<PollingErrorHandler>();

		botClient.StartReceiving(
			
			updateHandler.HandleUpdate,
			pollingErrorHandler.HandlePollingError,
			new ReceiverOptions
			{
				AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery]
			},
			new CancellationTokenSource().Token
		);
	}

	private static void RegisterCommands(IServiceProvider serviceProvider)
	{
		var commandRegistry = serviceProvider.GetRequiredService<ICommandRegistry>();

		commandRegistry.RegisterCommand(serviceProvider.GetRequiredService<StartCommand>());
		commandRegistry.RegisterCommand(serviceProvider.GetRequiredService<ChangeLanguageCommand>());
	}
}
