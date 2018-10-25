using System;
using Prism.Navigation;
using Reactive.Bindings;
using AiForms.Dialogs;
using Sample.Views;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using AiForms.Dialogs.Abstractions;
using System.Diagnostics;
namespace Sample.ViewModels
{
    public class AutoTestViewModel
    {
        public ReactiveCommand StartCommand { get; } = new ReactiveCommand();
        public IDialogNotifier Notifier { get; set; }
        public List<TestItem> TestResults { get; } = new List<TestItem>();

        public AutoTestViewModel(INavigationService navigationService)
        {
            StartCommand.Subscribe(async _ =>
            {
                await ToastTest();
                await LoadingTest();
                await DialogTest();

                await navigationService.NavigateAsync("ResultPage", new NavigationParameters { { "item", TestResults } });
            });
        }

        async Task DialogTest()
        {
            var dialog = new DialogTestView();

            ExitDialog(dialog.DialogNotifier, false);
            var ret = await Dialog.Instance.ShowAsync(dialog);

            var result = new TestItem
            {
                Name = "Dialog ShowAsync1",
                Result = dialog.Assert.All(x => x) && dialog.Assert.Count == 5 && ret,
                Detail = string.Join(", ", dialog.Assert.Select((x, index) => new { index, result = x })
                                     .Where(x => !x.result).Select(x => x.index + 1))
            };

            TestResults.Add(result);

            ExitDialog(dialog.DialogNotifier, true);
            ret = await Dialog.Instance.ShowAsync(dialog);

            TestResults.Add(new TestItem
            {
                Name = "Dialog ShowAsync2",
                Result = dialog.Assert.All(x => x) && dialog.Assert.Count == 5 && !ret,
                Detail = string.Join(", ", dialog.Assert.Select((x, index) => new { index, result = x })
                                     .Where(x => !x.result).Select(x => x.index + 1))
            });


            var reusable = Dialog.Instance.Create(dialog);

            ExitDialog(dialog.DialogNotifier, false);
            ret = await reusable.ShowAsync();

            TestResults.Add(new TestItem
            {
                Name = "Dialog Create ShowAsync",
                Result = dialog.Assert.All(x => x) && dialog.Assert.Count == 4 && ret,
                Detail = string.Join(", ", dialog.Assert.Select((x, index) => new { index, result = x })
                                     .Where(x => !x.result).Select(x => x.index + 1))
            });

            reusable.Dispose();

            TestResults.Add(new TestItem
            {
                Name = "Dialog Create Dispose",
                Result = dialog.Assert.All(x => x) && dialog.Assert.Count == 5,
                Detail = string.Join(", ", dialog.Assert.Select((x, index) => new { index, result = x })
                                     .Where(x => !x.result).Select(x => x.index + 1))
            });


            dialog = new DialogTestView();
            reusable = Dialog.Instance.Create(dialog, this);

            ExitDialog(Notifier, false);
            var notifierRetOk = await reusable.ShowAsync();

            await Task.Delay(250);

            ExitDialog(Notifier, true);
            var notifierRetNg = await reusable.ShowAsync();

            TestResults.Add(new TestItem
            {
                Name = "Dialog Send cancel or complete from VM",
                Result = notifierRetOk && !notifierRetNg,
            });

        }

        async void ExitDialog(IDialogNotifier notifier,bool isCancel = false)
        {
            await Task.Delay(1000);
            Device.BeginInvokeOnMainThread(() => {
                if(isCancel)
                {
                    notifier.Cancel();
                }
                else{
                    notifier.Complete();
                }
            });
        }

        async Task ToastTest()
        {
            var toastView = new ToastTestView{Tcs = new TaskCompletionSource<bool>()};
            Toast.Instance.Show(toastView, new { Duration = 500 });
            await toastView.Tcs.Task;

            var result = new TestItem
            {
                Name = "Toast",
                Result = toastView.Assert.All(x => x) && toastView.Assert.Count == 4,
                Detail = string.Join(", ", toastView.Assert.Select((x, index) => new { index, result = x })
                                     .Where(x => !x.result).Select(x => x.index+1))
            };


            TestResults.Add(result);

            toastView = new ToastTestView{Tcs = new TaskCompletionSource<bool>()};

            Toast.Instance.Show(toastView, new { Duration = 1500 });
            await toastView.Tcs.Task;

            result = new TestItem
            {
                Name = "Toast2",
                Result = toastView.Assert.All(x => x) && toastView.Assert.Count == 4,
                Detail = string.Join(", ", toastView.Assert.Select((x, index) => new { index, result = x })
                                     .Where(x => !x.result).Select(x => x.index))
            };

            TestResults.Add(result);
        }

        async Task LoadingTest()
        {
            var config = new LoadingConfig();
            Configurations.LoadingConfig = config;

            Loading.Instance.Show();
            await Task.Delay(100);
            Loading.Instance.Hide();

            await Loading.Instance.StartAsync(async p =>
            {
                Debug.WriteLine("Default1");
                await Task.Delay(250);
            });

            await Loading.Instance.StartAsync(async p =>
            {
                Debug.WriteLine("Default2");
                await Task.Delay(250);
            },"hoge",true);


            Loading.Instance.Dispose();

            config.IsReusable = true;

            Loading.Instance.Show();
            await Task.Delay(100);
            Loading.Instance.Hide();

            await Loading.Instance.StartAsync(async p =>
            {
                Debug.WriteLine("Default3");
                await Task.Delay(250);
            });

            await Loading.Instance.StartAsync(async p =>
            {
                Debug.WriteLine("Default4");
                await Task.Delay(250);
            }, "hoge", true);

            // 通過テスト 通過できればOKとする
            var result = new TestItem
            {
                Name = "Loading Default",
                Result = true
            };

            TestResults.Add(result);

            Loading.Instance.Dispose();

            await Task.Delay(500);

            var loading = new LoadingTestView();
            var reusable = Loading.Instance.Create(loading);
           
            await reusable.StartAsync(async p =>
            {
                await Task.Delay(50);
                p.Report(0.3);
                await Task.Delay(50);
                p.Report(0.6);
                await Task.Delay(50);
                p.Report(0.9);
                await Task.Delay(50);
            });

            await Task.Delay(500);

            TestResults.Add(new TestItem
            {
                Name = "Loading Custom",
                Result = loading.Assert.All(x => x) && loading.Assert.Count == 3, // ReuseなのでDestroyはなし
                Detail = string.Join(", ", loading.Assert.Select((x, index) => new { index, result = x })
                                     .Where(x => !x.result).Select(x => x.index+1))
            });


            await reusable.StartAsync(async p =>
            {
                await Task.Delay(50);
                p.Report(0.3);
                await Task.Delay(50);
                p.Report(0.6);
                await Task.Delay(50);
                p.Report(0.9);
                await Task.Delay(50);
            },true);

            await Task.Delay(500);

            TestResults.Add(new TestItem
            {
                Name = "Loading Custom Reuse",
                // 2回目は前回のインスタンスを使用している
                Result = loading.Assert.Count == 6 && loading.seq == 5 && loading.progressCnt == 6,
            });

            reusable.Dispose();

            TestResults.Add(new TestItem
            {
                Name = "Loading Custom Dispose",
                Result = loading.Assert.Count == 7 && loading.seq == 6 && loading.progressCnt == 6
            });
        }
    }

    public class TestItem
    {
        public string Name { get; set; }
        public bool Result { get; set; }
        public string Detail { get; set; }
        public Color Color {
            get{
                return Result ? Color.Green : Color.Red;
            }
        }
    }
}
