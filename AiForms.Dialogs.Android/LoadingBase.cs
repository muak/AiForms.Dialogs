using System;
using Android.App;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.Android;

namespace AiForms.Dialogs
{
    public class LoadingBase:IDisposable
    {
        protected FragmentManager FragmentManager => Dialogs.FragmentManager;
        protected LoadingPlatformDialog PlatformDialog;
        protected Action OnceInitializeAction;
        protected Progress<double> Progress;

        protected ViewGroup ContentView;

        protected TaskCompletionSource<bool> IsDialogShownTcs;

        public LoadingBase(LoadingPlatformDialog loadingDialog)
        {
            PlatformDialog = loadingDialog;

            ContentView = new FrameLayout(Dialogs.Context);

            using (var param = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent))
            {
                ContentView.LayoutParameters = param;
            }

            ContentView.SetClipChildren(false);
            ContentView.SetClipToPadding(false);
        }

        public virtual void Dispose()
        {
            ContentView.RemoveFromParent();
            ContentView.Dispose(); 
            ContentView = null;

            OnceInitializeAction = null;
            PlatformDialog = null;
        }

        protected bool IsRunning()
        {
            var dialog = Dialogs.FragmentManager.FindFragmentByTag<LoadingPlatformDialog>(LoadingImplementation.LoadingDialogTag);
            return dialog != null;
        }

        protected async Task WaitDialogDestroy()
        {
            var dialog = Dialogs.FragmentManager.FindFragmentByTag<LoadingPlatformDialog>(LoadingImplementation.LoadingDialogTag);
            if(dialog == null)
            {
                return;
            }
            
            await dialog.DestroyTcs.Task;
            await Task.Delay(100); // in additon, wait for a bit time untile dialog is completely released.
        }
    }
}
