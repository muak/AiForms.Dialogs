using System;
using AiForms.Dialogs.Abstractions;
using Android.App;
using Android.OS;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;

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

            return Dialogs.CreateFullScreenTransparentDialog(_contentView);
        }

        public override void OnStart()
        {
            base.OnStart();

            _dialogView.RunPresentationAnimation();
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();

            _contentView.RemoveFromParent();
            _contentView = null;
            _dialogView = null;
            Dialog?.Dispose();
        }
    }
}
