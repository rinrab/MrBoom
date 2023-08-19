// Copyright (c) Timofei Zhakov. All rights reserved.

using System;

namespace MrBoom
{
    public interface IMenuItem
    {
        string Text { get; }

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
    }

    public class SelectMenuItem : IMenuItem
    {
        private readonly string header;
        private readonly string[] options;
        private readonly int maxCount;
        private string currentOption => options[SelectionIndex];

        public int SelectionIndex;
        public string Text => string.Format("{0}: {1}", header, currentOption);

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
    }
}
