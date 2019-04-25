using System;
using System.Windows.Media;

namespace Graph2D.ViewModels
{
    class ColoredObject
    {
        public object Object { get; }
        public Brush Foreground { get; }
        public Brush Background { get; }

        public ColoredObject(int value, int max)
        {
            Object = value;

            var b = (byte)Math.Min(value * 255 / max, 0xff);
            var f = (byte)~b;
            Background = new SolidColorBrush(Color.FromRgb(b, b, 0x00));
            Foreground = new SolidColorBrush(Color.FromRgb(f, f, f));
        }

        public override string ToString() => $"{Object},{Foreground},{Background}";

    }
}
