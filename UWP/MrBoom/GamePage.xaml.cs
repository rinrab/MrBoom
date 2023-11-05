// Copyright (c) Timofei Zhakov. All rights reserved.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MrBoom
{
    public sealed partial class GamePage : Page
    {
        public GamePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string launchArguments = string.Empty;

            if (e.Parameter != null && e.Parameter is string commandParam)
            {
                launchArguments = commandParam;
            }

            MonoGame.Framework.XamlGame<Game>.Create(launchArguments, Window.Current.CoreWindow, swapChainPanel);

            base.OnNavigatedTo(e);
        }
    }
}
