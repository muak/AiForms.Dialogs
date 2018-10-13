using System;
using System.Threading.Tasks;
using AiForms.Dialogs.Abstractions;

namespace AiForms.Dialogs
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class LoadingImplementation:ILoading
    {
        internal LoadingPlatformDialog LoadingDialog;
        LoadingConfig _config => Configurations.LoadingConfig;

        public static readonly string LoadingDialogTag = "LoadingDialog";

        ReusableLoading _loadingInstance;
        ReusableLoading LoadingInstance {
            get {
                _loadingInstance = _loadingInstance ?? new ReusableLoading();
                return _loadingInstance;
            }
        }

        public LoadingImplementation()
        {
            LoadingDialog = new LoadingPlatformDialog();
        }


        public async Task StartAsync(Func<IProgress<double>, Task> action, string message = null, bool isCurrentScope = false)
        {
            if (IsRunning())
            {
                return;
            }

            await LoadingInstance.StartAsync(action, message, isCurrentScope);
            Hide();
        }

        public void Show(string message = null,bool isCurrentScope = false)
        {
            if(IsRunning()){
                return;
            }

            LoadingInstance.Show(message, isCurrentScope);
        }

        public void Hide()
        {
            LoadingInstance.Hide();
            if(!_config.IsReusable)
            {
                LoadingInstance.Dispose();
                _loadingInstance = null;
            }
        }

        public void SetMessage(string message)
        {
            if(_loadingInstance != null)
            {
                _loadingInstance.SetMessage(message);
            }
        }

        bool IsRunning()
        {
            var dialog = Dialogs.FragmentManager.FindFragmentByTag<LoadingPlatformDialog>(LoadingDialogTag);
            return dialog != null;
        }
    }


}
