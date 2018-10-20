using System.Threading.Tasks;
using AiForms.Dialogs.Abstractions;
using Android.App;

namespace AiForms.Dialogs
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class DialogImplementation : IDialog
    {
        public static readonly string ExtraDialogTag = "ExtraDialog";
        internal ExtraPlatformDialog ExtraDialog;
        FragmentManager FragmentManager => Dialogs.FragmentManager;

        public DialogImplementation()
        {
            ExtraDialog = new ExtraPlatformDialog();
        }

        public IReusableDialog Create<TView>(object viewModel = null) where TView : DialogView
        {
            var view = ExtraView.InstanceCreator<TView>.Create();
            return Create(view, viewModel);
        }

        public IReusableDialog Create(DialogView view, object viewModel = null)
        {
            view.BindingContext = viewModel;
            return new ReusableDialog(view);
        }

        public async Task<bool> ShowAsync<TView>(object viewModel = null) where TView : DialogView
        {
            if (IsRunning()) return false;

            using (var dlg = Create<TView>(viewModel))
            {
                return await dlg.ShowAsync();
            }
        }

        public async Task<bool> ShowAsync(DialogView view, object viewModel = null)
        {
            if (IsRunning()) return false;

            using (var dlg = Create(view, viewModel))
            {
                return await dlg.ShowAsync();
            }
        }

        bool IsRunning()
        {
            var dialog = Dialogs.FragmentManager.FindFragmentByTag<ExtraPlatformDialog>(DialogImplementation.ExtraDialogTag);
            return dialog != null;
        }

    }
}
