// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Globalization;
using Yextly.Telemetry.Abstractions;

namespace Yextly.Telemetry.Azure
{
    /// <summary>
    /// Represents a an Application Insights client wrapper.
    /// </summary>
    /// <remarks>This API supports the telemetry infrastructure and is not intended to be used directly from your code.</remarks>
    public sealed class AiTelemetryClient : ITelemetryClient
    {
        private readonly TelemetryClient _telemetryClient;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="telemetryClient"></param>
        /// <param name="initializers"></param>
        /// <param name="serviceProvider"></param>
        public AiTelemetryClient(TelemetryClient telemetryClient, IAiTelemetryClientInitializers initializers, IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(telemetryClient);
            ArgumentNullException.ThrowIfNull(initializers);
            ArgumentNullException.ThrowIfNull(serviceProvider);

            _telemetryClient = telemetryClient;
            InnerClient = telemetryClient;

            foreach (var item in initializers.Initializers)
            {
                if (ActivatorUtilities.CreateInstance(serviceProvider, item) is IAiTelemetryClientInitializer initializer)
                {
                    initializer.Initialize(telemetryClient);
                }
            }
        }

        internal TelemetryClient InnerClient { get; }

        /// <inheritdoc/>
        public ITelemetryPropertyBag CreatePropertyBag()
        {
            return new AiPropertyBag();
        }

        /// <inheritdoc />
        public async Task<bool> FlushAsync(CancellationToken cancellationToken = default)
        {
            return await _telemetryClient.FlushAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public void TrackEvent(string name, ITelemetryPropertyBag? tags = null)
        {
            var @event = new EventTelemetry(name);

            if (tags is AiPropertyBag bag)
            {
                foreach (var item in bag.Data)
                {
                    @event.Properties.Add(item);
                }
            }

            _telemetryClient.TrackEvent(@event);
        }

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "We can't throw here")]
        public void TrackException(Exception exception, ITelemetryPropertyBag? tags = null)
        {
            ArgumentNullException.ThrowIfNull(exception);

            Dictionary<string, string>? data = null;

            if (tags is AiPropertyBag bag)
            {
                data = bag.Data;
            }

            if (exception.Data.Count > 0)
            {
                data ??= new(StringComparer.Ordinal);
                foreach (DictionaryEntry item in exception.Data)
                {
                    // The purpose of this code is to try to convert the data contained in the exception to string.
                    // If we can't, we just skip the entry. The user can always pass additional metadata to amend this situation.

                    try
                    {
                        var key = item.Key.ToString();
                        var value = Convert.ToString(item.Value, CultureInfo.InvariantCulture) ?? string.Empty;

                        if (key != null)
                        {
                            data.TryAdd(key, value);
                        }
                    }
                    catch
                    {
                        // We could crash here when we cannot properly convert the value to a string. This could be due to many factors, the complex nature of
                        // of the object, or simply because there is no built-in conversion.
                        // Since then the purpose of the telemetry is to aid the user and limit the developer efforts we just ignore it.
                    }
                }
            }

            _telemetryClient.TrackException(exception, data);
        }

        /// <inheritdoc />
        public ITelemetryOperation TrackOperation(string operationName, string type)
        {
            var operation = _telemetryClient.StartOperation<DependencyTelemetry>(operationName);
            operation.Telemetry.Type = type;

            return new AiOperation(this, operation);
        }

        /// <inheritdoc />
        public ITelemetryOperation TrackOperation(string operationName, string type, string operationId, string? parentOperationid = null)
        {
            var operation = _telemetryClient.StartOperation<DependencyTelemetry>(operationName, operationId, parentOperationid);
            operation.Telemetry.Type = type;

            return new AiOperation(this, operation);
        }
    }
}
