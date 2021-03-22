using System.Threading.Tasks;
using AiForms.Dialogs.Abstractions;
using Android.Views;
using Java.IO;

namespace AiForms.Dialogs
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class LoadingDialogPayload : Java.Lang.Object, ISerializable
    {
        public static readonly string PayloadKey = "loadingDialogPayload";
        public LoadingView LoadingView { get; set; }
        public ViewGroup ContentView { get; set; }
        public TaskCompletionSource<bool> IsShownTcs { get; set; }

        public LoadingDialogPayload(ViewGroup contentView, TaskCompletionSource<bool> tcs, LoadingView loadingView = null)
        {
            LoadingView = loadingView;
            ContentView = contentView;
            IsShownTcs = tcs;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                LoadingView = null;
                ContentView = null;
                IsShownTcs = null;
            }
            base.Dispose(disposing);
        }
    }
}
