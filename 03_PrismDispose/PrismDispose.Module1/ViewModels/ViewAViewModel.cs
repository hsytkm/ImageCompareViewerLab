using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Diagnostics;
using System.Linq;

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
                var viewToRemove = _regionManager.Regions["ContentRegion"].Views
                    .FirstOrDefault<dynamic>(v => v.ViewTitle == nameof(Views.ViewA));
                if (viewToRemove != null)
                {
                    _regionManager.Regions["ContentRegion"].Remove(viewToRemove);
                }
            });
        }

        public void Dispose()
        {
            Debug.WriteLine("ViewAViewModel.Dispose()");
        }
    }
}
