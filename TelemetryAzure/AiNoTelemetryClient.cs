// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Yextly.Telemetry.Abstractions;

namespace Yextly.Telemetry.Azure
{
    /// <summary>
    /// Provides an implementation that does nothing.
    /// </summary>
    public sealed class AiNoTelemetryClient : ITelemetryClient
    {
        /// <inheritdoc/>
        public ITelemetryPropertyBag CreatePropertyBag()
        {
            return new PropertyBag();
        }

        /// <inheritdoc/>
        public Task<bool> FlushAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc/>
        public void TrackEvent(string name, ITelemetryPropertyBag? tags = null)
        {
            // no-op on purpose
        }

        /// <inheritdoc/>
        public void TrackException(Exception exception, ITelemetryPropertyBag? tags = null)
        {
            // no-op on purpose
        }

        /// <inheritdoc/>
        public ITelemetryOperation TrackOperation(string operationName, string type)
        {
            return new NullOperation();
        }

        /// <inheritdoc/>
        public ITelemetryOperation TrackOperation(string operationName, string type, string operationId, string? parentOperationid = null)
        {
            return new NullOperation();
        }

        private sealed class NullOperation : ITelemetryOperation
        {
            public string Id => string.Empty;

            public void AddBaggage(string name, string? value)
            {
                // no-op on purpose
            }

            public void AddTag(string name, string value)
            {
                // no-op on purpose
            }

            public ITelemetryPropertyBag CreatePropertyBag()
            {
                return new PropertyBag();
            }

            public void Dispose()
            {
                // no-op on purpose
            }

            public void TrackEvent(string name, ITelemetryPropertyBag? tags = null)
            {
                // no-op on purpose
            }

            public void TrackException(Exception exception, ITelemetryPropertyBag? tags = null)
            {
                // no-op on purpose
            }
        }

        private sealed class PropertyBag : ITelemetryPropertyBag
        {
            public Dictionary<string, string> Data { get; } = new(StringComparer.Ordinal);

            public void Add(string name, string value)
            {
                ArgumentNullException.ThrowIfNull(name);

                Data.Add(name, value ?? string.Empty);
            }
        }
    }
}
