// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the Apache 2.0 License
// See the LICENSE file in the project root for more information.
// Maintainer: Argo Zhang(argo@live.ca) Website: https://www.blazor.zone

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BootstrapBlazor.Components;

class AutoClearTableDynamicObjectTypeCacheTask(ILogger<AutoClearTableDynamicObjectTypeCacheTask> logger) : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (stoppingToken is { IsCancellationRequested: false })
        {
            try
            {
                ClearCache();
                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AutoClearTableDynamicObjectTypeCacheTask background service");
            }
        }
    }

    private void ClearCache()
    {
        var type = Type.GetType("Microsoft.AspNetCore.Components.Reflection.ComponentProperties, Microsoft.AspNetCore.Components");
    }
}
