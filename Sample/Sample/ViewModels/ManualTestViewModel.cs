using System;
using Reactive.Bindings;
using System.Threading.Tasks;
using System.Collections.Generic;
using AiForms.Dialogs;
using Sample.Views;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using AiForms.Dialogs.Abstractions;

namespace Sample.ViewModels
{
    public class ManualTestViewModel
    {
        public ReactiveCommand RunCommand { get; } = new ReactiveCommand();
        public ReactiveCommand AllCheckCommand { get; } = new ReactiveCommand();
        public ReactiveCommand NoneCheckCommand { get; } = new ReactiveCommand();
        public ReactiveCommand SaveCommand { get; } = new ReactiveCommand();
        public ReactivePropertySlim<bool> CheckSize { get; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> CheckLayout { get; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> CheckCorner { get; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> CheckToastCommon { get; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> CheckDialogCommon { get; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> CheckDialog { get; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> CheckLoadingCommon { get; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> CheckLoading { get; } = new ReactivePropertySlim<bool>();

        List<ReactivePropertySlim<bool>> Checks = new List<ReactivePropertySlim<bool>>();
        Func<Task> Show;
        DialogTestViewModel _dialogVM = new DialogTestViewModel();

        public ManualTestViewModel()
        {
            Checks.Add(CheckSize);
            Checks.Add(CheckLayout);
            Checks.Add(CheckCorner);
            Checks.Add(CheckToastCommon);
            Checks.Add(CheckDialogCommon);
            Checks.Add(CheckDialog);
            Checks.Add(CheckLoadingCommon);
            Checks.Add(CheckLoading);

            RunCommand.Subscribe(async _ =>
            {
                await ToastTest();
                await DialogTest();
                await LoadingTest();
            });

            void CheckChange(bool turned)
            {
                foreach (var check in Checks)
                {
                    check.Value = turned;
                }
            }

            AllCheckCommand.Subscribe(_ => CheckChange(true));
            NoneCheckCommand.Subscribe(_ => CheckChange(false));

            SaveCommand.Subscribe(_ =>
            {
                for (var i = 0; i < Checks.Count; i++)
                {
                    Application.Current.Properties[$"check{i}"] = Checks[i].Value;
                }
                Application.Current.SavePropertiesAsync();
            });

            for (var i = 0; i < Checks.Count; i++)
            {
                if(Application.Current.Properties.TryGetValue($"check{i}",out var check))
                {
                    Checks[i].Value = (bool)check;
                }
            }
        }

        async Task ToastTest()
        {
            await DoCommon(CheckToastCommon.Value);
        }

        async Task DialogTest()
        {
            await DoCommon(CheckDialogCommon.Value);
            if (!CheckDialog.Value) return;

            // Use CurrentPage Location
            _dialogVM.UseCurrentPageLocation = true;
            await LayoutTest();

            // Overlay Color
            _dialogVM.OverlayColor = Color.FromRgba(0.9, 0 , 0, 0.4);
            _dialogVM.Message = "Turn red";
            await Show();

            // IsCanceledOnTouchOutside OFF
            _dialogVM.IsCanceledOnTouchOutside = false;
            _dialogVM.Message = "Can't close when tapping outside.";
            await Show();

            _dialogVM.Reset();
        }

        async Task LoadingTest()
        {
            await DoCommon(CheckLoadingCommon.Value);
            if (!CheckLoading.Value) return;

            // Overlay Color
            _dialogVM.OverlayColor = Color.FromRgba(0.9, 0, 0, 0.4);
            await Show();

            _dialogVM.Reset();

            // Default Loading Test Start
            SetShowAction("LoadingDefault");


            Configurations.LoadingConfig = new LoadingConfig();

            // IndicatorColor
            Configurations.LoadingConfig.IndicatorColor = Color.Red;
            _dialogVM.Message = "Indicator turn red";
            await Show();

            // OverlayColor
            Configurations.LoadingConfig.OverlayColor = Color.FromRgba(0.9, 0, 0, 0.4);
            _dialogVM.Message = "Ovarlay turn red";
            await Show();

            // FontSize
            Configurations.LoadingConfig.FontSize = 20;
            _dialogVM.Message = "Font Size Large";
            await Show();

            // FontColor
            Configurations.LoadingConfig.FontColor = Color.Blue;
            _dialogVM.Message = "Font Color Blue";
            await Show();

            // Opacity
            Configurations.LoadingConfig.Opacity = 1.0;
            _dialogVM.Message = "Opacity 1.0";
            await Show();

            // Offset 50 50
            Configurations.LoadingConfig.OffsetX = 50;
            Configurations.LoadingConfig.OffsetY = 50;
            _dialogVM.Message = "Offset 50 50";
            await Show();

            // Offset -50 -50
            Configurations.LoadingConfig.OffsetX = -50;
            Configurations.LoadingConfig.OffsetY = -50;
            _dialogVM.Message = "Offset -50 -50";
            await Show();

            // Reset config
            Configurations.LoadingConfig = new LoadingConfig();
        }

        async Task DoCommon(bool check, [CallerMemberName]  string member = "")
        {
            SetShowAction(member);
            if (!check) return;

            await SizeTest();
            await LayoutTest();
            await CornerTest();
        }

        async Task SizeTest()
        {
            if (!CheckSize.Value) return;

            _dialogVM.Duration = 2500;

            // Fix / Fix
            _dialogVM.ProportionalWidth = -1;
            _dialogVM.ProportionalHeight = -1;
            _dialogVM.HeightRequest = 120;
            _dialogVM.Message = "W120 H120";
            await Show();

            // Fix / Auto
            _dialogVM.HeightRequest = -1d;
            _dialogVM.Message = "W120 Auto";
            await Show();

            // Proportional 0.5 / Auto
            _dialogVM.ProportionalWidth = 0.5d;
            _dialogVM.Message = "W0.5 Auto";
            await Show();

            // Proportional 1.0 / Auto
            _dialogVM.ProportionalWidth = 1.0d;
            _dialogVM.Message = "W1.0 Auto";
            await Show();

            // Proportional 1.0 / Proportional 0.5
            _dialogVM.ProportionalHeight = 0.5d;
            _dialogVM.Message = "W1.0 H0.5";
            await Show();

            // Proportional 1.0 / Proportional 1.0
            _dialogVM.ProportionalHeight = 1.0d;
            _dialogVM.Message = "W1.0 H1.0";
            await Show();


            _dialogVM.Reset();
        }

        async Task LayoutTest()
        {
            if (!CheckLayout.Value) return;

            _dialogVM.Duration = 1500;

            // Center / Center Offset 0 0
            _dialogVM.SetLayout(LayoutAlignment.Center, LayoutAlignment.Center, 0);
            await Show();

            // Center / Center Offset 50 50
            _dialogVM.SetLayout(LayoutAlignment.Center, LayoutAlignment.Center, 50);
            await Show();

            // Center / Center Offset -50 -50
            _dialogVM.SetLayout(LayoutAlignment.Center, LayoutAlignment.Center, -50);
            await Show();

            // Top / Left Offset 0 0
            _dialogVM.SetLayout(LayoutAlignment.Start, LayoutAlignment.Start, 0);
            await Show();

            // Top / Left Offset 50 50
            _dialogVM.SetLayout(LayoutAlignment.Start, LayoutAlignment.Start, 50);
            await Show();

            // Top / Left Offset 50 50
            _dialogVM.SetLayout(LayoutAlignment.Start, LayoutAlignment.Start, -50);
            await Show();

            // Top / Center Offset 0 0
            _dialogVM.SetLayout(LayoutAlignment.Start, LayoutAlignment.Center, 0);
            await Show();

            // Top / Center Offset 50 50
            _dialogVM.SetLayout(LayoutAlignment.Start, LayoutAlignment.Center, 50);
            await Show();

            // Top / Center Offset -50 -50
            _dialogVM.SetLayout(LayoutAlignment.Start, LayoutAlignment.Center, -50);
            await Show();

            // Top / Right Offset 0 0
            _dialogVM.SetLayout(LayoutAlignment.Start, LayoutAlignment.End, 0);
            await Show();

            // Top / Right Offset 50 50
            _dialogVM.SetLayout(LayoutAlignment.Start, LayoutAlignment.End, 50);
            await Show();

            // Top / Right Offset -50 -50
            _dialogVM.SetLayout(LayoutAlignment.Start, LayoutAlignment.End, -50);
            await Show();

            // Center / Left Offset 0 0
            _dialogVM.SetLayout(LayoutAlignment.Center, LayoutAlignment.Start, 0);
            await Show();

            // Center / Left Offset 50 50
            _dialogVM.SetLayout(LayoutAlignment.Center, LayoutAlignment.Start, 50);
            await Show();

            // Center / Left Offset -50 -50
            _dialogVM.SetLayout(LayoutAlignment.Center, LayoutAlignment.Start, -50);
            await Show();

            // Center / Right Offset 0 0
            _dialogVM.SetLayout(LayoutAlignment.Center, LayoutAlignment.End, 0);
            await Show();

            // Center / Right Offset 50 50
            _dialogVM.SetLayout(LayoutAlignment.Center, LayoutAlignment.End, 50);
            await Show();

            // Center / Right Offset -50 -50
            _dialogVM.SetLayout(LayoutAlignment.Center, LayoutAlignment.End, -50);
            await Show();

            // Bottom / Left Offset 0 0
            _dialogVM.SetLayout(LayoutAlignment.End, LayoutAlignment.Start, 0);
            await Show();

            // Bottom / Left Offset 50 50
            _dialogVM.SetLayout(LayoutAlignment.End, LayoutAlignment.Start, 50);
            await Show();

            // Bottom / Left Offset -50 -50
            _dialogVM.SetLayout(LayoutAlignment.End, LayoutAlignment.Start, -50);
            await Show();

            // Bottom / Center Offset 0 0
            _dialogVM.SetLayout(LayoutAlignment.End, LayoutAlignment.Center, 0);
            await Show();

            // Bottom / Center Offset 50 50
            _dialogVM.SetLayout(LayoutAlignment.End, LayoutAlignment.Center, 50);
            await Show();

            // Bottom / Center Offset -50 -50
            _dialogVM.SetLayout(LayoutAlignment.End, LayoutAlignment.Center, -50);
            await Show();

            // Bottom / Right Offset 0 0
            _dialogVM.SetLayout(LayoutAlignment.End, LayoutAlignment.End, 0);
            await Show();

            // Bottom / Right Offset 50 50
            _dialogVM.SetLayout(LayoutAlignment.End, LayoutAlignment.End, 50);
            await Show();

            // Bottom / Right Offset -50 -50
            _dialogVM.SetLayout(LayoutAlignment.End, LayoutAlignment.End, -50);
            await Show();

            _dialogVM.Reset();
        }

        async Task CornerTest()
        {
            if (!CheckCorner.Value) return;

            _dialogVM.CornerRadius = 15;
            _dialogVM.Message = "Rounded Corner On";
            await Show();

            _dialogVM.Reset();
        }

        void SetShowAction(string member)
        {
            if(member == nameof(ToastTest))
            {
                Show = async () =>
                {
                    Toast.Instance.Show<ManualToastView>(_dialogVM);
                    await Task.Delay(_dialogVM.Duration + 250);
                };
            }
            else if(member == nameof(DialogTest))
            {
                Show = async () =>
                {
                    await Dialog.Instance.ShowAsync<ManualDialogView>(_dialogVM);
                };
            }
            else if(member == nameof(LoadingTest))
            {
                Show = async () =>
                {
                    var loading = Loading.Instance.Create<ManualLoadingView>(_dialogVM);
                    await loading.StartAsync(async _ =>
                    {
                        await Task.Delay(1000);
                    });
                };
            }
            else if(member == "LoadingDefault")
            {
                Show = async () =>
                {
                    await Loading.Instance.StartAsync(async _ =>
                    {
                        await Task.Delay(4000);
                    },_dialogVM.Message);
                };
            }
        }


    }
}
