using System;
namespace AiForms.Dialogs.Abstractions
{
    public interface IDialogNotifier
    {
        void Complete();
        void Cancel();
    }

    public class DialogNotifier:IDialogNotifier
    {
        internal event EventHandler Completed;
        internal event EventHandler Canceled;

        public void Complete()
        {
            Completed?.Invoke(this, EventArgs.Empty);
        }

        public void Cancel()
        {
            Canceled?.Invoke(this, EventArgs.Empty);
        }
    }
}
