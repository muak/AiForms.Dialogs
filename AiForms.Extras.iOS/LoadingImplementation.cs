using System;
using AiForms.Extras.Abstractions;
using UIKit;
using CoreGraphics;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace AiForms.Extras
{
    public class LoadingImplementation : ILoading, IDisposable
    {
        UIView _overlayView;
        LoadingConfig _config => Configurations.LoadingConfig;
        UIViewController _viewController => Extras.RootViewController;
        Progress<double> _progress;
        UIActivityIndicatorView _activitySpinner;
        string _message;
        UILabel _messageLabel;
        static IVisualElementRenderer _indicatorRenderer;

        public LoadingImplementation()
        {

        }

        public void Show(string message = null,bool isCurrentScope = false)
        {
            if (_overlayView != null)
            {
                return;
            }
            _message = message;

            SetView(isCurrentScope);
            _activitySpinner?.StartAnimating();
            (_indicatorRenderer?.Element as IIndicatorView)?.StartAnimating();
            //_viewController.BeginInvokeOnMainThread(() =>
            //{
            //    var bounds = UIScreen.MainScreen.Bounds;
            //    _overlayView = new LoadingOverlay(bounds, message, Configurations.LoadingConfig);
            //    _viewController.Add(_overlayView);
            //});           
        }

        public void Show(out IProgress<double> progress, string message = null,bool isCurrentScope = false)
        {
            _progress = new Progress<double>();
            _progress.ProgressChanged += ProgressAction;

            progress = _progress;

            Show(message,isCurrentScope);
        }

        public void Hide()
        {
            Dispose();
        }

        public void SetMessage(string message)
        {
            //_overlayView.SetMessage(message);

            SetMessageInternal(message);
        }

        void SetMessageInternal(string message, double progress = -1)
        {
            _message = message ?? _config.DefaultMessage;
            _viewController.BeginInvokeOnMainThread(() =>
            {
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

        void ProgressAction(object sender, double e)
        {
            //_overlayView?.ProgressAction(e);
            SetMessageInternal(_message, e);
        }

        public void Dispose()
        {
            if (_progress != null)
            {
                _progress.ProgressChanged -= ProgressAction;
                _progress = null;
            }
            //_overlayView?.Hide();
            //_overlayView = null;

            UIView.Animate(
                0.5, // duration
                () => { _overlayView.Alpha = 0; },
                () =>
                {
                    _activitySpinner?.StopAnimating();
                    (_indicatorRenderer?.Element as IIndicatorView)?.StopAnimating();

                    _messageLabel?.RemoveFromSuperview();
                    _indicatorRenderer?.NativeView?.RemoveFromSuperview();
                    _activitySpinner?.RemoveFromSuperview();
                    _overlayView.RemoveFromSuperview();
                    _messageLabel?.Dispose();
                    _activitySpinner?.Dispose();
                    _overlayView?.Dispose();
                    _activitySpinner = null;
                    _messageLabel = null;
                    _overlayView = null;
                }
            );
        }

        public IDisposable Start(string message = null,bool isCurrentScope = false)
        {
            Show(message,isCurrentScope);
            return this;
        }

        public IDisposable Start(out IProgress<double> progress, string message = null,bool isCurrentScope = false)
        {
            Show(out progress, message,isCurrentScope);
            return this;
        }


        void SetView(bool isCurrentScope = false)
        {
            _overlayView = new UIView();
            _overlayView.BackgroundColor = _config.OverlayColor.ToUIColor();
            _overlayView.Alpha = (System.nfloat)_config.Opacity;
            _overlayView.TranslatesAutoresizingMaskIntoConstraints = false;

            UIView parentView;
            if(isCurrentScope){
                _viewController.View.AddSubview(_overlayView);
                parentView = _viewController.View;
            }
            else{
                var window = UIApplication.SharedApplication.KeyWindow;
                window.AddSubview(_overlayView);
                parentView = window;
            }

            _overlayView.TopAnchor.ConstraintEqualTo(parentView.TopAnchor).Active = true;
            _overlayView.LeftAnchor.ConstraintEqualTo(parentView.LeftAnchor).Active = true;
            _overlayView.RightAnchor.ConstraintEqualTo(parentView.RightAnchor).Active = true;
            _overlayView.BottomAnchor.ConstraintEqualTo(parentView.BottomAnchor).Active = true;

            if (_config.IndicatorView == null)
            {
                _activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
                _activitySpinner.Color = _config.IndicatorColor.ToUIColor();
                _activitySpinner.TranslatesAutoresizingMaskIntoConstraints = false;

                _overlayView.AddSubview(_activitySpinner);

                _activitySpinner.CenterXAnchor.ConstraintEqualTo(_overlayView.CenterXAnchor, _config.OffsetX).Active = true;
                _activitySpinner.CenterYAnchor.ConstraintEqualTo(_overlayView.CenterYAnchor, _config.OffsetY).Active = true;

            }
            else
            {
                if (_indicatorRenderer == null)
                {
                    _indicatorRenderer = Extras.CreateNativeView(_config.IndicatorView);
                    _config.IndicatorView.Layout(new Rectangle(0, 0, _config.IndicatorViewWidth, _config.IndicatorViewHeight));
                }

                var nativeView = _indicatorRenderer.NativeView;

                nativeView.TranslatesAutoresizingMaskIntoConstraints = false;

                _overlayView.AddSubview(nativeView);

                nativeView.WidthAnchor.ConstraintEqualTo((System.nfloat)_config.IndicatorViewWidth).Active = true;
                nativeView.HeightAnchor.ConstraintEqualTo((System.nfloat)_config.IndicatorViewHeight).Active = true;
                nativeView.CenterXAnchor.ConstraintEqualTo(_overlayView.CenterXAnchor, _config.OffsetX).Active = true;
                nativeView.CenterYAnchor.ConstraintEqualTo(_overlayView.CenterYAnchor, _config.OffsetY).Active = true;
            }

            _messageLabel = new UILabel();
            _messageLabel.BackgroundColor = UIColor.Clear;
            _messageLabel.TextColor = _config.FontColor.ToUIColor();
            _messageLabel.Font = _messageLabel.Font.WithSize((System.nfloat)_config.FontSize);
            _messageLabel.Text = _message ?? _config.DefaultMessage;
            _messageLabel.TextAlignment = UITextAlignment.Center;
            _messageLabel.Lines = 0;
            _messageLabel.LineBreakMode = UILineBreakMode.WordWrap;
            _messageLabel.TranslatesAutoresizingMaskIntoConstraints = false;

            _overlayView.AddSubview(_messageLabel);

            var aboveView = _config.IndicatorView == null ? _activitySpinner : _indicatorRenderer.NativeView;
            _messageLabel.TopAnchor.ConstraintEqualTo(aboveView.BottomAnchor, 20).Active = true;
            _messageLabel.CenterXAnchor.ConstraintEqualTo(_overlayView.CenterXAnchor, _config.OffsetX).Active = true;
        }
    }

    public class LoadingOverlay : UIView
    {
        string _message;
        UILabel _messageLabel;
        LoadingConfig _config;
        UIActivityIndicatorView _activitySpinner;
        static IVisualElementRenderer _indicatorCache;

        public LoadingOverlay(CGRect frame, string message, LoadingConfig config) : base(frame)
        {
            _message = message;
            _config = config;

            BackgroundColor = config.OverlayColor.ToUIColor();
            Alpha = (System.nfloat)config.Opacity;
            AutoresizingMask = UIViewAutoresizing.All;

            nfloat labelWidth = Frame.Width - 20;

            nfloat centerX = Frame.Width / 2;
            nfloat centerY = Frame.Height / 2;
            nfloat labelY;

            if (config.IndicatorView == null)
            {
                labelY = centerY;

                _activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
                _activitySpinner.Color = config.IndicatorColor.ToUIColor();

                _activitySpinner.Frame = new CGRect(
                    centerX - _activitySpinner.Frame.Width / 2.0,
                    centerY - _activitySpinner.Frame.Height - 20,
                    _activitySpinner.Frame.Width,
                    _activitySpinner.Frame.Height);
                _activitySpinner.AutoresizingMask = UIViewAutoresizing.All;
                AddSubview(_activitySpinner);
                _activitySpinner.StartAnimating();
            }
            else
            {
                labelY = centerY + (System.nfloat)config.IndicatorViewHeight / 2.0f;
                _indicatorCache = _indicatorCache ?? CreateNativeIndicatorView(config.IndicatorView, centerX, centerY, config.IndicatorViewWidth, config.IndicatorViewHeight);
                AddSubview(_indicatorCache.NativeView);
                (_indicatorCache.Element as IIndicatorView)?.StartAnimating();
            }

            _messageLabel = new UILabel();
            _messageLabel.BackgroundColor = UIColor.Clear;
            _messageLabel.TextColor = config.FontColor.ToUIColor();
            _messageLabel.Font = _messageLabel.Font.WithSize((System.nfloat)config.FontSize);
            _messageLabel.Text = message ?? config.DefaultMessage;
            _messageLabel.TextAlignment = UITextAlignment.Center;
            _messageLabel.AutoresizingMask = UIViewAutoresizing.All;
            _messageLabel.Lines = 0;
            _messageLabel.LineBreakMode = UILineBreakMode.WordWrap;
            _messageLabel.SizeToFit();
            _messageLabel.Frame = new CGRect(
                centerX - labelWidth / 2.0,
                labelY,
                labelWidth,
                _messageLabel.Frame.Height * 2
            );
            AddSubview(_messageLabel);

        }

        public void SetMessage(string message, double progress = -1d)
        {
            _message = message ?? _config.DefaultMessage;
            BeginInvokeOnMainThread(() =>
            {
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

        public void Hide()
        {

            Animate(
                0.5, // duration
                () => { Alpha = 0; },
                () =>
                {
                    (_indicatorCache?.Element as IIndicatorView)?.StopAnimating();
                    RemoveFromSuperview();
                    _messageLabel?.RemoveFromSuperview();
                    _indicatorCache?.NativeView?.RemoveFromSuperview();
                    _activitySpinner?.RemoveFromSuperview();
                    _messageLabel?.Dispose();
                    _activitySpinner?.Dispose();
                    Dispose();
                }
            );
        }

        IVisualElementRenderer CreateNativeIndicatorView(ContentView view, nfloat centerX, nfloat centerY, double width, double height)
        {
            var newRenderer = Extras.CreateNativeView(view);

            newRenderer.NativeView.Frame = new CGRect(
                centerX - width / 2.0,
                centerY - height / 2.0 - 20,
                width,
                height);

            Layout.LayoutChildIntoBoundingRegion(view, newRenderer.NativeView.Frame.ToRectangle());

            return newRenderer;
        }
    }
}
