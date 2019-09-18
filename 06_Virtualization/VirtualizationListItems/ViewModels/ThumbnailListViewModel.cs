using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Media.Imaging;
using VirtualizationListItems.Models;

namespace VirtualizationListItems.ViewModels
{
    class ThumbnailListViewModel : BindableBase
    {
        public ReadOnlyObservableCollection<ThubnailVModel> Thumbnails { get; }

        public ReactiveProperty<ThubnailVModel> SelectedItem { get; }

        // スクロール変化時
        public ReactiveProperty<(double CenterRatio, double ViewportRatio)> ScrollChangedHorizontal { get; } =
            new ReactiveProperty<(double CenterRatio, double ViewportRatio)>(mode: ReactivePropertyMode.DistinctUntilChanged);
        
        // 削除したいファイルPATH
        public ReactiveProperty<string> DeleteFilePath { get; } =
            new ReactiveProperty<string>(mode: ReactivePropertyMode.None);

        // Deleteボタンの動作  OFF=リストから削除, ON=リストから削除+ゴミ箱移動
        public ReactiveProperty<bool> EnableFileDelete { get; } = new ReactiveProperty<bool>();

        public InteractionRequest<IConfirmation> ConfirmationRequest { get; } = new InteractionRequest<IConfirmation>();

        public ThumbnailListViewModel(IContainerExtension container, IRegionManager regionManager)
        {
            var imageSources = container.Resolve<ImageSources>();
            var modelImageSources = imageSources.Sources;

            // Mの画像リストをVM用に変換(schedulerがないと意図通りに動作しないけど理解できていない…)
            Thumbnails = modelImageSources
                .ToReadOnlyReactiveCollection(m => new ThubnailVModel(m), scheduler: Scheduler.CurrentThread);

            // Mの画像更新をVMに通知(解放時のnullも通知される)
            modelImageSources
                .ObserveElementProperty(m => m.Thumbnail)
                .Subscribe(m =>
                {
                    //Console.WriteLine($"{x.Instance} {x.Property} {x.Value}");
                    var vmItem = Thumbnails.FirstOrDefault(vm => vm.FilePath == m.Instance.FilePath);
                    if (vmItem != null) vmItem.Image = m.Value;
                });

#if false
            // VM→M通知(nullになったらnullを通知する。後で変化エッジを付けるため)
            SelectedItem
                .Select(vm => vm?.FilePath)
                .Subscribe(x => imageSources.SelectedImagePath = x);

            // M→VM通知
            imageSources
                .ObserveProperty(x => x.SelectedImagePath)
                .Select(m => Thumbnails.FirstOrDefault(vm => vm.FilePath == m))
                .Subscribe(x => SelectedItem.Value = x);
#else
            // VM⇔Mの通知を以下に置き換えた(Thumbnailsのscheduler指定がポイント)
            SelectedItem = imageSources
                .ToReactivePropertyAsSynchronized(x => x.SelectedImagePath,
                    // M->VM
                    convert: m => Thumbnails.FirstOrDefault(vm => vm.FilePath == m),
                    // M<-VM
                    convertBack: vm => imageSources.SelectedImagePath = vm?.FilePath);
#endif

            // スクロール操作時の画像読出/解放
            ScrollChangedHorizontal
                .Subscribe(x => imageSources.UpdateThumbnail(x.CenterRatio, x.ViewportRatio));

            // リストからファイル削除
            DeleteFilePath
                .Subscribe(x =>
                {
                    if (EnableFileDelete.Value)
                    {
                        ConfirmationRequest.Raise(new Confirmation()
                        {
                            Title = "Confirmation",
                            Content = "Confirmation Message"
                        },
                        r =>
                        {
                            if (r.Confirmed) imageSources.DeleteImageFile(x);
                        });
                    }
                    else
                    {
                        imageSources.ClearImageFile(x);
                    }
                });
        }
    }

    class ThubnailVModel : BindableBase
    {
        private BitmapSource _Image;
        public BitmapSource Image
        {
            get => _Image;
            set => SetProperty(ref _Image, value);
        }
        public string FilePath { get; }
        public string Filename { get; }

        public ThubnailVModel(ImageSource source)
        {
            Image = source?.Thumbnail;
            FilePath = source?.FilePath;
            Filename = source?.Filename;
        }
    }

}
