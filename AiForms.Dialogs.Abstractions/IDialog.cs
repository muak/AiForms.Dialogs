using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;

namespace AiForms.Dialogs.Abstractions
{
    public interface IDialog
    {
        Task<bool> ShowAsync<TView>(object viewModel = null) where TView : DialogView, new();
        Task<bool> ShowAsync(DialogView view, object viewModel = null);
        IReusableDialog Create<TView>(object viewModel = null) where TView : DialogView, new();
        IReusableDialog Create(DialogView view, object viewModel = null);
    }
}
