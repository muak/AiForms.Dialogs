using System;
namespace AiForms.Dialogs.Abstractions
{
    public class DialogNotifier
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
