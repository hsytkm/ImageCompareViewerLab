using OxyPlotInspector.Views;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System.Linq;
using System.Windows.Controls;

namespace OxyPlotInspector.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        private readonly IContainerExtension _container;
        private readonly IRegionManager _regionManager;

        public DelegateCommand NotificationCommand { get; }

        public InteractionRequest<INotification> NotificationRequest { get; }
            = new InteractionRequest<INotification>();

        // ダイアログ表示中フラグ(多重表示回避)
        private bool _IsNotificationRequesting;
        public bool IsNotificationRequesting
        {
            get => _IsNotificationRequesting;
            private set => SetProperty(ref _IsNotificationRequesting, value, nameof(EnableNotificationButton));
        }

        // ボタンの押下可能フラグ(ダイアログ表示中は禁止)
        public bool EnableNotificationButton { get => !IsNotificationRequesting; }

        public MainWindowViewModel(IContainerExtension container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;

            _regionManager.RegisterViewWithRegion("ContentRegion", typeof(MainImage));

            NotificationCommand = new DelegateCommand(() =>
            {
                if (IsNotificationRequesting) return;
                NotificationRequest.Raise(
                    new Notification
                    {
                        Content = "Notification Message",
                        Title = "Notification"
                    },
                    r => IsNotificationRequesting = false);
                IsNotificationRequesting = true;
            });
            
        }

    }
}
