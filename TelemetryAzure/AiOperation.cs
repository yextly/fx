// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System.Diagnostics;
using Yextly.Telemetry.Abstractions;

namespace Yextly.Telemetry.Azure
{
    /// <summary>
    /// Represents a wrapped Application Insights operation.
    /// </summary>
    /// <remarks>This API supports the telemetry infrastructure and is not intended to be used directly from your code.</remarks>
    public sealed class AiOperation : ITelemetryOperation
    {
        private readonly IOperationHolder<DependencyTelemetry> _operation;
        private readonly AiCoreTelemetryClient _telemetryClient;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="telemetryClient"></param>
        /// <param name="operation"></param>
        public AiOperation(AiCoreTelemetryClient telemetryClient, IOperationHolder<DependencyTelemetry> operation)
        {
            _telemetryClient = telemetryClient;
            _operation = operation;
        }

        /// <inheritdoc />
        public string Id => _operation.Telemetry.Id;

        /// <inheritdoc />
        public void AddBaggage(string name, string? value)
        {
            Activity.Current?.AddBaggage(name, value);
        }

        /// <inheritdoc />
        public void AddTag(string name, string value)
        {
            _operation.Telemetry.Properties.Add(name, value);
        }

        /// <inheritdoc />
        public ITelemetryPropertyBag CreatePropertyBag()
        {
            return _telemetryClient.CreatePropertyBag();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _telemetryClient.InnerClient.StopOperation(_operation);
        }

        /// <inheritdoc />
        public void TrackEvent(string name, ITelemetryPropertyBag? tags = null)
        {
            _telemetryClient.TrackEvent(name, tags);
        }

        /// <inheritdoc />
        public void TrackException(Exception exception, ITelemetryPropertyBag? tags = null)
        {
            _telemetryClient.TrackException(exception, tags);
        }
    }
}
