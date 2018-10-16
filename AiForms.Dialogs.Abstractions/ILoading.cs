using System;
using System.Threading.Tasks;

namespace AiForms.Dialogs.Abstractions
{
    public interface ILoading
    {
        IReusableLoading Create<TView>(object viewModel = null) where TView : LoadingView, new();
        IReusableLoading Create(LoadingView view, object viewModel = null);

        void Show(string message = null, bool isCurrentScope = false);
        void Hide();
        void SetMessage(string message);
        Task StartAsync(Func<IProgress<double>, Task> action, string message = null, bool isCurrentScope = false);

#if DEBUG
        void Dispose();
#endif
    }
}
