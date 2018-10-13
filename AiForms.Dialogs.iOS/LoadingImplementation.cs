using System;
using System.Threading.Tasks;
using AiForms.Dialogs.Abstractions;

namespace AiForms.Dialogs
{
    [Foundation.Preserve(AllMembers = true)]
    public class LoadingImplementation : ILoading
    {
        LoadingConfig _config => Configurations.LoadingConfig;

        ReusableLoading _loadingInstance;
        ReusableLoading LoadingInstance
        {
            get
            {
                _loadingInstance = _loadingInstance ?? new ReusableLoading();
                return _loadingInstance;
            }
        }

        public LoadingImplementation()
        {
        }

        public void Show(string message = null, bool isCurrentScope = false)
        {
            if (LoadingInstance.IsRunning)
            {
                return;
            }

            LoadingInstance.Show(message, isCurrentScope);
        }

        public async Task StartAsync(Func<IProgress<double>, Task> action, string message = null, bool isCurrentScope = false)
        {
            if (LoadingInstance.IsRunning)
            {
                return;
            }

            await LoadingInstance.StartAsync(action, message, isCurrentScope);
            Hide();
        }


        public void Hide()
        {
            LoadingInstance.Hide();
            if (!_config.IsReusable)
            {
                LoadingInstance.Dispose();
                _loadingInstance = null;
            }
        }

        public void SetMessage(string message)
        {
            if (_loadingInstance != null)
            {
                _loadingInstance.SetMessage(message);
            }
        }
    }
}
