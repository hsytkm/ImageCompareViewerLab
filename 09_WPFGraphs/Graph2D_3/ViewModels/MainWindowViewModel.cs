using Prism.Mvvm;
using System;

namespace Graph2D.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        private static readonly int ValueMax = 4096;
        private static readonly int RowLength = 5;
        private static readonly int ColumnLength = 4;

        public ColoredObject[,] Data { get; }

        public MainWindowViewModel()
        {
            var random = new Random();
            var data = new ColoredObject[RowLength, ColumnLength];
            for (int c = 0; c < data.GetLength(0); c++)
            {
                for (int r = 0; r < data.GetLength(1); r++)
                    data[c, r] = new ColoredObject(random.Next(ValueMax), ValueMax);
            }
            Data = data;
        }

    }
}
