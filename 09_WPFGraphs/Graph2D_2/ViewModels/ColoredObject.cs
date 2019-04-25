using System;
using System.Windows.Media;

namespace Graph2D.ViewModels
{
    class ColoredObject
    {
        public object Object { get; }
        public Color Foreground { get;}
        public Color Background { get; }

        public ColoredObject(int value, int max)
        {
            Object = value;

            var b = (byte)Math.Min(value * 255 / max, 0xff);
            var f = (byte)~b;
            Background = Color.FromRgb(b, b, 0x00);
            Foreground = Color.FromRgb(f, f, f);
        }


        public ColoredObject(object data, Color fore, Color back)
        {
            Object = data;
            Foreground = fore;
            Background = back;
        }

        public override string ToString() => $"{Object},{Foreground},{Background}";

    }
}
