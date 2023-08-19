// Copyright (c) Timofei Zhakov. All rights reserved.

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
        public int selectionIndex;

        public string Text => $"{header} {options[selectionIndex]}";

        public SelectMenuItem(string header, string[] options)
        {
            this.header = header;
            this.options = options;
        }

        public bool OnLeft()
        {
            selectionIndex--;
            if (selectionIndex < 0)
            {
                selectionIndex = options.Length - 1;
            }
            return true;
        }

        public bool OnRight()
        {
            selectionIndex++;
            if (selectionIndex >= options.Length)
            {
                selectionIndex = 0;
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
