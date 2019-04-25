using Prism.Mvvm;
using System;

namespace Graph2D.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        private static readonly int ValueMax = 4096;
        private static readonly int RowLength = 5;
        private static readonly int ColumnLength = 4;

        public ColoredObjectRow[] Data1 { get; }
        public ColoredObject[,] Data2 { get; }

        public MainWindowViewModel()
        {
            var random = new Random();

            var data1 = new ColoredObjectRow[RowLength];
            for (int r = 0; r < RowLength; r++)
            {
                var objects = new ColoredObject[ColumnLength];
                for (int c = 0; c < ColumnLength; c++)
                {
                    objects[c] = new ColoredObject(random.Next(ValueMax), ValueMax);
                }
                data1[r] = new ColoredObjectRow(objects);
            }
            Data1 = data1;

            var data2 = new ColoredObject[RowLength, ColumnLength];
            for (int c = 0; c < data2.GetLength(0); c++)
            {
                for (int r = 0; r < data2.GetLength(1); r++)
                {
                    data2[c, r] = new ColoredObject(random.Next(ValueMax), ValueMax);
                    //data2[c, r] = new ColoredObject(c * data2.GetLength(1) + r, data2.GetLength(0) * data2.GetLength(1));
                }
            }
            Data2 = data2;
        }

    }
}
