using System;
using AiForms.Extras.Abstractions;
using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Views;
using Xamarin.Forms.Platform.Android;
using Android.Views.Animations;
using XF = Xamarin.Forms;

namespace AiForms.Extras
{
    public class LoadingImplementation:ILoading,IDisposable
    {
        LoadingDialog _loadingDialog;
        FragmentManager _fragmentManager;
        Progress<double> _progress;
        public static readonly string LoadingDialogTag = "LoadingDialog";

        public LoadingImplementation()
        {
            _loadingDialog = new LoadingDialog();
            _fragmentManager = (Extras.Context as Activity)?.FragmentManager;
        }

        public void Dispose()
        {
            if (_progress != null)
            {
                _progress.ProgressChanged -= ProgressAction;
                _progress = null;
            }
            var dialog = _fragmentManager.FindFragmentByTag<LoadingDialog>(LoadingDialogTag);
            dialog?.Dismiss();
        }

        public IDisposable Start(string message = null,bool isCurrentScope = false)
        {
            Show(message);
            return this;
        }

        public IDisposable Start(out IProgress<double> progress, string message = null,bool isCurrentScope = false)
        {
            Show(out progress, message);
            return this;
        }

        public void Show(string message = null,bool isCurrentScope = false)
        {
            if(IsRunning()){
                return;
            }

            var bundle = new Bundle();
            bundle.PutString("message",message);
            _loadingDialog.Arguments = bundle;
            _loadingDialog.Show(_fragmentManager, LoadingDialogTag);
        }

        public void Show(out IProgress<double> progress, string message = null,bool isCurrentScope = false)
        {
            _progress = new Progress<double>();
            progress = _progress;

            if (IsRunning())
            {
                return;
            }

            _progress.ProgressChanged += ProgressAction;
            Show(message);
        }

        bool IsRunning()
        {
            var dialog = _fragmentManager.FindFragmentByTag<LoadingDialog>(LoadingDialogTag);
            return dialog != null;
        }

        public void Hide()
        {
            Dispose();
        }

        public void SetMessage(string message)
        {
            _loadingDialog?.SetMessage(message);
        }

        void ProgressAction(object sender, double e)
        {
            _loadingDialog?.ProgressAction(e);
        }
    }

    public class LoadingDialog:DialogFragment
    {
        string _message;
        LoadingConfig _config => Configurations.LoadingConfig;
        TextView _messageLabel;
        RelativeLayout _contentView;
        static IVisualElementRenderer _indicatorCache;

        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            base.OnCreateDialog(savedInstanceState);

            _message = Arguments.GetString("message");

            _contentView = (Extras.Context as Activity).LayoutInflater.Inflate(Resource.Layout.LoadingDialogLayout,null) as RelativeLayout;
            _contentView.SetBackgroundColor(_config.OverlayColor.ToAndroid());
            _contentView.Alpha = (float)_config.Opacity;

            var progress = _contentView.FindViewById<ProgressBar>(Resource.Id.progress);

            _messageLabel = _contentView.FindViewById<TextView>(Resource.Id.loading_message);
            _messageLabel.SetTextSize(Android.Util.ComplexUnitType.Sp, (float)_config.FontSize);
            _messageLabel.SetTextColor(_config.FontColor.ToAndroid());
            _messageLabel.Text = _message ?? _config.DefaultMessage;


            if(_config.IndicatorView == null)
            {
                progress.IndeterminateDrawable.SetColorFilter(_config.IndicatorColor.ToAndroid(), PorterDuff.Mode.SrcIn);

            }
            else{
                XF.Layout.LayoutChildIntoBoundingRegion(_config.IndicatorView, new XF.Rectangle(0, 0, _config.IndicatorViewWidth, _config.IndicatorViewHeight));
                _indicatorCache = _indicatorCache ?? Extras.CreateNativeView(_config.IndicatorView);

                progress.RemoveFromParent();
                var inner = _contentView.FindViewById<RelativeLayout>(Resource.Id.loading_inner_container);

                using (var param = new RelativeLayout.LayoutParams(
                    ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent
                )
                {
                    Width = (int)Extras.Context.ToPixels(_config.IndicatorViewWidth),
                    Height = (int)Extras.Context.ToPixels(_config.IndicatorViewHeight)
                }){
                    param.AddRule(LayoutRules.CenterInParent);
                    _indicatorCache.View.Id = View.GenerateViewId();
                    inner.AddView(_indicatorCache.View,0,param);
                }

                var textParams = _messageLabel.LayoutParameters as RelativeLayout.LayoutParams;
                textParams.RemoveRule(LayoutRules.Below);
                textParams.AddRule(LayoutRules.Below, _indicatorCache.View.Id);
                (_indicatorCache.Element as IIndicatorView)?.StartAnimating();
            }

            //var inner = _contentView.FindViewById<LinearLayout>(Resource.Id.loading_inner_container);

            var padding = CalcOffsetToPadding();

            _contentView.SetPadding(padding.left,padding.top,padding.right,padding.bottom);

            var dialog = Extras.CreateFullScreenTransparentDialog(_contentView);

            Cancelable = false;
            dialog.SetCancelable(false);
            dialog.SetCanceledOnTouchOutside(false);

            return dialog;
        }

        (int left,int top,int right, int bottom) CalcOffsetToPadding()
        {
            int left = 0;
            int right = 0;
            int top = 0;
            int bottom = 0;
            var padLR = (int)Math.Abs(Extras.Context.ToPixels(_config.OffsetX * 2));
            var padTB = (int)Math.Abs(Extras.Context.ToPixels(_config.OffsetY * 2));

            if(_config.OffsetX < 0) {
                right = padLR;
            }
            else{
                left = padLR;
            }

            if(_config.OffsetY < 0){
                bottom = padTB;
            }
            else{
                top = padTB;
            }

            return (left, top, right, bottom);
        }

        public override void Dismiss()
        {       
            var anim = new AlphaAnimation(_contentView.Alpha, 0.0f);
            anim.Duration = 500;
            anim.FillAfter = true;
            _contentView.StartAnimation(anim);

            base.Dismiss();

            (_indicatorCache?.Element as IIndicatorView)?.StopAnimating();
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            _indicatorCache?.View.RemoveFromParent();
            _messageLabel = null;
            _contentView = null;
        }

        public void SetMessage(string message, double progress = -1d)
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

        public void ProgressAction(double progress)
        {
            SetMessage(_message, progress);
        }
    }
}
