using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismDropNavigation.TabItem.ViewModels
{
    public class TabItemDoubleViewModel : BindableBase, INavigationAware, ITabItemViewModel
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
        }

        // ナビゲーション移行時に移行先になるか判定のためコールされる
        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        // ナビゲーションが他に移る時にコールされる
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        // ナビゲーションが移ってきた時にコールされる
        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

    }
}
