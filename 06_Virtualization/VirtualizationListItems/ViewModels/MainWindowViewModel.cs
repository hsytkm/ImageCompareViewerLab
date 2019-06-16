using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using VirtualizationListItems.Models;

namespace VirtualizationListItems.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public DelegateCommand ReadImagesCommand { get; }

        public ReadOnlyReactiveProperty<string> SelectedPath { get; }
        public ReadOnlyReactiveProperty<string> LoadStatus { get; }

        public MainWindowViewModel(IContainerExtension container, IRegionManager regionManager)
        {
            var imageSources = container.Resolve<ImageSources>();

            ReadImagesCommand = new DelegateCommand(() => imageSources.Initialize());

            // 選択PATHのデバッグ表示
            SelectedPath = imageSources.ObserveProperty(x => x.SelectedImagePath).ToReadOnlyReactiveProperty();

            // 読み込み画像のデバッグ表示
            LoadStatus = imageSources.ObserveProperty(x => x.LoadStatus).ToReadOnlyReactiveProperty();

        }
    }
}
