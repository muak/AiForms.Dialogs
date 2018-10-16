using System;
using Reactive.Bindings;
using Prism.Navigation;

namespace Sample.ViewModels
{
    public class IndexPageViewModel
    {
        public ReactiveCommand<string> GoToTest { get; set; } = new ReactiveCommand<string>();

        public IndexPageViewModel(INavigationService navigationService)
        {
            GoToTest.Subscribe(async p => {
                await navigationService.NavigateAsync(p);
            });
        }
    }
}
