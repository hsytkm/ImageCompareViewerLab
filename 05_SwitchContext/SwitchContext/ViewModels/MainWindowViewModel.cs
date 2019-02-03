using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Windows;

namespace SwitchContext.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        public DelegateCommand SwapCommand { get; }

        public MainWindowViewModel(IContainerExtension container, IRegionManager regionManager)
        {

        }

    }
}
