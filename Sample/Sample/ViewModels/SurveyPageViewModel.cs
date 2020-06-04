using System;
using AiForms.Dialogs.Abstractions;
using Prism.Mvvm;
using Reactive.Bindings;
using Sample.Views;
using Sample.Views.Dialogs;

namespace Sample.ViewModels
{
    public class SurveyPageViewModel:BindableBase
    {
        public ReactiveCommand ShowDialogCommand { get; } = new ReactiveCommand();
        public SurveyPageViewModel()
        {
            var dialog = AiForms.Dialogs.Dialog.Instance;
            ShowDialogCommand.Subscribe(async _ =>
            {
                var ret = await dialog.ShowAsync<TestDialog>();
                if (ret)
                {
                    //await dialog.ShowAsync<DialogTestView>();
                }
            });
        }
    }
}
