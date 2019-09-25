using System;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Reactive.Bindings;

namespace VirtualizationListItems.ViewModels
{
    public class ConfirmDialogViewModel : BindableBase, IDialogAware
    {
        public string Title => "タイトル";

        public ReactiveProperty<string> Message { get; } = new ReactiveProperty<string>("");
        public ReactiveCommand YesCommand { get; } = new ReactiveCommand();

        public ReactiveCommand NoCommand { get; } = new ReactiveCommand();

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Message.Value = parameters.GetValue<string>("Message");
        }

        public ConfirmDialogViewModel()
        {
            YesCommand.Subscribe(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Yes)));
            NoCommand.Subscribe(() => RequestClose?.Invoke(new DialogResult(ButtonResult.No)));
        }
    }
}