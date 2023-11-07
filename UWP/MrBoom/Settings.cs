// Copyright (c) Timofei Zhakov. All rights reserved.

using Windows.Storage;

namespace MrBoom
{
    public class Settings
    {
        private readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public TeamMode TeamMode
        {
            get
            {
                if (localSettings.Values.TryGetValue("teamMode", out var value))
                {
                    return (TeamMode)value;
                }
                else
                {
                    return TeamMode.Off;
                }
            }
            set
            {
                localSettings.Values["teamMode"] = (int)value;
            }
        }

        public string PlayerId
        {
            get
            {
                return (string)localSettings.Values[nameof(PlayerId)];
            }
            set
            {
                localSettings.Values[nameof(PlayerId)] = value;
            }
        }

        public bool IsDebug { get; set; }
    }

    public enum TeamMode
    {
        Off,
        Color,
        Sex
    }
}
