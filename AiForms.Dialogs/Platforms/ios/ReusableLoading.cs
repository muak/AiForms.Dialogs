using System;
using System.Threading.Tasks;
using AiForms.Dialogs.Abstractions;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Collections.Generic;

namespace AiForms.Dialogs
{
    [Foundation.Preserve(AllMembers = true)]
    public class ReusableLoading: LoadingBase,IReusableLoading
    {
        LoadingView _loadingView;
        IVisualElementRenderer _renderer;

        public ReusableLoading(LoadingView loadingView)
        {
            _loadingView = loadingView;

            OnceInitializeAction = Initialize;
        }

        public override void Dispose()
        {
            _loadingView.Parent = null;
            Dialogs.DisposeModelAndChildrenRenderers(_loadingView);
            _renderer = null;
        
            _loadingView.Destroy();
            _loadingView.BindingContext = null;
            _loadingView = null;

            base.Dispose();
        }

        public async Task StartAsync(Func<IProgress<double>, Task> action, bool isCurrentScope = false)
        {
            if(!await WaitDismiss())
            {
                return;
            }

            ShowInner(isCurrentScope);
            Progress = new Progress<double>();
            Progress.ProgressChanged += ProgressAction;
            await action(Progress);
            await Hide();
        }

        public void Show(bool isCurrentScope = false)
        {
            if(IsRunning) {
                return;
            }

            ShowInner(isCurrentScope);
        }

        void ShowInner(bool isCurrentScope)
        {
            IsRunning = true;

            OnceInitializeAction?.Invoke(isCurrentScope);

            if (IsCurrentScope.HasValue && IsCurrentScope != isCurrentScope)
            {
                SetOverlayConstrants(isCurrentScope);
            }

            IsCurrentScope = isCurrentScope;

            _loadingView.RunPresentationAnimation();

            UIView.Animate(0.25, () => OverlayView.Alpha = 1f, () => { });
        }

        public async Task Hide()
        {

            if (Progress != null)
            {
                Progress.ProgressChanged -= ProgressAction;
                Progress = null;
                if (_loadingView != null)
                {
                    //_loadingView.Progress = 0d;
                }
            }

            await UIView.AnimateAsync(
                0.25, // duration
                () => { OverlayView.Alpha = 0; }
            );

            _loadingView?.RunDismissalAnimation();
            IsRunning = false;
        }


        void ProgressAction(object sender, double progress)
        {
            if (_loadingView != null)
            {
                _loadingView.Progress = progress;
            }
        }

        void Initialize(bool isCurrentScope = false)
        {
            OnceInitializeAction = null;

            OverlayView.BackgroundColor = _loadingView.OverlayColor.ToUIColor();
            OverlayView.Alpha = 0f;
            OverlayView.TranslatesAutoresizingMaskIntoConstraints = false;

            SetOverlayConstrants(isCurrentScope);

            _loadingView.Parent = Application.Current.MainPage;

            _renderer = Dialogs.CreateNativeView(_loadingView);

            if (_loadingView.CornerRadius > 0)
            {
                _renderer.NativeView.Layer.CornerRadius = _loadingView.CornerRadius;
                _renderer.NativeView.Layer.MasksToBounds = true;
            }
            if (_loadingView.BorderWidth > 0)
            {
                _renderer.NativeView.Layer.BorderWidth = (float)_loadingView.BorderWidth;
                _renderer.NativeView.Layer.BorderColor = _loadingView.BorderColor.ToCGColor();
            }

            var measure = Dialogs.Measure(_loadingView);
            _renderer.SetElementSize(measure);

            var nativeView = _renderer.NativeView;

            nativeView.TranslatesAutoresizingMaskIntoConstraints = false;

            OverlayView.AddSubview(nativeView);

            nativeView.WidthAnchor.ConstraintEqualTo((System.nfloat)_loadingView.Bounds.Width).Active = true;
            nativeView.HeightAnchor.ConstraintEqualTo((System.nfloat)_loadingView.Bounds.Height).Active = true;

            Dialogs.SetLayoutAlignment(nativeView, OverlayView, _loadingView);
        }
    }
}
