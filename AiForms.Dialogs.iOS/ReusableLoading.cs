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
    public class ReusableLoading: IDisposable
    {
        public bool IsRunning { get; private set; }
        LoadingConfig _config => Configurations.LoadingConfig;
        UIViewController _viewController => Dialogs.RootViewController;
        LoadingView _loadingView;
        IVisualElementRenderer _renderer;
        Action<bool> OnceInitializeAction;
        UIView _overlayView;
        UIActivityIndicatorView _activitySpinner;
        UILabel _messageLabel;
        Progress<double> _progress;
        string _message;
        bool? _isCurrentScope = null;
        List<NSLayoutConstraint> _overlayConstraints = new List<NSLayoutConstraint>();

        Color _overlayColor
        {
            get
            {
                if (!_config.IsRegisteredView) return _config.OverlayColor;

                return _loadingView.OverlayColor.IsDefault ?
                                   _config.OverlayColor :
                                   _loadingView.OverlayColor;
            }
        }

        public ReusableLoading()
        {
            if (_config.IsRegisteredView)
            {
                _loadingView = _loadingView ?? _config.ViewResolver();
            }

            OnceInitializeAction = Initialize;
        }

        public void Dispose()
        {
            if (_loadingView != null)
            {
                _loadingView.Destroy();
                _loadingView.Parent = null;
                Dialogs.DisposeModelAndChildrenRenderers(_loadingView);
                _loadingView = null;
                _renderer = null;
            }

            _messageLabel?.RemoveFromSuperview();
            _messageLabel?.Dispose();
            _messageLabel = null;

            _activitySpinner?.RemoveFromSuperview();
            _activitySpinner?.Dispose();
            _activitySpinner = null;

            NSLayoutConstraint.DeactivateConstraints(_overlayConstraints.ToArray());
            _overlayConstraints.Clear();
            _overlayConstraints = null;

            _overlayView.RemoveFromSuperview();
            _overlayView.Dispose();
            _overlayView = null;

            _progress = null;
            _isCurrentScope = null;

            OnceInitializeAction = null;
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
            IsRunning = true;

            _message = message ?? _config.DefaultMessage;

            OnceInitializeAction?.Invoke(isCurrentScope);

            if(_isCurrentScope.HasValue && _isCurrentScope != isCurrentScope)
            {
                SetOverlayConstrants(isCurrentScope);
            }

            _isCurrentScope = isCurrentScope;

            _activitySpinner?.StartAnimating();
            _loadingView?.RunPresentationAnimation();

            UIView.Animate(0.25, () => _overlayView.Alpha = (float)_config.Opacity, () => { });
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

            UIView.Animate(
                0.25, // duration
                () => { _overlayView.Alpha = 0; },
                () =>
                {
                    _activitySpinner?.StopAnimating();
                    _loadingView?.RunDismissalAnimation();
                    IsRunning = false;
                }
            );
        }

        public void SetMessage(string message)
        {
            SetMessageInner(message);
        }

        void SetMessageInner(string message, double progress = -1)
        {
            _message = message ?? _config.DefaultMessage;

            _viewController.BeginInvokeOnMainThread(() =>
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

        void ProgressAction(object sender, double progress)
        {
            SetMessageInner(_message, progress);
            if (_loadingView != null)
            {
                _loadingView.Progress = progress;
            }
        }

        void Initialize(bool isCurrentScope = false)
        {
            OnceInitializeAction = null;

            _overlayView = new UIView();
            _overlayView.BackgroundColor = _overlayColor.ToUIColor();
            _overlayView.Alpha = 0f;
            _overlayView.TranslatesAutoresizingMaskIntoConstraints = false;

            SetOverlayConstrants(isCurrentScope);

            if (!_config.IsRegisteredView)
            {
                _activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
                _activitySpinner.Color = _config.IndicatorColor.ToUIColor();
                _activitySpinner.TranslatesAutoresizingMaskIntoConstraints = false;

                _overlayView.AddSubview(_activitySpinner);

                _activitySpinner.CenterXAnchor.ConstraintEqualTo(_overlayView.CenterXAnchor, _config.OffsetX).Active = true;
                _activitySpinner.CenterYAnchor.ConstraintEqualTo(_overlayView.CenterYAnchor, _config.OffsetY).Active = true;

                _messageLabel = new UILabel();
                _messageLabel.BackgroundColor = UIColor.Clear;
                _messageLabel.TextColor = _config.FontColor.ToUIColor();
                _messageLabel.Font = _messageLabel.Font.WithSize((System.nfloat)_config.FontSize);
                _messageLabel.TextAlignment = UITextAlignment.Center;
                _messageLabel.Lines = 0;
                _messageLabel.LineBreakMode = UILineBreakMode.WordWrap;
                _messageLabel.TranslatesAutoresizingMaskIntoConstraints = false;
                _messageLabel.Text = _message;

                _overlayView.AddSubview(_messageLabel);

                _messageLabel.TopAnchor.ConstraintEqualTo(_activitySpinner.BottomAnchor, 20).Active = true;
                _messageLabel.CenterXAnchor.ConstraintEqualTo(_overlayView.CenterXAnchor, _config.OffsetX).Active = true;

                return;
            }

            _loadingView.Parent = Application.Current.MainPage;

            _renderer = Dialogs.CreateNativeView(_loadingView);

            if (_loadingView.CornerRadius > 0)
            {
                _renderer.NativeView.Layer.CornerRadius = _loadingView.CornerRadius;
                _renderer.NativeView.Layer.MasksToBounds = true;
            }

            var measure = Dialogs.Measure(_loadingView);
            _renderer.SetElementSize(measure);

            var nativeView = _renderer.NativeView;

            nativeView.TranslatesAutoresizingMaskIntoConstraints = false;

            _overlayView.AddSubview(nativeView);

            nativeView.WidthAnchor.ConstraintEqualTo((System.nfloat)_loadingView.Bounds.Width).Active = true;
            nativeView.HeightAnchor.ConstraintEqualTo((System.nfloat)_loadingView.Bounds.Height).Active = true;
            nativeView.CenterXAnchor.ConstraintEqualTo(_overlayView.CenterXAnchor, _loadingView.OffsetX).Active = true;
            nativeView.CenterYAnchor.ConstraintEqualTo(_overlayView.CenterYAnchor, _loadingView.OffsetY).Active = true;



        }

        void SetOverlayConstrants(bool isCurrentScope)
        {
            _overlayView.RemoveFromSuperview();

            UIView parentView;
            if (isCurrentScope)
            {
                _viewController.View.AddSubview(_overlayView);
                parentView = _viewController.View;
            }
            else
            {
                var window = UIApplication.SharedApplication.KeyWindow;
                window.AddSubview(_overlayView);
                parentView = window;
            }

            NSLayoutConstraint.DeactivateConstraints(_overlayConstraints.ToArray());
            _overlayConstraints.Clear();

            _overlayConstraints.Add(_overlayView.TopAnchor.ConstraintEqualTo(parentView.TopAnchor));
            _overlayConstraints.Add(_overlayView.LeftAnchor.ConstraintEqualTo(parentView.LeftAnchor));
            _overlayConstraints.Add(_overlayView.RightAnchor.ConstraintEqualTo(parentView.RightAnchor));
            _overlayConstraints.Add(_overlayView.BottomAnchor.ConstraintEqualTo(parentView.BottomAnchor));

            NSLayoutConstraint.ActivateConstraints(_overlayConstraints.ToArray());
        }
    }
}
