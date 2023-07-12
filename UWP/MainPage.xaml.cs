using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            Init();
        }

        private async void Init()
        {
            await WebView2.EnsureCoreWebView2Async();

            WebView2.CoreWebView2.IsMuted = false;
            WebView2.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;

            WebView2.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "mrboom.app", "Assets/www",
                Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow);
            WebView2.Source = new Uri("https://mrboom.app/index.html");
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            float scale = (float)Math.Min(e.NewSize.Height / 400, e.NewSize.Width / 640);

            transform.ScaleX = scale;
            transform.ScaleY = scale;
        }
    }
}
