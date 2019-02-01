using Prism.Regions;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace PrismDispose.Views
{
    public partial class ViewA : UserControl, IDisposable
    {
        private IRegionManager _regionManager;

        public ViewA(IRegionManager regionManager)
        {
            InitializeComponent();

            _regionManager = regionManager;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _regionManager.Regions["ContentRegion"].Remove(this);
        }

        public void Dispose() => Debug.WriteLine("ViewA.Dispose()");

    }
}
