using System.Threading.Tasks;
using AiForms.Extras.Abstractions;
using Android.App;
using Android.Views;
using Java.IO;

namespace AiForms.Extras
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class DialogImplementation : IDialog
    {
        public static readonly string ExtraDialogTag = "ExtraDialog";
        internal ExtraPlatformDialog ExtraDialog;
        FragmentManager FragmentManager => Extras.FragmentManager;

        public DialogImplementation()
        {
            ExtraDialog = new ExtraPlatformDialog();
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
            using(var dlg = Create<TView>(viewModel))
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

    [Android.Runtime.Preserve(AllMembers = true)]
    public class ExtraDialogPayload : Java.Lang.Object, ISerializable
    {
        public DialogView DialogView { get; set; }
        public ViewGroup ContentView { get; set; }

        public ExtraDialogPayload(DialogView dialogView, ViewGroup contentView)
        {
            DialogView = dialogView;
            ContentView = contentView;
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing){
                DialogView = null;
                ContentView = null;
            }
            base.Dispose(disposing);
        }
    }
}
