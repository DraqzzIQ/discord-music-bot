﻿using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DiscordMusicBot.Services;

internal sealed class DiscordClientHost : IHostedService
{
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly InteractionService _interactionService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DiscordClientHost> _logger;

    public DiscordClientHost(
        DiscordSocketClient discordSocketClient,
        InteractionService interactionService,
        IServiceProvider serviceProvider,
        ILogger<DiscordClientHost> logger)
    {
        ArgumentNullException.ThrowIfNull(discordSocketClient);
        ArgumentNullException.ThrowIfNull(interactionService);
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(logger);

        _discordSocketClient = discordSocketClient;
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _discordSocketClient.InteractionCreated += InteractionCreated;
        _discordSocketClient.Ready += ClientReady;
        _discordSocketClient.Log += LogAsync;

        await _discordSocketClient
            .LoginAsync(TokenType.Bot, ConfigService.BotToken)
            .ConfigureAwait(false);

        await _discordSocketClient
            .StartAsync()
            .ConfigureAwait(false);
    }

    private Task LogAsync(LogMessage arg)
    {
        if (arg.Exception is InteractionException interactionException)
        {
            _logger.LogError(interactionException,
                $"{interactionException.GetBaseException().GetType()} was thrown while executing {interactionException.CommandInfo} in Channel {interactionException.InteractionContext.Channel} on Server {interactionException.InteractionContext.Guild} by user {interactionException.InteractionContext.User}.");
            return Task.CompletedTask;
        }

        switch (arg.Severity)
        {
            case LogSeverity.Critical:
                _logger.LogCritical(arg.Exception, arg.Message);
                break;
            case LogSeverity.Error:
                _logger.LogError(arg.Exception, arg.Message);
                break;
            case LogSeverity.Warning:
                _logger.LogWarning(arg.Exception, arg.Message);
                break;
            case LogSeverity.Info:
                _logger.LogInformation(arg.Exception, arg.Message);
                break;
            case LogSeverity.Verbose:
                _logger.LogTrace(arg.Exception, arg.Message);
                break;
            case LogSeverity.Debug:
                _logger.LogDebug(arg.Exception, arg.Message);
                break;
        }

        return Task.CompletedTask;
    }


    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _discordSocketClient.InteractionCreated -= InteractionCreated;
        _discordSocketClient.Ready -= ClientReady;
        _discordSocketClient.Log -= LogAsync;

        await _discordSocketClient
            .StopAsync()
            .ConfigureAwait(false);
    }

    private Task<IResult> InteractionCreated(SocketInteraction interaction)
    {
        var interactionContext = new SocketInteractionContext(_discordSocketClient, interaction);
        return _interactionService.ExecuteCommandAsync(interactionContext, _serviceProvider);
    }

    private async Task ClientReady()
    {
        await _interactionService
            .AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider)
            .ConfigureAwait(false);

        // enable logging
        _interactionService.Log += LogAsync;
        
        // set activity
        await _discordSocketClient
            .SetActivityAsync(new Game("Music", ActivityType.Listening))
            .ConfigureAwait(false);

        // register commands to guild
#if DEBUG
        await _interactionService
            .RegisterCommandsToGuildAsync(ConfigService.DebugGuildId)
            .ConfigureAwait(false);
#else
        await _interactionService
            .RegisterCommandsGloballyAsync()
            .ConfigureAwait(false);
#endif
    }
}