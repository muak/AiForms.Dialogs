using System;
using Xamarin.Forms;

namespace AiForms.Dialogs.Abstractions
{
    public class DialogView : ExtraView
    {
        public bool IsCanceledOnTouchOutside { get; set; } = true;
        public DialogNotifier DialogNotifier { get; } = new DialogNotifier();
        public Color OverlayColor { get; set; }

        public virtual void SetUp() {}
        public virtual void TearDwon() {}
    }
}
