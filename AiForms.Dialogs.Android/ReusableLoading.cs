using System;
using System.Threading.Tasks;
using AiForms.Dialogs.Abstractions;
using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Java.IO;
using Xamarin.Forms.Platform.Android;
using XF = Xamarin.Forms;

namespace AiForms.Dialogs
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class ReusableLoading: IDisposable
    {
        FragmentManager FragmentManager => Dialogs.FragmentManager;
        LoadingImplementation _loadingImpl;
        LoadingPlatformDialog _loadingDialog => _loadingImpl.LoadingDialog;
        LoadingConfig _config => Configurations.LoadingConfig;

        IVisualElementRenderer _renderer;
        ViewGroup _contentView;

        Action OnceInitializeAction;
        LoadingView _loadingView;
        bool _isCustom => _loadingView != null;
        string _message;

        Progress<double> _progress;
        TextView _messageLabel;

        Color _overlayColor {
            get {
                if (!_config.IsRegisteredView) return _config.OverlayColor.ToAndroid();

                return _loadingView.OverlayColor.IsDefault ?
                                   _config.OverlayColor.ToAndroid() :
                                   _loadingView.OverlayColor.ToAndroid();
            }
        }

        public ReusableLoading()
        {
            _loadingImpl = Loading.Instance as LoadingImplementation;

            if(_config.IsRegisteredView)
            {
                _loadingView = _loadingView ?? _config.ViewResolver();
            }

            OnceInitializeAction = Initialize;
        }

        public void Dispose()
        {
            _loadingImpl = null;

            if(_loadingView != null)
            {
                _loadingView.Destroy();
                _loadingView.Parent = null;
                _loadingView = null;
            }

            if(_renderer != null)
            {
                if (!_renderer.View.IsDisposed())
                {
                    _renderer.View.Dispose();
                }
                _renderer?.Dispose();
                _renderer = null;
            }

            _contentView.Dispose();
            _contentView = null;

           
            OnceInitializeAction = null;
            _messageLabel?.Dispose();
            _messageLabel = null;
        }

        public async Task StartAsync(Func<IProgress<double>, Task> action, string message = null, bool isCurrentScope = false)
        {
            Show(message, isCurrentScope);
            _progress = new Progress<double>();
            _progress.ProgressChanged += ProgressAction;
            await action(_progress);
        }

        public void Show(string message = null, bool isCurrentScope = false)
        {
            OnceInitializeAction?.Invoke();

            var payload = new LoadingDialogPayload(_loadingView, _contentView);

            var bundle = new Bundle();
            bundle.PutSerializable("loadingDialogPayload", payload);
            _loadingDialog.Arguments = bundle;

            _message = message ?? _config.DefaultMessage;
            if(_messageLabel != null)
            {
                _messageLabel.Text = _message;
            }

            _loadingDialog.Show(FragmentManager, LoadingImplementation.LoadingDialogTag);
        }

        public void Hide()
        {
            if (_progress != null)
            {
                _progress.ProgressChanged -= ProgressAction;
                _progress = null;
                if (_loadingView != null)
                {
                    _loadingView.Progress = 0d;
                }
            }

            var anim = new AlphaAnimation(_contentView.Alpha, 0.0f);
            anim.Duration = 500;
            anim.FillAfter = true;
            _contentView.StartAnimation(anim);

            _loadingView?.RunDismissalAnimation();

            var dialog = FragmentManager.FindFragmentByTag<LoadingPlatformDialog>(LoadingImplementation.LoadingDialogTag);
            dialog?.Dismiss();
            _contentView.RemoveFromParent();
        }

        void ProgressAction(object sender, double progress)
        {
            SetMessageInner(_message, progress);
            if(_loadingView != null)
            {
                _loadingView.Progress = progress;
            }
        }

        public void SetMessage(string message)
        {
            SetMessageInner(message);
        }

        void SetMessageInner(string message ,double progress = -1)
        {
            _message = message ?? _config.DefaultMessage;

            XF.Device.BeginInvokeOnMainThread(() =>
            {
                if (_messageLabel == null)
                {
                    return;
                }


                if (progress >= 0)
                {
                    _messageLabel.Text = string.Format(_config.ProgressMessageFormat, _message, progress);
                }
                else
                {
                    _messageLabel.Text = _message;
                }
            });
        }

        void Initialize()
        {
            OnceInitializeAction = null;

            _contentView = new FrameLayout(Dialogs.Context);
            using (var param = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent))
            {
                _contentView.LayoutParameters = param;
            }

            _contentView.SetBackgroundColor(_overlayColor);
            _contentView.SetClipChildren(false);
            _contentView.SetClipToPadding(false);
            _contentView.Alpha = (float)_config.Opacity;

            if (!_config.IsRegisteredView)
            {
                var innerView = (Dialogs.Context as Activity).LayoutInflater.Inflate(Resource.Layout.LoadingDialogLayout, null);

                var progress = innerView.FindViewById<ProgressBar>(Resource.Id.progress);
                _messageLabel = innerView.FindViewById<TextView>(Resource.Id.loading_message);

                progress.IndeterminateDrawable.SetColorFilter(_config.IndicatorColor.ToAndroid(), PorterDuff.Mode.SrcIn);
                _messageLabel.SetTextSize(Android.Util.ComplexUnitType.Sp, (float)_config.FontSize);
                _messageLabel.SetTextColor(_config.FontColor.ToAndroid());

                using (var param = new FrameLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
                {
                    Gravity = GravityFlags.Center,
                })
                {
                    Dialogs.SetOffsetMargin(param, _config.OffsetX,_config.OffsetY);
                    _contentView.AddView(innerView, 0, param);
                };

                return;
            }

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
                _contentView.AddView(_renderer.View, 0, param);
            }
        }
    }

    [Android.Runtime.Preserve(AllMembers = true)]
    public class LoadingDialogPayload : Java.Lang.Object, ISerializable
    {
        public LoadingView LoadingView { get; set; }
        public ViewGroup ContentView { get; set; }

        public LoadingDialogPayload(LoadingView loadingView, ViewGroup contentView)
        {
            LoadingView = loadingView;
            ContentView = contentView;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                LoadingView = null;
                ContentView = null;
            }
            base.Dispose(disposing);
        }
    }
}
