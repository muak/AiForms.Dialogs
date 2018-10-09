using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Reactive.Bindings;
using System.Threading.Tasks;
using AiForms.Extras;
using Sample.Views;
using System.ComponentModel;
using Xamarin.Forms;
using AiForms.Extras.Abstractions;

namespace Sample.ViewModels
{
	public class MainPageViewModel : BindableBase, INavigationAware
	{
        public ReactiveCommand LoadingCommand { get; } = new ReactiveCommand();
        public ReactiveCommand DialogCommand { get; } = new ReactiveCommand();
        public ReactiveCommand ToastCommand { get; } = new ReactiveCommand();

        public MainPageViewModel(MyIndicatorView myIndicatorView)
		{
           
            LoadingCommand.Subscribe(async _ =>
            {
                
                using(Loading.Instance.Start(out var progress,"Loading...")){
                    progress.Report(0d);
                    for (var i = 0; i < 100;i++){
                        if(i == 50){
                            Loading.Instance.SetMessage("Soon...");
                        }
                        await Task.Delay(100);
                        progress.Report((i + 1) * 0.01d);
                    }
                }
                //using (AiForms.Extras.Loading.Instance.Start("Loading..."))
                //{
                //    await Task.Delay(10000);
                //}
            });

            var dlgPage = new MyDialogView();


            DialogCommand.Subscribe(async _ =>
            {


                var ret = await redlg.ShowAsync();

                //dlg.Dispose();

                //var ret = await Dialog.Instance.ShowAsync<MyDialogView>(vm);
                //var ret = await Dialog.Instance.ShowAsync(page, vm);
            });

            ToastCommand.Subscribe(_ =>
            {
                Toast.Instance.Show<MyToastView>();
            });
		}

		public void OnNavigatedFrom(NavigationParameters parameters)
		{

		}

        IReusableDialog redlg;

		public void OnNavigatedTo(NavigationParameters parameters)
		{
            var vm = new { Title = "Title", Description = "Some description write here." };
            redlg = Dialog.Instance.Create<MyDialogView>(vm);
        }

		public void OnNavigatingTo(NavigationParameters parameters)
		{
		}
	}
}

