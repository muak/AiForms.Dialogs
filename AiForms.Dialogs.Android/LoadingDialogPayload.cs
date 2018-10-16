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

        public LoadingDialogPayload(ViewGroup contentView, LoadingView loadingView = null)
        {
            LoadingView = loadingView;
            ContentView = contentView;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                LoadingView = null;
                ContentView = null;
            }
            base.Dispose(disposing);
        }
    }
}
