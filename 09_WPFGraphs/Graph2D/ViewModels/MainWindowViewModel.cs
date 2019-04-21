using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Graph2D.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        public static int[,] Array2D { get; } = GetArray2DRandom(9, 9, 0, 255);

        public MainWindowViewModel()
        {

        }

        private static int[,] GetArray2DRandom(int xLen, int yLen, int dataMin, int dataMax)
        {
            var ary = new int[xLen, yLen];
            int len0 = ary.GetLength(0);
            int len1 = ary.GetLength(1);

            var rdm = new Random();
            foreach (var x in Enumerable.Range(0, len0 * len1)
                .Select(_ => rdm.Next(dataMin, dataMax))
                .Select((data, index) => (data, index)))
            {
                ary[x.index / len0, x.index % len1] = x.data;
            }

            ary[3, 3] = 0;
            return ary;
        }

    }
}
