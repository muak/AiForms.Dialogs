using System;
using AiForms.Dialogs.Abstractions;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;

namespace AiForms.Dialogs
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class ExtraPlatformDialog : AndroidX.Fragment.App.DialogFragment, IDialogInterfaceOnKeyListener
    {
        DialogView _dialogView;
        ViewGroup _contentView;
        View _rootView;
        KeyboardListener _keyboardListener;
        bool _isKeyboardOpen;


        public ExtraPlatformDialog() { }

        // System Required!
        public ExtraPlatformDialog(IntPtr handle, JniHandleOwnership transfer) :base(handle,transfer) { }

        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            base.OnCreateDialog(savedInstanceState);

            var payload = Arguments.GetSerializable("extraDialogPayload") as ExtraDialogPayload;

            _dialogView = payload.DialogView;
            _contentView = payload.ContentView;
             
            var dialog = Dialogs.CreateFullScreenTransparentDialog(_contentView);

            // If the OverlayColor is default or transparent, the top padding of the Dialog is set.
            // Because it avoids the status bar color turning dark.
            if (_dialogView.OverlayColor.IsTransparentOrDefault())
            {
                var height = Dialogs.ContentSize.Height;

                dialog.Window.SetGravity(GravityFlags.CenterHorizontal | GravityFlags.Bottom);
                dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, height);
            }

            dialog.SetOnKeyListener(this);

            _rootView = (Context as Activity).FindViewById(Android.Resource.Id.Content);
            _keyboardListener = new KeyboardListener(_rootView, this);
            _rootView.ViewTreeObserver.AddOnGlobalLayoutListener(_keyboardListener);
            
            return dialog;
        }

        public override void OnStart()
        {
            base.OnStart();
            _dialogView.RunPresentationAnimation();
        }

        public void Clear()
        {
            _rootView.ViewTreeObserver.RemoveOnGlobalLayoutListener(_keyboardListener);
            _keyboardListener?.Dispose();
            _keyboardListener = null;
            _rootView = null;
            _contentView = null;
            _dialogView = null;
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();                       
        }

        public bool OnKey(IDialogInterface dialog, [GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            // Back Button handling
            if (keyCode == Keycode.Back && e.Action == KeyEventActions.Up && !_isKeyboardOpen)
            {
                _dialogView.DialogNotifierInternal.Cancel();
                return true;
            }

            return false;
        }

        // https://stackoverflow.com/questions/4312319/how-to-capture-the-virtual-keyboard-show-hide-event-in-android
        public class KeyboardListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
        {
            int _defaultKeyboardHeightDP = 100;
            int _estimatedKeyboardDP;
            bool _alreadyOpen;
            Rect _rect;
            View _rootView;
            ExtraPlatformDialog _dialog;

            public KeyboardListener(View rootView, ExtraPlatformDialog dialog)
            {
                _rootView = rootView;
                _dialog = dialog;
                _estimatedKeyboardDP = _defaultKeyboardHeightDP + (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop ? 48 : 0);
                _rect = new Rect();
            }

            protected override void Dispose(bool disposing)
            {
                if(disposing)
                {
                    _rect?.Dispose();
                    _rect = null;
                    _rootView = null;
                    _dialog = null;
                }
                base.Dispose(disposing);
            }

            public void OnGlobalLayout()
            {
                int estimatedKeyboardHeight = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, _estimatedKeyboardDP, _rootView.Resources.DisplayMetrics);
                _rootView.GetWindowVisibleDisplayFrame(_rect);
                int heightDiff = _rootView.RootView.Height - (_rect.Bottom - _rect.Top);
                var isShown = heightDiff >= estimatedKeyboardHeight;

                if (isShown == _alreadyOpen)
                {                    
                    return;
                }
                _alreadyOpen = isShown;

                // Delay as OnKey event timing adjustment. 
                new Handler().PostDelayed(() =>
                {
                    if(_dialog != null)
                    {
                        _dialog._isKeyboardOpen = _alreadyOpen;
                    }                    
                }, 1000);                
            }
        }
    }
}
