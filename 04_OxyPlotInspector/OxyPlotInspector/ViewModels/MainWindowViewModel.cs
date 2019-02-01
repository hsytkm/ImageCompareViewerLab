using OxyPlotInspector.Models;
using OxyPlotInspector.Views;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;

namespace OxyPlotInspector.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        private readonly Histogram Histogram = ModelMaster.Instance.Histogram;

        private readonly IContainerExtension _container;
        private readonly IRegionManager _regionManager;

        public DelegateCommand NotificationCommand { get; }

        public InteractionRequest<INotification> NotificationRequest { get; }
            = new InteractionRequest<INotification>();

        // ダイアログ表示中フラグ(CanExecute対応)
        private bool _IsNotificationRequesting;
        private bool IsNotificationRequesting
        {
            get => _IsNotificationRequesting;
            set
            {
                if (SetProperty(ref _IsNotificationRequesting, value))
                {
                    NotificationCommand.RaiseCanExecuteChanged();
                    Histogram.IsShowingHistgramView = value;
                }
            }
        }

        public MainWindowViewModel(IContainerExtension container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;

            _regionManager.RegisterViewWithRegion("ContentRegion", typeof(MainImage));

            NotificationCommand = new DelegateCommand(() =>
            {
                NotificationRequest.Raise(
                    new Notification
                    {
                        Title = "Histogram Inspector",
                        Content = "Not Implement",
                    },
                    n => IsNotificationRequesting = false);
                IsNotificationRequesting = true;
            },
            () => !IsNotificationRequesting);   // 非表示中なら押下可能

        }

    }
}
