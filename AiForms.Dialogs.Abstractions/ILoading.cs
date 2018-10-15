using System;
using System.Threading.Tasks;

namespace AiForms.Dialogs.Abstractions
{
    public interface ILoading
    {
        void Show(string message = null,bool isCurrentScope = false);
        void Hide();
        void SetMessage(string message);
        Task StartAsync(Func<IProgress<double>,Task> action, string message = null, bool isCurrentScope = false);
    }
}
