// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.Extensions.Logging;
using System.Net;
using Yextly.Testing.Mocks.Http;

namespace Yextly.Testing.Mocks.Http
{
    internal sealed partial class HttpMessageHandlerMock : HttpMessageHandler
    {
        private readonly Chain _chain;
        private readonly Delays _delays;
        private readonly ILogger _logger;
        private readonly int _handlerId;
        private static int _sharedHandlerId;
        private int _requestId;

        public HttpMessageHandlerMock(ILogger logger, Delays delays, Chain chain)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(delays);
            ArgumentNullException.ThrowIfNull(chain);

            _handlerId = Interlocked.Increment(ref _sharedHandlerId);

            _logger = logger;
            _delays = delays;
            _chain = chain;
        }

        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var ret = new HttpResponseMessage();

            var delay = _delays.SyncReplyDelay;

            if (delay == TimeSpan.Zero)
            {
                Thread.Sleep(delay);
            }

            ResolveFlow(request, ret);

            return ret;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var delay = _delays.AsyncReplyDelay;

            if (delay == TimeSpan.Zero)
            {
                await Task.Yield();
            }
            else
            {
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            }

            var ret = new HttpResponseMessage();

            ResolveFlow(request, ret);

            return ret;
        }

        [LoggerMessage(LogLevel.Information, "[{HandlerId}-{RequestId}] {Method} {Uri} Accepted {ResposeStatusCode}")]
        static partial void LogAcceptedRequest(ILogger logger, int handlerId, int requestId, HttpMethod method, Uri? Uri, HttpStatusCode resposeStatusCode);

        [LoggerMessage(LogLevel.Information, "[{HandlerId}-{RequestId}] {Method} {Uri} REJECTED {Message}")]
        static partial void LogRejectedRequest(ILogger logger, int handlerId, int requestId, HttpMethod method, Uri? Uri, string message);

        private int GetNewRequestId() => Interlocked.Increment(ref _requestId);

        private void ResolveFlow(HttpRequestMessage request, HttpResponseMessage response)
        {
            ArgumentNullException.ThrowIfNull(request);

            var requestId = GetNewRequestId();

            var actualMethod = request.Method;
            var actualUri = request.RequestUri;

            var next = _chain.DequeueNext();

            if (next == null)
            {
                response.StatusCode = HttpStatusCode.BadGateway;
                response.ReasonPhrase = "Unmatched flow: no more data";

                LogRejectedRequest(_logger, _handlerId, requestId, actualMethod, actualUri, "no more data");
                return;
            }

            if (next.ExpectedMethod != actualMethod)
            {
                response.StatusCode = HttpStatusCode.BadGateway;
                response.ReasonPhrase = $"Unmatched flow: expected method {next.ExpectedMethod}, actual method {actualMethod}";

                LogRejectedRequest(_logger, _handlerId, requestId, actualMethod, actualUri, $"expected method {next.ExpectedMethod}");
                return;
            }

            if (next.ExpectedUri != actualUri)
            {
                response.StatusCode = HttpStatusCode.BadGateway;
                response.ReasonPhrase = $"Unmatched flow: expected uri {next.ExpectedUri}, actual uri {actualUri}";

                LogRejectedRequest(_logger, _handlerId, requestId, actualMethod, actualUri, $"expected uri {next.ExpectedUri}");
                return;
            }

            response.RequestMessage = request;
            if (next.Action == null)
            {
                response.StatusCode = next.StatusCode;
                response.Content = next.Content;
            }
            else
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                next.Action(request, response);
            }

            LogAcceptedRequest(_logger, _handlerId, requestId, actualMethod, actualUri, next.StatusCode);
        }
    }
}
