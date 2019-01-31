using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismDropNavigation.TabItem.ViewModels
{
    public class TabItemSingleViewModel : BindableBase, IActiveAware, INavigationAware, ITabItemViewModel
    {
        public string Title { get; set; }

        private string _message;
        public string Message
        {
            get => _message;
            private set => SetProperty(ref _message, value);
        }

        public TabItemSingleViewModel()
        {
            Message = "Single from your Prism Module";

            IsActiveChanged += (object sender, EventArgs e) =>
            {
                if (e is DataEventArgs<bool> e2)
                    Console.WriteLine($"Single-IsActive: {e2.Value}");
            };
        }

        #region IActiveAware

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (SetProperty(ref _isActive, value))
                    IsActiveChanged?.Invoke(this, new DataEventArgs<bool>(value));
            }
        }

        public event EventHandler IsActiveChanged;

        #endregion

        #region INavigationAware

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;
        public void OnNavigatedFrom(NavigationContext navigationContext) { }

        // ナビゲーションが移ってきた時にコールされる
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var message = "";
            if (navigationContext.Parameters["image0"] is string message0)
                message = message0;
            Message = message;
        }

        #endregion

    }
}
