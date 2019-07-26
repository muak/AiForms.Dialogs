using System;
using System.Threading.Tasks;
using AiForms.Dialogs.Abstractions;
using Android.App;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using XF = Xamarin.Forms;

namespace AiForms.Dialogs
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class ReusableLoading: LoadingBase,IReusableLoading
    {
        IVisualElementRenderer _renderer;
        LoadingView _loadingView;

        public ReusableLoading(LoadingView loadingView,LoadingPlatformDialog loadingDialog):base(loadingDialog)
        {
            _loadingView = loadingView;

            OnceInitializeAction = Initialize;
        }

        public override void Dispose()
        {
            _loadingView.Destroy();
            _loadingView.BindingContext = null;
            _loadingView.Parent = null;
            _loadingView = null;


            if (_renderer != null)
            {
                if (!_renderer.View.IsDisposed())
                {
                    _renderer.View.Dispose();
                }
                _renderer?.Dispose();
                _renderer = null;
            }

            base.Dispose();
        }

        public void Show(bool isCurrentScope = false)
        {
            if (IsRunning()) return;

            ShowInner();
        }

        public async Task StartAsync(Func<IProgress<double>, Task> action, bool isCurrentScope = false)
        {
            await WaitDialogDestroy();

            ShowInner();
            Progress = new Progress<double>();
            Progress.ProgressChanged += ProgressAction;
            await action(Progress);
            Hide();
        }

        void ShowInner()
        {
            OnceInitializeAction?.Invoke();

            var payload = new LoadingDialogPayload(ContentView, _loadingView);

            var bundle = new Bundle();
            bundle.PutSerializable(LoadingDialogPayload.PayloadKey, payload);
            PlatformDialog.Arguments = bundle;

            PlatformDialog.Show(FragmentManager, LoadingImplementation.LoadingDialogTag);
        }

        public void Hide()
        {
            if (Progress != null)
            {
                Progress.ProgressChanged -= ProgressAction;
                Progress = null;
                //_loadingView.Progress = 0d;
            }

            var anim = new AlphaAnimation(ContentView.Alpha, 0.0f);
            anim.Duration = 250;
            anim.FillAfter = true;
            ContentView.StartAnimation(anim);

            _loadingView?.RunDismissalAnimation();

            Task.Run(async () =>
            {
                // Wait a bit for ensuring that the dialog is created. 
                // Because it sometimes crashes or freezes when executing a very short process.
                await Task.Delay(50);
                var dialog = FragmentManager.FindFragmentByTag<LoadingPlatformDialog>(LoadingImplementation.LoadingDialogTag);
                dialog?.Dismiss();
                ContentView.RemoveFromParent();
            });
        }

        void ProgressAction(object sender, double progress)
        {
            _loadingView.Progress = progress;
        }

        void Initialize()
        {
            OnceInitializeAction = null;

            ContentView.SetBackgroundColor(_loadingView.OverlayColor.ToAndroid());
            ContentView.Alpha = 1f;
            ContentView.SetPadding(0, (int)Dialogs.Context.ToPixels(24), 0, 0); // Statusbar

            _loadingView.Parent = XF.Application.Current.MainPage;

            _renderer = Dialogs.CreateNativeView(_loadingView);

            if (_loadingView.CornerRadius > 0)
            {
                var nativeView = _renderer.View as ViewGroup;
                var border = new GradientDrawable();
                border.SetCornerRadius(Dialogs.Context.ToPixels(_loadingView.CornerRadius));
                if (!_loadingView.BackgroundColor.IsDefault)
                {
                    border.SetColor(_loadingView.BackgroundColor.ToAndroid());
                }
                nativeView.ClipToOutline = true;
                nativeView.SetBackground(border);
            }

            var measure = Dialogs.Measure(_loadingView);

            _loadingView.Layout(new XF.Rectangle(0, 0, measure.Width, measure.Height));


            var width = (int)Dialogs.Context.ToPixels(_loadingView.Bounds.Width);
            var height = (int)Dialogs.Context.ToPixels(_loadingView.Bounds.Height);

            using (var param = new FrameLayout.LayoutParams(
                    ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent
                )
            {
                Width = width,
                Height = height,
                Gravity = Dialogs.GetGravity(_loadingView)
            })
            {
                Dialogs.SetOffsetMargin(param, _loadingView);
                ContentView.AddView(_renderer.View, 0, param);
            }
        }
    }
}
