// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.Extensions.Http;

namespace Yextly.Testing.Mocks.Http
{
    internal sealed class HttpMessageHandlerBuilderMock : HttpMessageHandlerBuilder
    {
        private readonly HttpMessageHandler _handler;

        private readonly string _name;

        public HttpMessageHandlerBuilderMock(string name, HttpMessageHandler handler, IServiceProvider serviceProvider) : base()
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNull(handler);
            ArgumentNullException.ThrowIfNull(serviceProvider);

            _name = name;
            _handler = handler;
            Services = serviceProvider;
        }

        public override IList<DelegatingHandler> AdditionalHandlers { get; } = new List<DelegatingHandler>();

        public override string? Name { get => _name; set => throw new NotSupportedException(); }
        public override HttpMessageHandler PrimaryHandler { get => _handler; set => throw new NotSupportedException(); }

        public override IServiceProvider Services { get; }

        public override HttpMessageHandler Build()
        {
            return CreateHandlerPipeline(_handler, AdditionalHandlers);
        }
    }
}
