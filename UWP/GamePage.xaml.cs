// Copyright (c) Timofei Zhakov. All rights reserved.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MrBoom
{
    public sealed partial class GamePage : Page
    {
        readonly Game _game;

        public GamePage()
        {
            this.InitializeComponent();

            // Create the game.
            var launchArguments = string.Empty;
            _game = MonoGame.Framework.XamlGame<Game>.Create(launchArguments, Window.Current.CoreWindow, swapChainPanel);
        }
    }
}
