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
    public class TabItemDoubleViewModel : BindableBase, IActiveAware, INavigationAware, ITabItemViewModel
    {
        public string Title { get; set; }

        private string _message;
        public string Message
        {
            get => _message;
            private set => SetProperty(ref _message, value);
        }

        public TabItemDoubleViewModel()
        {
            Message = "Double from your Prism Module";

            IsActiveChanged += (object sender, EventArgs e) =>
            {
                if (e is DataEventArgs<bool> e2)
                    Console.WriteLine($"Double-IsActive: {e2.Value}");
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
            var messages = new List<string>(2);
            if (navigationContext.Parameters["image0"] is string message0)
                messages.Add(message0);
            if (navigationContext.Parameters["image1"] is string message1)
                messages.Add(message1);
            Message = string.Join(" | ", messages);
        }

        #endregion

    }
}
