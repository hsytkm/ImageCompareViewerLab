using Prism.Commands;
using Prism.Mvvm;
using System;

namespace Graph2D.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        private static readonly int ValueMax = 4096;
        private static readonly int RowLengthMin = 1;
        private static readonly int RowLengthMax = 10;
        private static readonly int ColumnLengthMin = 1;
        private static readonly int ColumnLengthMax = 10;

        private static readonly Random Random = new Random();

        private ColoredObjectRow[] _Data1 = GetRandomData1();
        public ColoredObjectRow[] Data1
        {
            get => _Data1;
            private set => SetProperty(ref _Data1, value);
        }

        //public ColoredObject[,] Data2 { get; }

        public DelegateCommand UpdateDataCommand { get; }

        public MainWindowViewModel()
        {
#if false
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
#endif
            UpdateDataCommand = new DelegateCommand(() => Data1 = GetRandomData1());
        }

        private static ColoredObjectRow[] GetRandomData1()
        {
            int rowLength = Random.Next(RowLengthMin, RowLengthMax);
            int columnLength = Random.Next(ColumnLengthMin, ColumnLengthMax);

            var data = new ColoredObjectRow[rowLength];
            for (int r = 0; r < data.Length; r++)
            {
                var objects = new ColoredObject[columnLength];
                for (int c = 0; c < objects.Length; c++)
                {
                    objects[c] = new ColoredObject(Random.Next(ValueMax), ValueMax);
                }
                data[r] = new ColoredObjectRow(objects);
            }
            return data;
        }

    }
}
