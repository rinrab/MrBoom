// Copyright (c) Timofei Zhakov. All rights reserved.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MrBoom
{
    public sealed partial class GamePage : Page
    {
        Game _game;

        public GamePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var launchArguments = string.Empty;

            if (e.Parameter != null && e.Parameter is string commandParam)
            {
                launchArguments = commandParam;
            }

            // Create the game.
            _game = MonoGame.Framework.XamlGame<Game>.Create(launchArguments, Window.Current.CoreWindow, swapChainPanel);

            base.OnNavigatedTo(e);
        }
    }
}
