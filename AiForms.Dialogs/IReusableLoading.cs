using System;
using System.Threading.Tasks;

namespace AiForms.Dialogs.Abstractions
{
    public interface IReusableLoading:IDisposable
    {
        void Show(bool isCurrentScope = false);
        void Hide();
        Task StartAsync(Func<IProgress<double>, Task> action, bool isCurrentScope = false);
    }
}
