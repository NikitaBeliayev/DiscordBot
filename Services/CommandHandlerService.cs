using System.Diagnostics;
using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Services;

public class CommandHandlerService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commandService;
    public CommandHandlerService(IServiceProvider serviceProvider)
    {
        this._client = serviceProvider.GetRequiredService<DiscordSocketClient>();
        this._commandService = serviceProvider.GetRequiredService<CommandService>();
        this._serviceProvider = serviceProvider;
    }

    public async Task InstallCommandsAsync()
    {
        _client.MessageReceived += HandleCommandAsync;
        await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
    }

    public async Task HandleCommandAsync(SocketMessage socketMessage)
    {
        
        SocketUserMessage? message = socketMessage as SocketUserMessage;
        if (message is null) return;

        int index = 0;

        if (!message.HasCharPrefix('!', ref index) ||
            message.HasMentionPrefix(_client.CurrentUser, ref index) ||
            message.Author.IsBot) return;

        SocketCommandContext commandContext = new SocketCommandContext(_client, message);
        
        await _commandService.ExecuteAsync(commandContext, index, _serviceProvider);
    }
}