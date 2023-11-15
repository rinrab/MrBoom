// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;

namespace MrBoom
{
    public interface IMenuItem
    {
        string Text { get; }
        string[] GetDynamicTexts();

        bool OnLeft();
        bool OnRight();
        bool OnEnter();
    }

    public class TextMenuItem : IMenuItem
    {
        public string Text { get; }

        public TextMenuItem(string text)
        {
            Text = text;
        }

        public bool OnLeft()
        {
            return false;
        }

        public bool OnRight()
        {
            return false;
        }

        public bool OnEnter()
        {
            return true;
        }

        public string[] GetDynamicTexts()
        {
            return new string[] { Text };
        }
    }

    public class SelectMenuItem : IMenuItem
    {
        private readonly string header;
        private readonly string[] options;
        private readonly int maxCount;
        private string CurrentOption => options[SelectionIndex];

        public int SelectionIndex;
        public string Text => FormatText(CurrentOption);

        public SelectMenuItem(string header, string[] options)
        {
            this.header = header;
            this.options = options;

            foreach (string option in options)
            {
                maxCount = Math.Max(option.Length, maxCount);
            }
        }

        public bool OnLeft()
        {
            SelectionIndex--;
            if (SelectionIndex < 0)
            {
                SelectionIndex = options.Length - 1;
            }
            return true;
        }

        public bool OnRight()
        {
            SelectionIndex++;
            if (SelectionIndex >= options.Length)
            {
                SelectionIndex = 0;
            }
            return true;
        }

        public bool OnEnter()
        {
            OnRight();
            return false;
        }

        public string[] GetDynamicTexts()
        {
            List<string> result = new List<string>();

            foreach (var option in options)
            {
                result.Add(FormatText(option));
            }

            return result.ToArray();
        }

        private string FormatText(string currentOption)
        {
            return string.Format("{0}: {1}", header, currentOption);
        }
    }
}
