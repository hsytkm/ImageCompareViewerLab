using Prism.Regions;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace PrismDispose.Module1.Views
{
    /// <summary>
    /// Interaction logic for ViewA.xaml
    /// </summary>
    public partial class ViewA : UserControl, IDisposable
    {
        public string ViewName { get; } = nameof(ViewA);

        private IRegionManager _regionManager { get; set; }

        public ViewA(IRegionManager regionManager)
        {
            InitializeComponent();

            _regionManager = regionManager;
        }

        public void Dispose()
        {
            Debug.WriteLine("ViewA.Dispose()");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _regionManager.Regions["ContentRegion"].Remove(this);
        }

    }
}
