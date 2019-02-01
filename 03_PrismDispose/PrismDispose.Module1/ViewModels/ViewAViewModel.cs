using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;

namespace PrismDispose.Module1.ViewModels
{
    public class ViewAViewModel : BindableBase, IDisposable
    {
        private IRegionManager _regionManager;

        private string _message = "View A from Module";
        public string Message
        {
            get { return _message; }
            private set { SetProperty(ref _message, value); }
        }

        public DelegateCommand CloseCommand { get; }

        public ViewAViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            CloseCommand = new DelegateCommand(() =>
            {
                RemoveModule<Views.ViewA>("ContentRegion");
            });
        }

        // 指定リージョンからモジュールを削除
        private void RemoveModule<T>(string regionName) where T : UserControl
        {
            var viewToRemove = _regionManager.Regions[regionName].Views
                .FirstOrDefault<dynamic>(v => v.ViewName == typeof(T).Name);

            if (viewToRemove != null)
                _regionManager.Regions[regionName].Remove(viewToRemove);
        }

        public void Dispose()
        {
            Debug.WriteLine("ViewAViewModel.Dispose()");
        }
    }
}
