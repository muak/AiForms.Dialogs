using System;
using System.Threading.Tasks;
using AiForms.Dialogs.Abstractions;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using System.Linq;
using Xamarin.Forms;
using System.Reflection;
using AiForms.Dialogs.Platforms.ios;

namespace AiForms.Dialogs
{
    [Foundation.Preserve(AllMembers = true)]
    public class ReusableDialog: IReusableDialog
    {
        UIViewController _viewController => Dialogs.RootViewController;
        DialogView _dlgView;
        IVisualElementRenderer _renderer;
        UIView _overlayView;
        ContentViewController _contentViewController;
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
            
            var touchGesture = new TouchBeginGestureRecognizer();
            touchGesture.AddTarget(() => DimmingViewTapped(touchGesture));
            _overlayView.AddGestureRecognizer(touchGesture);


            // Because the process can't be executed until application completely loads,
            // set the action here to execute later on.
            OnceInitializeAction = Initialize;
        }

        void Initialize()
        {
            _dlgView.Parent = Application.Current.MainPage;


            _renderer = Dialogs.CreateNativeView(_dlgView);

            if (_dlgView.CornerRadius > 0)
            {
                _renderer.NativeView.Layer.CornerRadius = _dlgView.CornerRadius;
                _renderer.NativeView.Layer.MasksToBounds = true;
            }

            if (_dlgView.BorderWidth > 0)
            {
                _renderer.NativeView.Layer.BorderWidth = (float)_dlgView.BorderWidth;
                _renderer.NativeView.Layer.BorderColor = _dlgView.BorderColor.ToCGColor();
            }

            var measure = Dialogs.Measure(_dlgView);
            //_renderer.SetElementSize(measure); // <- There is diff about margin when use this method.
            _dlgView.Layout(new Xamarin.Forms.Rectangle(0, 0, measure.Width, measure.Height));

            _contentViewController = new ContentViewController(_dlgView.AutoRotateForIOS)
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
            Dialogs.DisposeModelAndChildrenRenderers(_dlgView);
            _dlgView.BindingContext = null;
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

            _dlgView.DialogNotifierInternal.Canceled += cancel;
            _dlgView.DialogNotifierInternal.Completed += complete;

            _dlgView.RunPresentationAnimation();
            await _viewController.PresentViewControllerAsync(_contentViewController, true);

            try
            {
                return await tcs.Task;
            }
            finally
            {
                _dlgView.DialogNotifierInternal.Canceled -= cancel;
                _dlgView.DialogNotifierInternal.Completed -= complete;
                _dlgView.TearDown();
            }
        }

        void DimmingViewTapped(TouchBeginGestureRecognizer sender)
        {
            if (_dlgView.IsCanceledOnTouchOutside)
            {
                _dlgView.DialogNotifierInternal.Cancel();
            }
        }


    }
}
