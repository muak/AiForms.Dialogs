using AiForms.Extras.Abstractions;
using Android.App;
using Android.OS;
using Android.Views;
using Xamarin.Forms.Platform.Android;

namespace AiForms.Extras
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class LoadingPlatformDialog : DialogFragment
    {
        LoadingView _loadingView;
        ViewGroup _contentView;

        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            base.OnCreateDialog(savedInstanceState);

            var payload = Arguments.GetSerializable("loadingDialogPayload") as LoadingDialogPayload;

            _loadingView = payload.LoadingView;
            _contentView = payload.ContentView;

            payload.Dispose();

            var dialog = Extras.CreateFullScreenTransparentDialog(_contentView);

            Cancelable = false;
            dialog.SetCancelable(false);
            dialog.SetCanceledOnTouchOutside(false);

            return dialog;
        }

        public override void OnStart()
        {
            base.OnStart();

            _loadingView?.RunPresentationAnimation();
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();

            _loadingView = null;
            _contentView = null;
            Dialog?.Dispose();
        }
    }
}
