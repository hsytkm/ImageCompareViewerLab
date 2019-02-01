using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using PrismDispose.Views;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;

namespace PrismDispose.ViewModels
{
    public class ViewAViewModel : BindableBase, IDisposable
    {
        private IRegionManager _regionManager;

        public DelegateCommand CloseCommand { get; }

        public ViewAViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            CloseCommand = new DelegateCommand(() => RemoveModule<ViewA>("ContentRegion"));
        }

        // 指定リージョンからモジュールを削除
        private void RemoveModule<T>(string regionName) where T : UserControl
        {
            var viewToRemove = _regionManager.Regions[regionName].Views
                .FirstOrDefault(x => x.GetType().Name == typeof(T).Name);

            if (viewToRemove != null)
                _regionManager.Regions[regionName].Remove(viewToRemove);
        }

        public void Dispose() => Debug.WriteLine("ViewModel.Dispose()");
    }
}
