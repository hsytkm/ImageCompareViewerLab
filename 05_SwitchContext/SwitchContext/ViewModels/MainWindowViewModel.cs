using Prism.Mvvm;
using SwitchContext.Common;

namespace SwitchContext.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        public IApplicationCommands ApplicationCommands { get; }

        public MainWindowViewModel(IApplicationCommands applicationCommands)
        {
            ApplicationCommands = applicationCommands;
        }

    }
}
