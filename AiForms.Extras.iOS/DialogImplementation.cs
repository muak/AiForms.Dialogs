using System.Threading.Tasks;
using AiForms.Extras.Abstractions;
using UIKit;

namespace AiForms.Extras
{
    [Foundation.Preserve(AllMembers = true)]
    public class DialogImplementation:IDialog
    {
        UIViewController _viewController => Extras.RootViewController;

        public DialogImplementation()
        {
        }

        public IReusableDialog Create<TView>(object viewModel = null) where TView : DialogView, new()
        {
            var view = new TView();
            return Create(view, viewModel);
        }

        public IReusableDialog Create(DialogView view, object viewModel = null)
        {
            view.BindingContext = viewModel;
            return new ReusableDialog(view);
        }

        public async Task<bool> ShowAsync<TView>(object viewModel = null) where TView : DialogView, new()
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
