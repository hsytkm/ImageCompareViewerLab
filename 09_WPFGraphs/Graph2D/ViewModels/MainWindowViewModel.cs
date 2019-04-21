using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Graph2D.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        public static int[,] Array2D { get; } = GetArray2DRandom(4, 4, 0, 255);

        public MainWindowViewModel() { }

        private static int[,] GetArray2DRandom(int rLen, int cLen, int dataMin, int dataMax)
        {
            var ary = new int[rLen, cLen];
            int rowLength = ary.GetLength(0);
            int columnLength = ary.GetLength(1);

            var rdm = new Random();
            foreach (var x in Enumerable.Range(0, rowLength * columnLength)
                .Select(_ => rdm.Next(dataMin, dataMax))
                .Select((data, index) => (data, index)))
            {
                ary[x.index / columnLength, x.index % columnLength] = x.data;
            }

            ary[0, 0] = dataMin;
            ary[rowLength - 1, columnLength - 1] = dataMax;
            return ary;
        }

    }
}
