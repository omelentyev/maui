using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
#if WINDOWS
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
#endif
using WebViewAppShared;
using Xunit;

namespace Microsoft.Maui.DeviceTests
{
	[Category(TestCategory.BlazorWebView)]
	public partial class BlazorWebViewTests : HandlerTestBase
	{
		[Fact]
		public async Task BlazorPassing()
		{
			EnsureHandlerCreated(additionalCreationActions: appBuilder =>
			{
				appBuilder.Services.AddBlazorWebView();
			});

			var bwv = new BlazorWebViewWithCustomFiles
			{
				HostPage = "wwwroot/index.html",
				CustomFiles = new Dictionary<string, string>
				{
					{ "index.html", TestStaticFilesContents.DefaultMauiIndexHtmlContent },
				},
			};
			bwv.RootComponents.Add(new RootComponent { ComponentType = typeof(TestComponent1), Selector="#app", });

			await InvokeOnMainThreadAsync(async () =>
			{
				var bwvHandler = CreateHandler<BlazorWebViewHandler>(bwv);

				await Task.Delay(0);

				var nativeWebView = bwvHandler.NativeView;
#if WINDOWS

				await WaitForWebViewReady(nativeWebView);

				await WaitForControlDiv(bwvHandler.NativeView, controlValueToWaitFor: "0");

				var c1 = await nativeWebView.CoreWebView2.ExecuteScriptAsync(javaScript: "document.getElementById('incrementButton').click()");

				await WaitForControlDiv(bwvHandler.NativeView, controlValueToWaitFor: "1");

				var c2 = await nativeWebView.CoreWebView2.ExecuteScriptAsync(javaScript: "document.getElementById('incrementButton').click()");

				await WaitForControlDiv(bwvHandler.NativeView, controlValueToWaitFor: "2");

				var c3 = await nativeWebView.CoreWebView2.ExecuteScriptAsync(javaScript: "document.getElementById('incrementButton').click()");

				await WaitForControlDiv(bwvHandler.NativeView, controlValueToWaitFor: "3");

				var c4 = await nativeWebView.CoreWebView2.ExecuteScriptAsync(javaScript: "document.getElementById('incrementButton').click()");

				await WaitForControlDiv(bwvHandler.NativeView, controlValueToWaitFor: "4");
#endif
			});

		}

#if WINDOWS
		private async Task WaitForWebViewReady(WebView2 wv2)
		{
			const int MaxWaitTimes = 5;
			const int WaitTimeInMS = 100;

			CoreWebView2 coreWebView2 = null;
			for (int i = 0; i < MaxWaitTimes; i++)
			{
				coreWebView2 = wv2.CoreWebView2;
				if (coreWebView2 != null)
				{
					break;
				}
				await Task.Delay(WaitTimeInMS);
			}

			if (coreWebView2 == null)
			{
				throw new Exception($"Waited {MaxWaitTimes * WaitTimeInMS}ms but couldn't get CoreWebView2 to be available.");
			}

			var domLoaded = false;
			var sem = new SemaphoreSlim(1);
			await sem.WaitAsync();
			wv2.CoreWebView2.DOMContentLoaded += (s, e) =>
			{
				domLoaded = true;
				sem.Release();
			};

			await Task.WhenAny(Task.Delay(1000), sem.WaitAsync());

			if (!domLoaded)
			{
				throw new Exception($"Waited {MaxWaitTimes * WaitTimeInMS}ms but couldn't get CoreWebView2.DOMContentLoaded to complete.");
			}
			return;
		}

		private async Task WaitForControlDiv(WebView2 webView2, string controlValueToWaitFor)
		{
			const int MaxWaitTimes = 5;
			const int WaitTimeInMS = 100;
			var quotedExpectedValue = "\"" + controlValueToWaitFor + "\"";
			for (int i = 0; i < MaxWaitTimes; i++)
			{
				var controlValue = await webView2.CoreWebView2.ExecuteScriptAsync(javaScript: "document.getElementById('controlDiv').innerText");
				if (controlValue == quotedExpectedValue)
				{
					return;
				}
				await Task.Delay(WaitTimeInMS);
			}

			throw new Exception($"Waited {MaxWaitTimes * WaitTimeInMS}ms but couldn't get controlDiv to have value '{controlValueToWaitFor}'.");
		}
#endif

		public static class TestStaticFilesContents
		{
			public static readonly string DefaultMauiIndexHtmlContent = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no"" />
    <title>Blazor app</title>
    <base href=""/"" />
    <link href=""css/app.css"" rel=""stylesheet"" />
</head>

<body>
	This HTML is coming from a custom provider!
    <div id=""app""></div>

    <div id=""blazor-error-ui"">
        An unhandled error has occurred.
        <a href="""" class=""reload"">Reload</a>
        <a class=""dismiss"">🗙</a>
    </div>
    <script src=""_framework/blazor.webview.js"" autostart=""false""></script>

</body>

</html>
";
		}

		private sealed class BlazorWebViewWithCustomFiles : BlazorWebView
		{
			public Dictionary<string, string> CustomFiles { get; set; }

			public override IFileProvider CreateFileProvider(string contentRootDir)
			{
				if (CustomFiles == null)
				{
					return null;
				}
				var inMemoryFiles = new InMemoryStaticFileProvider(
					fileContentsMap: CustomFiles,
					// The contentRoot is ignored here because in WinForms it would include the absolute physical path to the app's content, which this provider doesn't care about
					contentRoot: null);
				return inMemoryFiles;
			}
		}
	}
}
