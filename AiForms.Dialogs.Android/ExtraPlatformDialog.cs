using System;
using AiForms.Dialogs.Abstractions;
using Android.App;
using Android.OS;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Graphics;

namespace AiForms.Dialogs
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class ExtraPlatformDialog : Android.App.DialogFragment
    {
        DialogView _dialogView;
        ViewGroup _contentView;

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
                Display display = (Context as Activity).WindowManager.DefaultDisplay;
                Point size = new Point();
                display.GetSize(size);

                var height = size.Y - (int)Context.ToPixels(24);

                dialog.Window.SetGravity(GravityFlags.CenterHorizontal | GravityFlags.Bottom);
                dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, height);
            }

            return dialog;
        }

        public override void OnStart()
        {
            base.OnStart();
            _dialogView.RunPresentationAnimation();
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();

            _contentView = null;
            _dialogView = null;
            Dialog?.Dispose();
        }
    }
}
