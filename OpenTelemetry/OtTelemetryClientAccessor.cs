// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.Extensions.DependencyInjection;
using Yextly.Telemetry.Abstractions;

namespace Yextly.OpenTelemetry
{
    internal sealed class OtTelemetryClientAccessor : ITelemetryClientAccessor
    {
        private readonly IServiceProvider _serviceProvider;

        public OtTelemetryClientAccessor(IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider);

            _serviceProvider = serviceProvider;
        }

        public ITelemetryClient<T> GetClient<T>() where T : ITelemetryActivitySourceProvider, new()
        {
            return _serviceProvider.GetRequiredService<ITelemetryClient<T>>();
        }
    }
}