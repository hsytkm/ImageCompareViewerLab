using Prism;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PrismDropNavigation.TabItem.ViewModels
{
    public class TabItemViewModelBase : BindableBase, IActiveAware, INavigationAware
    {
        public int Index { get; private set; }
        public string Title { get; private set; }

        private string _message;
        public string Message
        {
            get => _message;
            internal set => SetProperty(ref _message, value);
        }

        public TabItemViewModelBase()
        {
            IsActiveChanged += (sender, e) =>
            {
                if (e is DataEventArgs<bool> e2)
                    Console.WriteLine($"{Title}-IsActive: {e2.Value}");
            };
        }

        public void SetIndex(int index)
        {
            Index = index;
            Title = $"Title{index}";
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
            var messages = new List<string>();
            for (int i = 0; i < Index; i++)
            {
                if (navigationContext.Parameters[$"image{i}"] is string message)
                    messages.Add(message);
            }

            if (messages.Any())
                Message = string.Join(" | ", messages);
        }

        public static NavigationParameters GetNavigationParameters(IList<string> messages)
        {
            var parameters = new NavigationParameters();
            for (int i = 0; i < messages.Count; i++)
            {
                parameters.Add($"image{i}", messages[i]);
            }
            return parameters;
        }

        #endregion

    }
}
