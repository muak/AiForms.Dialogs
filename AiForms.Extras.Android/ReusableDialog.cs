using System;
using System.Threading.Tasks;
using AiForms.Extras.Abstractions;
using Android.App;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using XF = Xamarin.Forms;

namespace AiForms.Extras
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class ReusableDialog:Java.Lang.Object, IReusableDialog, View.IOnKeyListener
    {
        DialogImplementation _extraDialog;
        ExtraPlatformDialog _platformDialog => _extraDialog.ExtraDialog;
        FragmentManager FragmentManager => _extraDialog.FragmentManager;
        DialogView _dlgView;
        IVisualElementRenderer _renderer;
        ViewGroup _contentView;
        Action OnceInitializeAction;

        public ReusableDialog(DialogView view)
        {
            _dlgView = view;
            _extraDialog = Dialog.Instance as DialogImplementation;

            // Because the process can't be executed until application completely loads,
            // set the action here to execute later on.
            OnceInitializeAction = Initialize;
        }

        void Initialize()
        {
            _dlgView.Parent = XF.Application.Current.MainPage;

            _renderer = Extras.CreateNativeView(_dlgView);

            var measure = Extras.Measure(_dlgView);

            _dlgView.Layout(new XF.Rectangle(0, 0, measure.Width, measure.Height));

            if (_dlgView.CornerRadius > 0)
            {
                var nativeView = _renderer.View as ViewGroup;
                var border = new GradientDrawable();
                border.SetCornerRadius(Extras.Context.ToPixels(_dlgView.CornerRadius));
                if (!_dlgView.BackgroundColor.IsDefault)
                {
                    border.SetColor(_dlgView.BackgroundColor.ToAndroid());
                }
                nativeView.ClipToOutline = true;
                nativeView.SetBackground(border);
            }

            _contentView = (Extras.Context as Activity).LayoutInflater.Inflate(Resource.Layout.ExtraDialogLayout, null) as RelativeLayout;
            _contentView.SetBackgroundColor(_dlgView.OverlayColor.ToAndroid());

            _contentView.SetOnKeyListener(this);
            _contentView.FocusableInTouchMode = true;
            _contentView.Touch += _contentView_Touch;

            var width = Extras.Context.ToPixels(_dlgView.Bounds.Width);
            var height = Extras.Context.ToPixels(_dlgView.Bounds.Height);

            var innerView = _contentView.FindViewById<RelativeLayout>(Resource.Id.extra_dialog_container);

            using (var param = new RelativeLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
            {
                Width = (int)width,
                Height = (int)height
            })
            {
                param.AddRule(LayoutRules.CenterInParent);
                innerView.AddView(_renderer.View, 0, param);
            };

            OnceInitializeAction = null;
        }

        public async Task<bool> ShowAsync()
        {
            var dialog = _extraDialog.FragmentManager.FindFragmentByTag<ExtraPlatformDialog>(DialogImplementation.ExtraDialogTag);
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
            };

            _dlgView.DialogNotifier.Canceled += cancel;
            _dlgView.DialogNotifier.Completed += complete;


            var payload = new ExtraDialogPayload(_dlgView,_contentView);
            var bundle = new Bundle();
            bundle.PutSerializable("extraDialogPayload", payload);
            _platformDialog.Arguments = bundle;
            _platformDialog.Show(FragmentManager, DialogImplementation.ExtraDialogTag);

            try
            {
                return await tcs.Task;
            }
            finally
            {
                _dlgView.DialogNotifier.Canceled -= cancel;
                _dlgView.DialogNotifier.Completed -= complete;
                _dlgView.TearDwon();
                payload.Dispose();
                bundle.Dispose();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing) {
                _dlgView.Destroy();
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

                _extraDialog = null;
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
                _dlgView.DialogNotifier.Cancel();
                return;
            }

            e.Handled = false;
        }

        public bool OnKey(View v, [GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back && e.Action == KeyEventActions.Up)
            {
                _dlgView.DialogNotifier.Cancel();
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

            var dialog = FragmentManager.FindFragmentByTag<ExtraPlatformDialog>(DialogImplementation.ExtraDialogTag);
            dialog.Dismiss();
        }
    }
}
