using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Xunit;

namespace Microsoft.Maui.DeviceTests
{
	[Category(TestCategory.BlazorWebView)]
	public partial class BlazorWebViewTests : HandlerTestBase
	{
		[Fact]
		public void BlazorFailing()
		{
			var bwv = new BlazorWebView()
			{
				HostPage = "value",
			};

			Assert.Equal("value2", bwv.HostPage);
		}

		[Fact]
		public void BlazorPassing()
		{
			var bwv = new BlazorWebView()
			{
				HostPage = "value",
			};

			Assert.Equal("value", bwv.HostPage);
		}
	}
}
