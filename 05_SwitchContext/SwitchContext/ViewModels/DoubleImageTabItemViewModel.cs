using Prism;
using Prism.Commands;
using Prism.Mvvm;
using SwitchContext.Common;
using System;

namespace SwitchContext.ViewModels
{
    class DoubleImageTabItemViewModel : BindableBase, IActiveAware
    {
        public string Title { get; } = "Double";

        public DelegateCommand SwapImagesCommand { get; }
        
        public DoubleImageTabItemViewModel(IApplicationCommands applicationCommands)
        {
            SwapImagesCommand = new DelegateCommand(SwapImages);

            applicationCommands.SaveCommand.RegisterCommand(SwapImagesCommand);
        }

        private void SwapImages()
        {
            Console.WriteLine($"SwapImages({Title})");
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
