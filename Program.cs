using System.Net.Mime;
using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot;

public class Program
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly DiscordSocketClient _client;
    private readonly DiscordSocketConfig _config;
    private readonly CommandHandlerService _commandHandlerService;
    private Program()
    {
        string startupPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        this._configuration = new ConfigurationManager()
            .SetBasePath(
                Path.Combine(
                    startupPath, 
                    "Config"))
            .AddJsonFile("config.json")
            .Build();
        
        this._config = new DiscordSocketConfig()
        {
            AlwaysDownloadUsers = false,
            GatewayIntents = GatewayIntents.All
        };
        
        this._serviceProvider = ConfigureServices();

        _client = new DiscordSocketClient(_config);

        _commandHandlerService = _serviceProvider.GetRequiredService<CommandHandlerService>();
    }

    public static void Main(string[] argc) => new Program().MainAsync(argc).GetAwaiter().GetResult();
    private async Task MainAsync(string[] argc)
    {
        _client.Log += Log;
        await _client.LoginAsync(TokenType.Bot, _configuration["token"]);
        await _client.StartAsync();
        await _commandHandlerService.InstallCommandsAsync();

        await Task.Delay(-1);
    }

    private IServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection()
            .AddSingleton(_config)
            .AddSingleton<CommandService>()
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<CommandHandlerService>();
        return serviceCollection.BuildServiceProvider();
    }


    private Task Log(LogMessage message)
    {
        Console.WriteLine(message.Message);
        return Task.CompletedTask;
    }
}