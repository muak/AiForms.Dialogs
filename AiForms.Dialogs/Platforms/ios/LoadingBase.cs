using System;
using System.Collections.Generic;
using UIKit;
using System.Threading.Tasks;

namespace AiForms.Dialogs
{
    [Foundation.Preserve(AllMembers = true)]
    public class LoadingBase:IDisposable
    {
        public bool IsRunning { get; protected set; }
        protected UIView OverlayView;
        protected UIViewController ViewController => Dialogs.RootViewController;
        protected Action<bool> OnceInitializeAction;
        protected Progress<double> Progress;
        protected bool? IsCurrentScope = null;

        List<NSLayoutConstraint> _overlayConstraints = new List<NSLayoutConstraint>();

      
        public LoadingBase()
        {
            OverlayView = new UIView();
        }

        public virtual void Dispose()
        {
            NSLayoutConstraint.DeactivateConstraints(_overlayConstraints.ToArray());
            _overlayConstraints.Clear();
            _overlayConstraints = null;

            OverlayView.RemoveFromSuperview();
            OverlayView.Dispose();
            OverlayView = null;

            Progress = null;
            IsCurrentScope = null;

            OnceInitializeAction = null;
        }

        protected void SetOverlayConstrants(bool isCurrentScope)
        {
            OverlayView.RemoveFromSuperview();

            UIView parentView;
            if (isCurrentScope)
            {
                ViewController.View.AddSubview(OverlayView);
                parentView = ViewController.View;
            }
            else
            {
                var window = UIApplication.SharedApplication.KeyWindow;
                window.AddSubview(OverlayView);
                parentView = window;
            }

            NSLayoutConstraint.DeactivateConstraints(_overlayConstraints.ToArray());
            _overlayConstraints.Clear();

            _overlayConstraints.Add(OverlayView.TopAnchor.ConstraintEqualTo(parentView.TopAnchor));
            _overlayConstraints.Add(OverlayView.LeftAnchor.ConstraintEqualTo(parentView.LeftAnchor));
            _overlayConstraints.Add(OverlayView.RightAnchor.ConstraintEqualTo(parentView.RightAnchor));
            _overlayConstraints.Add(OverlayView.BottomAnchor.ConstraintEqualTo(parentView.BottomAnchor));

            NSLayoutConstraint.ActivateConstraints(_overlayConstraints.ToArray());
        }

        protected async Task<bool> WaitDismiss()
        {
            for (var i = 0; i < 5;i++)
            {
                if(!IsRunning){
                    return true;
                }
                await Task.Delay(250);
            }

            return false;
        }


    }
}
