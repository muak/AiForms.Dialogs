using System;
using System.Threading.Tasks;
using AiForms.Extras.Abstractions;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using System.Linq;
using Xamarin.Forms;
using System.Reflection;

namespace AiForms.Extras
{
    [Foundation.Preserve(AllMembers = true)]
    public class ReusableDialog: IReusableDialog
    {
        UIViewController _viewController => Extras.RootViewController;
        DialogView _dlgView;
        IVisualElementRenderer _renderer;
        UIView _overlayView;
        UIViewController _contentViewController;
        DialogPresentationController _dialogController;
        Action OnceInitializeAction;

        public ReusableDialog(DialogView view)
        {
            _dlgView = view;

            _overlayView = new UIView()
            {
                BackgroundColor = _dlgView.OverlayColor.ToUIColor(),
                Opaque = false,
                Alpha = 0f
            };

            var tapGesture = new UITapGestureRecognizer();
            tapGesture.AddTarget(() => DimmingViewTapped(tapGesture));
            _overlayView.AddGestureRecognizer(tapGesture);


            // Because the process can't be executed until application completely loads,
            // set the action here to execute later on.
            OnceInitializeAction = Initialize;
        }

        void Initialize()
        {
            _dlgView.Parent = Application.Current.MainPage;

            _renderer = Extras.CreateNativeView(_dlgView);

            if (_dlgView.CornerRadius > 0)
            {
                _renderer.NativeView.Layer.CornerRadius = _dlgView.CornerRadius;
                _renderer.NativeView.Layer.MasksToBounds = true;
            }

            var measure = Extras.Measure(_dlgView);
            _renderer.SetElementSize(measure);

            _contentViewController = new UIViewController
            {
                View = _renderer.NativeView
            };


            _dialogController = new DialogPresentationController(_dlgView, _overlayView, _contentViewController, _viewController);
            _contentViewController.TransitioningDelegate = _dialogController;

            OnceInitializeAction = null;
        }

        public void Dispose()
        {
            _dlgView.Destroy();
            _dlgView.Parent = null;
            Extras.DisposeModelAndChildrenRenderers(_dlgView);
            _dlgView = null;

            var tapGesture = _overlayView.GestureRecognizers.FirstOrDefault();
            _overlayView.RemoveGestureRecognizer(tapGesture);
            tapGesture?.Dispose();

            _overlayView.RemoveFromSuperview();
            _overlayView.Dispose();
            _overlayView = null;
            
            _contentViewController.TransitioningDelegate = null;
            _contentViewController.Dispose();
            _contentViewController = null;

            _dialogController.Dispose();
            _dialogController = null;

            _renderer = null;
        }

        public async Task<bool> ShowAsync()
        {
            _dlgView.SetUp();

            OnceInitializeAction?.Invoke();

            var tcs = new TaskCompletionSource<bool>();

            async void cancel(object sender, EventArgs e)
            {
                _dlgView.RunDismissalAnimation();
                await _viewController.DismissViewControllerAsync(true);
                tcs.SetResult(false);

            }
            async void complete(object sender, EventArgs e)
            {
                _dlgView.RunDismissalAnimation();
                await _viewController.DismissViewControllerAsync(true);
                tcs.SetResult(true);
            };

            _dlgView.DialogNotifier.Canceled += cancel;
            _dlgView.DialogNotifier.Completed += complete;

            _dlgView.RunPresentationAnimation();
            await _viewController.PresentViewControllerAsync(_contentViewController, true);

            try
            {
                return await tcs.Task;
            }
            finally
            {
                _dlgView.DialogNotifier.Canceled -= cancel;
                _dlgView.DialogNotifier.Completed -= complete;
                _dlgView.TearDwon();
            }
        }

        void DimmingViewTapped(UITapGestureRecognizer sender)
        {
            if (_dlgView.IsCanceledOnTouchOutside)
            {
                _dlgView.DialogNotifier.Cancel();
            }
        }


    }
}
