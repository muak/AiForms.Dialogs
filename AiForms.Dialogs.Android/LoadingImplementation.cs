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

        DefaultLoading _defaultInstance;
        DefaultLoading DefaultInstance {
            get {
                _defaultInstance = _defaultInstance ?? new DefaultLoading(LoadingDialog);
                return _defaultInstance;
            }
        }

        public LoadingImplementation()
        {
            LoadingDialog = new LoadingPlatformDialog();
        }

        public IReusableLoading Create<TView>(object viewModel = null) where TView : LoadingView, new()
        {
            var view = new TView();
            return Create(view, viewModel);
        }

        public IReusableLoading Create(LoadingView view, object viewModel = null)
        {
            view.BindingContext = viewModel;
            return new ReusableLoading(view,LoadingDialog);
        }

        public void Dispose()
        {
            if(_defaultInstance != null)
            {
                _defaultInstance.Dispose();
                _defaultInstance = null;
            }
        }

        public async Task StartAsync(Func<IProgress<double>, Task> action, string message = null, bool isCurrentScope = false)
        {
            await WaitDialogDestroy();

            await DefaultInstance.StartAsync(action, message, isCurrentScope);
            Hide();
        }

        public void Show(string message = null,bool isCurrentScope = false)
        {
            if(IsRunning()){
                return;
            }

            DefaultInstance.Show(message, isCurrentScope);
        }

        public void Hide()
        {
            DefaultInstance.Hide();
            if(!_config.IsReusable)
            {
                DefaultInstance.Dispose();
                _defaultInstance = null;
            }
        }

        public void SetMessage(string message)
        {
            if(_defaultInstance != null)
            {
                _defaultInstance.SetMessage(message);
            }
        }

        bool IsRunning()
        {
            var dialog = Dialogs.FragmentManager.FindFragmentByTag<LoadingPlatformDialog>(LoadingDialogTag);
            return dialog != null;
        }

        async Task WaitDialogDestroy()
        {
            var dialog = Dialogs.FragmentManager.FindFragmentByTag<LoadingPlatformDialog>(LoadingImplementation.LoadingDialogTag);
            if (dialog == null)
            {
                return;
            }

            await dialog.DestroyTcs.Task;
            await Task.Delay(100);
        }
    }


}
