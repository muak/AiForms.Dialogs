using System.Linq;
using AiForms.Dialogs.Abstractions;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace AiForms.Dialogs
{
    [Foundation.Preserve(AllMembers = true)]
    public class DialogPresentationController : UIPresentationController, IUIViewControllerTransitioningDelegate, IUIViewControllerAnimatedTransitioning
    {
        UIView _overlayView;
        DialogView _dlgView;

        public DialogPresentationController(DialogView view,UIView overlayView, UIViewController presentedViewController, UIViewController presentingViewController)
            : base(presentedViewController, presentingViewController)
        {
            _dlgView = view;
            _overlayView = overlayView;
            presentedViewController.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            PresentedView.UserInteractionEnabled = true;
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                _overlayView = null;
                _dlgView = null;
            }
            base.Dispose(disposing);
        }

        // before transition start
        public override void PresentationTransitionWillBegin()
        {
            if(!ContainerView.Subviews.Any()) {
                ContainerView.InsertSubview(_overlayView, 0);
            }

            PresentingViewController.GetTransitionCoordinator()?.AnimateAlongsideTransition(
                obj => _overlayView.Alpha = 1, obj => { }
            );
        }

        public override void DismissalTransitionWillBegin()
        {
            PresentingViewController.GetTransitionCoordinator()?.AnimateAlongsideTransition(
                obj => _overlayView.Alpha = 0f, null
            );
        }

        public override void DismissalTransitionDidEnd(bool completed)
        {
            if (!completed)
            {
                return;
            }
        }


        public override CGSize GetSizeForChildContentContainer(IUIContentContainer contentContainer, CGSize parentContainerSize)
        {
            if (contentContainer == null)
            {
                return new CGSize(0f, 0f);
            }

            return new CGSize(_dlgView.Bounds.Width, _dlgView.Bounds.Height);
        }

        public override CGRect FrameOfPresentedViewInContainerView
        {
            get
            {
                var presentedViewFrame = CGRect.Empty;
                var containerViewBounds = ContainerView.Bounds;

                presentedViewFrame.Size = GetSizeForChildContentContainer(PresentedViewController, containerViewBounds.Size);
                presentedViewFrame.X = containerViewBounds.Size.Width / 2f - presentedViewFrame.Size.Width / 2f;
                presentedViewFrame.Y = containerViewBounds.Size.Height / 2f - presentedViewFrame.Size.Height / 2f;

                return presentedViewFrame;
            }
        }

        public override void ContainerViewWillLayoutSubviews()
        {
            base.ContainerViewWillLayoutSubviews();

            _overlayView.Frame = ContainerView.Bounds;
            PresentedView.Frame = FrameOfPresentedViewInContainerView;

            // If ContentPage background color is default or transparent, it is made to turn white by Xamarin.Forms.
            // So original color is made to restore here.
            PresentedView.BackgroundColor = _dlgView.BackgroundColor.ToUIColor();
        }

        public double TransitionDuration(IUIViewControllerContextTransitioning transitionContext)
        {
            return 0.25;
        }

        public void AnimateTransition(IUIViewControllerContextTransitioning transitionContext)
        {
            var fromViewController = transitionContext.GetViewControllerForKey(UITransitionContext.FromViewControllerKey);

            bool isPresenting = (fromViewController == PresentingViewController);

            if (isPresenting)
            {
                PresentAnimateTransition(transitionContext);
            }
            else
            {
                DismissAnimateTransition(transitionContext);
            }
        }

        void PresentAnimateTransition(IUIViewControllerContextTransitioning context)
        {
            var toViewController = context.GetViewControllerForKey(UITransitionContext.ToViewControllerKey);
            var contentView = context.ContainerView;

            ContainerView.AddSubview(toViewController.View);

            toViewController.View.Alpha = 0;

            UIView.Animate(TransitionDuration(context), 0, UIViewAnimationOptions.CurveLinear, () =>
            {
                toViewController.View.Alpha = 1;
            }, () =>
            {
                context.CompleteTransition(true);
            });
        }

        void DismissAnimateTransition(IUIViewControllerContextTransitioning context)
        {
            var fromViewController = context.GetViewControllerForKey(UITransitionContext.FromViewControllerKey);

            UIView.Animate(TransitionDuration(context), 0, UIViewAnimationOptions.CurveLinear, () =>
            {
                fromViewController.View.Alpha = 0;
            }, () =>
            {
                context.CompleteTransition(true);
            });
        }

        [Export("presentationControllerForPresentedViewController:presentingViewController:sourceViewController:")]
        public UIPresentationController GetPresentationControllerForPresentedViewController(UIViewController presentedViewController, UIViewController presentingViewController, UIViewController sourceViewController)
        {
            return this;
        }

        [Export("animationControllerForPresentedController:presentingController:sourceController:")]
        public IUIViewControllerAnimatedTransitioning GetAnimationControllerForPresentedController(UIViewController presented, UIViewController presenting, UIViewController source)
        {
            return this;
        }

        [Export("animationControllerForDismissedController:")]
        public IUIViewControllerAnimatedTransitioning GetAnimationControllerForDismissedController(UIViewController dismissed)
        {
            return this;
        }
    }
}
