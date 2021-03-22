using System;
using System.Threading.Tasks;
using AiForms.Dialogs.Abstractions;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace AiForms.Dialogs
{
    public class DefaultLoading:LoadingBase
    {
        LoadingConfig _config => Configurations.LoadingConfig;
        UILabel _messageLabel;
        UIActivityIndicatorView _activitySpinner;
        string _message;

        public DefaultLoading()
        {
            OnceInitializeAction = Initialize;
        }

        public override void Dispose()
        {
            _messageLabel?.RemoveFromSuperview();
            _messageLabel?.Dispose();
            _messageLabel = null;

            _activitySpinner?.RemoveFromSuperview();
            _activitySpinner?.Dispose();
            _activitySpinner = null;

            base.Dispose();
        }

        public async Task StartAsync(Func<IProgress<double>, Task> action, string message = null, bool isCurrentScope = false)
        {
            if(!await WaitDismiss())
            {
                return;
            }

            Show(message, isCurrentScope);
            Progress = new Progress<double>();
            Progress.ProgressChanged += ProgressAction;
            await action(Progress);
        }

        public void Show(string message = null, bool isCurrentScope = false)
        {
            IsRunning = true;

            _message = message ?? _config.DefaultMessage;

            OnceInitializeAction?.Invoke(isCurrentScope);

            if (IsCurrentScope.HasValue && IsCurrentScope != isCurrentScope)
            {
                SetOverlayConstrants(isCurrentScope);
            }

            IsCurrentScope = isCurrentScope;

            _activitySpinner?.StartAnimating();

            UIView.Animate(0.25, () => OverlayView.Alpha = (float)_config.Opacity, () => { });
        }

        public void Hide()
        {

            if (Progress != null)
            {
                Progress.ProgressChanged -= ProgressAction;
                Progress = null;
            }

            UIView.Animate(
                0.25, // duration
                () => { OverlayView.Alpha = 0; },
                () =>
                {
                    _activitySpinner?.StopAnimating();
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

            ViewController.BeginInvokeOnMainThread(() =>
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
        }

        void Initialize(bool isCurrentScope = false)
        {
            OnceInitializeAction = null;


            OverlayView.BackgroundColor = _config.OverlayColor.ToUIColor();
            OverlayView.Alpha = 0f;
            OverlayView.TranslatesAutoresizingMaskIntoConstraints = false;

            SetOverlayConstrants(isCurrentScope);

            _activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
            _activitySpinner.Color = _config.IndicatorColor.ToUIColor();
            _activitySpinner.TranslatesAutoresizingMaskIntoConstraints = false;

            OverlayView.AddSubview(_activitySpinner);

            _activitySpinner.CenterXAnchor.ConstraintEqualTo(OverlayView.CenterXAnchor, _config.OffsetX).Active = true;
            _activitySpinner.CenterYAnchor.ConstraintEqualTo(OverlayView.CenterYAnchor, _config.OffsetY).Active = true;

            _messageLabel = new UILabel();
            _messageLabel.BackgroundColor = UIColor.Clear;
            _messageLabel.TextColor = _config.FontColor.ToUIColor();
            _messageLabel.Font = _messageLabel.Font.WithSize((System.nfloat)_config.FontSize);
            _messageLabel.TextAlignment = UITextAlignment.Center;
            _messageLabel.Lines = 0;
            _messageLabel.LineBreakMode = UILineBreakMode.WordWrap;
            _messageLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            _messageLabel.Text = _message;

            OverlayView.AddSubview(_messageLabel);

            _messageLabel.TopAnchor.ConstraintEqualTo(_activitySpinner.BottomAnchor, 20).Active = true;
            _messageLabel.CenterXAnchor.ConstraintEqualTo(OverlayView.CenterXAnchor, _config.OffsetX).Active = true;
            
        }
    }
}
