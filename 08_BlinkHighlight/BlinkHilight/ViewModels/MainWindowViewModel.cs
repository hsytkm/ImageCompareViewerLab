using BlinkHilight.Models;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BlinkHilight.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        private readonly MyImage MyImage = new MyImage();

        public AsyncReactiveCommand BlinkHighlightCommand { get; } = new AsyncReactiveCommand();

        public ReadOnlyReactiveProperty<BitmapSource> ImageSource { get; }

        public MainWindowViewModel(IContainerExtension container, IRegionManager regionManager)
        {
            ImageSource = MyImage.ObserveProperty(x => x.ImageSource).ToReadOnlyReactiveProperty();

            BlinkHighlightCommand.Subscribe(async _ => await MyImage.BlinkHighlightAsync());
        }

    }
}
