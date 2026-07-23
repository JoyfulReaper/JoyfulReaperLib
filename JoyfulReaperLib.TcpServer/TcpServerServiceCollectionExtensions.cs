/*
 * JoyfulReaperLibrary
 * Copyright (c) 2026 Kyle Givler
 * Licensed under the MIT License.
 */

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace JoyfulReaperLib.TcpServer;

public static class TcpServerServiceCollectionExtensions
{
    /// <summary>
    /// Registers a protocol connection handler and its hosted TCP server.
    /// </summary>
    public static IServiceCollection AddTcpServer<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler, TOptions>(
            this IServiceCollection services)
        where THandler : class, ITcpConnectionHandler
        where TOptions : class, ITcpServerOptions
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddScoped<THandler>();
        services.AddHostedService<TcpServerHostedService<THandler, TOptions>>();

        return services;
    }
}