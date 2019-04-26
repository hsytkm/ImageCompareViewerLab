using System.Collections.Generic;

namespace Graph2D.Views
{
    class ColoredObjectRow
    {
        public IReadOnlyList<ColoredObject> ItemsSource { get; }

        public ColoredObjectRow(int r, ColoredObject[,] source)
        {
            int colLength = source.GetLength(1);
            var items = new List<ColoredObject>(colLength);
            for (var c = 0; c < items.Capacity; c++)
            {
                items.Add(source[r, c]);
            }
            ItemsSource = items;
        }

    }
}
