using Prism.Commands;
using Prism.Mvvm;
using System;

namespace Graph2D.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        private static readonly int RowLengthMin = 1;
        private static readonly int RowLengthMax = 10;
        private static readonly int ColumnLengthMin = 1;
        private static readonly int ColumnLengthMax = 10;

        private static readonly Random Random = new Random();

        private ColoredObject[,] _Array2d = GetSortedData();
        public ColoredObject[,] Array2d
        {
            get => _Array2d;
            private set => SetProperty(ref _Array2d, value);
        }

        public DelegateCommand ReflashDataCommand { get; }

        public MainWindowViewModel()
        {
            ReflashDataCommand = new DelegateCommand(() => Array2d = GetSortedData());
        }

        private static ColoredObject[,] GetSortedData()
        {
            int rowLength = Random.Next(RowLengthMin, RowLengthMax);
            int columnLength = Random.Next(ColumnLengthMin, ColumnLengthMax);
            int max = rowLength * columnLength;

            var data = new ColoredObject[rowLength, columnLength];
            for (int r = 0; r < data.GetLength(0); r++)
            {
                for (int c = 0; c < data.GetLength(1); c++)
                {
                    data[r, c] = new ColoredObject(1 + r * columnLength + c, max);
                }
            }
            return data;
        }

    }
}
