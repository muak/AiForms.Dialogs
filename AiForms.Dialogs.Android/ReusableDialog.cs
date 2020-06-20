using System;
using System.Threading;
using System.Threading.Tasks;
using AiForms.Dialogs.Abstractions;
using Android.App;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using XF = Xamarin.Forms;

namespace AiForms.Dialogs
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class ReusableDialog:Java.Lang.Object, IReusableDialog, View.IOnKeyListener
    {
        ExtraPlatformDialog _platformDialog;
        FragmentManager FragmentManager => Dialogs.FragmentManager;
        DialogView _dlgView;
        IVisualElementRenderer _renderer;
        ViewGroup _contentView;
        Action OnceInitializeAction;
        Guid _guid;

        public ReusableDialog(DialogView view)
        {
            _dlgView = view;            
            _guid = Guid.NewGuid();

            // Because the process can't be executed until application completely loads,
            // set the action here to execute later on.
            OnceInitializeAction = Initialize;
        }

        void Initialize()
        {
            _dlgView.Parent = XF.Application.Current.MainPage;

            _renderer = Dialogs.CreateNativeView(_dlgView);

            var measure = Dialogs.Measure(_dlgView);

            _dlgView.Layout(new XF.Rectangle(0, 0, measure.Width, measure.Height));

            if (_dlgView.CornerRadius > 0)
            {
                var nativeView = _renderer.View as ViewGroup;
                var border = new GradientDrawable();
                border.SetCornerRadius(Dialogs.Context.ToPixels(_dlgView.CornerRadius));
                if (!_dlgView.BackgroundColor.IsDefault)
                {
                    border.SetColor(_dlgView.BackgroundColor.ToAndroid());
                }
                nativeView.ClipToOutline = true;
                nativeView.SetBackground(border);
            }


            _contentView = new FrameLayout(Dialogs.Context);
            using (var param = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent))
            {
                _contentView.LayoutParameters = param;
            }

            var fixPaddingTop = _dlgView.OverlayColor.IsTransparentOrDefault() ? Dialogs.StatusBarHeight : 0;
            if (_dlgView.UseCurrentPageLocation)
            {
                var padding = Dialogs.CalcWindowPadding();
                _contentView.SetPadding(0, padding.top - fixPaddingTop, 0, padding.bottom);
            }
            else
            {
                _contentView.SetPadding(0, Dialogs.StatusBarHeight - fixPaddingTop, 0, 0); 
            }

            _contentView.SetBackgroundColor(_dlgView.OverlayColor.ToAndroid());
            _contentView.SetClipChildren(false);
            _contentView.SetClipToPadding(false);

            _contentView.SetOnKeyListener(this);
            _contentView.FocusableInTouchMode = true;
            _contentView.Touch += _contentView_Touch;

            var width = Dialogs.Context.ToPixels(_dlgView.Bounds.Width);
            var height = Dialogs.Context.ToPixels(_dlgView.Bounds.Height);

            using (var param = new FrameLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
            {
                Width = (int)width,
                Height = (int)height,
                Gravity = Dialogs.GetGravity(_dlgView),
            })
            {
                Dialogs.SetOffsetMargin(param, _dlgView);
                _contentView.AddView(_renderer.View, 0, param);
            };

            // For now, Dynamic resizing is gaven to only Dialog.
            _dlgView.LayoutNative = () =>
            {
                if (_renderer == null || _renderer.View.IsDisposed()) return;

                var p = _renderer.View.LayoutParameters as FrameLayout.LayoutParams;
                var w = (int)Dialogs.Context.ToPixels(_dlgView.Bounds.Width);
                var h = (int)Dialogs.Context.ToPixels(_dlgView.Bounds.Height);


                using (var param = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
                {
                    Width = w,
                    Height = h,
                    Gravity = p.Gravity
                })
                {
                    Dialogs.SetOffsetMargin(param, _dlgView);
                    _renderer.View.LayoutParameters = param;
                };
            };

            OnceInitializeAction = null;
        }

        public async Task<bool> ShowAsync()
        {
            var dialog = FragmentManager.FindFragmentByTag<ExtraPlatformDialog>(_guid.ToString());
            if (dialog != null)
            {
                return false;
            }

            _dlgView.SetUp();

            OnceInitializeAction?.Invoke();

            var tcs = new TaskCompletionSource<bool>();

            async void cancel(object sender,EventArgs e) 
            {
                _dlgView.RunDismissalAnimation();
                await Dismiss();
                tcs.SetResult(false);
            }
            async void complete(object sender, EventArgs e)
            {
                _dlgView.RunDismissalAnimation();
                await Dismiss();
                tcs.SetResult(true);
            }

            _dlgView.DialogNotifierInternal.Canceled += cancel;
            _dlgView.DialogNotifierInternal.Completed += complete;


            var payload = new ExtraDialogPayload(_dlgView,_contentView);
            var bundle = new Bundle();
            bundle.PutSerializable("extraDialogPayload", payload);
            _platformDialog = new ExtraPlatformDialog();
            _platformDialog.Arguments = bundle;
            _platformDialog.Show(FragmentManager, _guid.ToString());

            try
            {
                return await tcs.Task;
            }
            finally
            {
                _dlgView.DialogNotifierInternal.Canceled -= cancel;
                _dlgView.DialogNotifierInternal.Completed -= complete;
                _dlgView.TearDown();
                payload.Dispose();
                bundle.Dispose();

            }
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing) {
                _dlgView.Destroy();
                _dlgView.LayoutNative = null;
                _dlgView.BindingContext = null;
                _dlgView.Parent = null;
                _dlgView = null;

                _contentView.Touch -= _contentView_Touch;
                _contentView.SetOnKeyListener(null);


                if (!_renderer.View.IsDisposed())
                {
                    _renderer.View.Dispose();
                }                

                _contentView.Dispose();
                _contentView = null;

                _renderer.Dispose();
                _renderer = null;

                OnceInitializeAction = null;
            }
            base.Dispose(disposing);           
        }

        void _contentView_Touch(object sender, View.TouchEventArgs e)
        {
            if (e.Event.Action != MotionEventActions.Down || !_dlgView.IsCanceledOnTouchOutside)
            {
                e.Handled = false;
                return;
            }

            Android.Graphics.Rect rect = new Android.Graphics.Rect();
            _renderer.View.GetHitRect(rect);

            if (!rect.Contains((int)e.Event.GetX(), (int)e.Event.GetY()))
            {
                _dlgView.DialogNotifierInternal.Cancel();
                return;
            }

            e.Handled = false;
        }

        public bool OnKey(View v, [GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back && e.Action == KeyEventActions.Up)
            {
                _dlgView.DialogNotifierInternal.Cancel();
                return true;
            }

            return false;
        }

        async Task Dismiss()
        {
            var tcs = new TaskCompletionSource<bool>();

            var anim = new AlphaAnimation(_contentView.Alpha, 0.0f);
            anim.Duration = 250;
            anim.FillAfter = true;

            void handler(object sender, Animation.AnimationEndEventArgs e)
            {
                tcs.SetResult(true);
            };

            anim.AnimationEnd += handler;

            _contentView.StartAnimation(anim);

            await tcs.Task;
            anim.AnimationEnd -= handler;

            var dialog = FragmentManager.FindFragmentByTag<ExtraPlatformDialog>(_guid.ToString());
            dialog.Dismiss();
            _contentView.RemoveFromParent();
            _platformDialog.Dispose();
            _platformDialog = null;

            await Task.Delay(250); // wait for a bit time until the dialog is completely released.
        }
    }
}
