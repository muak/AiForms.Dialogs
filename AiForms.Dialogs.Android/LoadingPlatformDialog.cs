using AiForms.Dialogs.Abstractions;
using Android.App;
using Android.OS;
using Android.Views;
using Xamarin.Forms.Platform.Android;
using System.Threading.Tasks;

namespace AiForms.Dialogs
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class LoadingPlatformDialog : DialogFragment
    {
        LoadingView _loadingView;
        ViewGroup _contentView;
        public TaskCompletionSource<bool> DestroyTcs { get; private set; }

        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            base.OnCreateDialog(savedInstanceState);

            var payload = Arguments.GetSerializable(LoadingDialogPayload.PayloadKey) as LoadingDialogPayload;

            _loadingView = payload.LoadingView;
            _contentView = payload.ContentView;

            //payload.Dispose(); // TODO: a exception occurs on some devices. 

            var dialog = Dialogs.CreateFullScreenTransparentDialog(_contentView);

            Cancelable = false;
            dialog.SetCancelable(false);
            dialog.SetCanceledOnTouchOutside(false);

            DestroyTcs = new TaskCompletionSource<bool>();

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
            DestroyTcs.SetResult(true);
            DestroyTcs = null;
        }
    }
}
