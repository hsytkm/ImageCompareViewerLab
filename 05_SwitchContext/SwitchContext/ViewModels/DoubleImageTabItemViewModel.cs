using Prism;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using SwitchContext.Common;
using System;
using System.Linq;
using System.Windows;

namespace SwitchContext.ViewModels
{
    class DoubleImageTabItemViewModel : BindableBase, IActiveAware
    {
        public string Title { get; } = "Double";

        private readonly IRegionManager _regionManager;
        private readonly IApplicationCommands _applicationCommands;
        public DelegateCommand SwapImagesCommand { get; }
        
        public DoubleImageTabItemViewModel(IRegionManager regionManager, IApplicationCommands applicationCommands)
        {
            _regionManager = regionManager;
            _applicationCommands = applicationCommands;

            SwapImagesCommand = new DelegateCommand(SwapImages);

            _applicationCommands.SaveCommand.RegisterCommand(SwapImagesCommand);

            IsActiveChanged += DoubleImageTabItemViewModel_IsActiveChanged;
        }

        private void DoubleImageTabItemViewModel_IsActiveChanged(object sender, EventArgs e)
        {
        }

        private void SwapImages()
        {
            var region = _regionManager.Regions["Image2ContentRegion"];
            var views = region.Views.Cast<FrameworkElement>();
            var viewsLength = views.Count();
            //if (viewsLength < 2) return;

            // 内回り
            var head = views.First().DataContext;
            for (int i = 0; i < viewsLength - 1; i++)
            {
                views.ElementAt(i).DataContext = views.ElementAt(i + 1).DataContext;
            }
            views.Last().DataContext = head;
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (SetProperty(ref _isActive, value))
                {
                    SwapImagesCommand.IsActive = value;
                    IsActiveChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        public event EventHandler IsActiveChanged;

    }
}
