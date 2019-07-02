// ------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.
// ------------------------------------------------------------------------------

namespace Microsoft.Graph.Core.Test.Mocks
{
	using System;
	using System.IO;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Threading;
	using System.Threading.Tasks;

	using Moq;

	public class MockHttpProvider : Mock<IHttpProvider>
	{
		public string ContentAsString { get; private set; }
		public HttpContentHeaders ContentHeaders { get; private set; }

		public MockHttpProvider(HttpResponseMessage httpResponseMessage, ISerializer serializer = null)
				: base(MockBehavior.Loose)
		{
			//this.Setup(provider => provider.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<HttpCompletionOption>(), It.IsAny<CancellationToken>()))
			//		.Returns(Task.FromResult(httpResponseMessage));

			this.Setup(provider => provider.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<HttpCompletionOption>(), It.IsAny<CancellationToken>()))
				.Callback<HttpRequestMessage, HttpCompletionOption, CancellationToken>((req, opt, tok) => this.ReadRequestContent(req))
				.ReturnsAsync(httpResponseMessage);


			this.SetupGet(provider => provider.Serializer).Returns(serializer);
		}

		private void ReadRequestContent(HttpRequestMessage req)
		{
			if (req.Content != null)
			{
				this.ContentHeaders = req.Content.Headers;
				this.ContentAsString = req.Content.ReadAsStringAsync().GetAwaiter().GetResult();
			}
		}
	}
}