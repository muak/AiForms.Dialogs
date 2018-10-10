using System;
using System.Threading.Tasks;
using AiForms.Extras.Abstractions;
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

namespace AiForms.Extras
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class ReusableLoading: IDisposable
    {
        FragmentManager FragmentManager => Extras.FragmentManager;
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
            _messageLabel.Text = _message;

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

            _contentView = (Extras.Context as Activity).LayoutInflater.Inflate(Resource.Layout.LoadingDialogLayout, null) as RelativeLayout;
            _contentView.SetBackgroundColor(_overlayColor);
            _contentView.Alpha = (float)_config.Opacity;

            var progress = _contentView.FindViewById<ProgressBar>(Resource.Id.progress);
            _messageLabel = _contentView.FindViewById<TextView>(Resource.Id.loading_message);

            var padding = CalcOffsetToPadding();
            _contentView.SetPadding(padding.left, padding.top, padding.right, padding.bottom);

            if (!_config.IsRegisteredView)
            {
                progress.IndeterminateDrawable.SetColorFilter(_config.IndicatorColor.ToAndroid(), PorterDuff.Mode.SrcIn);
                _messageLabel.SetTextSize(Android.Util.ComplexUnitType.Sp, (float)_config.FontSize);
                _messageLabel.SetTextColor(_config.FontColor.ToAndroid());
                return;
            }

            _loadingView.Parent = XF.Application.Current.MainPage;

            _renderer = Extras.CreateNativeView(_loadingView);

            if (_loadingView.CornerRadius > 0)
            {
                var nativeView = _renderer.View as ViewGroup;
                var border = new GradientDrawable();
                border.SetCornerRadius(Extras.Context.ToPixels(_loadingView.CornerRadius));
                if (!_loadingView.BackgroundColor.IsDefault)
                {
                    border.SetColor(_loadingView.BackgroundColor.ToAndroid());
                }
                nativeView.ClipToOutline = true;
                nativeView.SetBackground(border);
            }

            var measure = Extras.Measure(_loadingView);

            _loadingView.Layout(new XF.Rectangle(0, 0, measure.Width, measure.Height));


            progress.RemoveFromParent();
            _messageLabel.RemoveFromParent();

            var inner = _contentView.FindViewById<RelativeLayout>(Resource.Id.loading_inner_container);

            var width = (int)Extras.Context.ToPixels(_loadingView.Bounds.Width);
            var height = (int)Extras.Context.ToPixels(_loadingView.Bounds.Height);

            using (var param = new RelativeLayout.LayoutParams(
                    ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent
                )
            {
                Width = width,
                Height = height
            })
            {
                param.AddRule(LayoutRules.CenterInParent);
                inner.AddView(_renderer.View, 0, param);
            }
        }

        (int left, int top, int right, int bottom) CalcOffsetToPadding()
        {
            var offsetX = _config.IsRegisteredView ? _loadingView.OffsetX : _config.OffsetX;
            var offsetY = _config.IsRegisteredView ? _loadingView.OffsetY : _config.OffsetY;

            int left = 0;
            int right = 0;
            int top = 0;
            int bottom = 0;
            var padLR = (int)Math.Abs(Extras.Context.ToPixels(offsetX * 2));
            var padTB = (int)Math.Abs(Extras.Context.ToPixels(offsetY * 2));

            if (_config.OffsetX < 0)
            {
                right = padLR;
            }
            else
            {
                left = padLR;
            }

            if (_config.OffsetY < 0)
            {
                bottom = padTB;
            }
            else
            {
                top = padTB;
            }

            return (left, top, right, bottom);
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
