using System.Threading.Tasks;
using AiForms.Dialogs.Abstractions;
using UIKit;

namespace AiForms.Dialogs
{
    [Foundation.Preserve(AllMembers = true)]
    public class DialogImplementation:IDialog
    {
        UIViewController _viewController => Dialogs.RootViewController;

        public DialogImplementation()
        {
        }

        public IReusableDialog Create<TView>(object viewModel = null) where TView : DialogView
        {
            var view = ExtraView.InstanceCreator<TView>.Create();
            return Create(view, viewModel);
        }

        public IReusableDialog Create(DialogView view, object viewModel = null)
        {
            if(viewModel != null)
            {
                view.BindingContext = viewModel;
            }            
            return new ReusableDialog(view);
        }

        public async Task<bool> ShowAsync<TView>(object viewModel = null) where TView : DialogView
        {
            using (var dlg = Create<TView>(viewModel))
            {
                return await dlg.ShowAsync();
            }
        }

        public async Task<bool> ShowAsync(DialogView view, object viewModel = null)
        {
            using (var dlg = Create(view, viewModel))
            {
                return await dlg.ShowAsync();
            }
        }
    }
}
