using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;

namespace AiForms.Dialogs.Abstractions
{
    public interface IDialog
    {
        Task<bool> ShowAsync<TView>(object viewModel = null) where TView : DialogView;
        Task<bool> ShowAsync(DialogView view, object viewModel = null);
        IReusableDialog Create<TView>(object viewModel = null) where TView : DialogView;
        IReusableDialog Create(DialogView view, object viewModel = null);

    }
}
