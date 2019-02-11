using Prism.Commands;
using Prism.Mvvm;
using VirtualizationListItems.Models;

namespace VirtualizationListItems.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly ImageSources ImageSources = ImageSources.Instance;

        public DelegateCommand ReadImagesCommand { get; }

        public MainWindowViewModel()
        {
            ReadImagesCommand = new DelegateCommand(() => ImageSources.Initialize());

        }
    }
}
