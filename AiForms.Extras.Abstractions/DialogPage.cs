using System;
using Xamarin.Forms;

namespace AiForms.Extras.Abstractions
{
    public class DialogPage : ContentPage
    {
        public DialogNotifier DialogNotifier { get; } = new DialogNotifier();
        public double DialogWidth { get; set; } = 280;
        public double DialogHeight { get; set; } = 200;
        public bool IsCanceledOnTouchOutside { get; set; } = true;
        public float CornerRadius { get; set; }
        public LayoutAlignment LayoutAlignment { get; set; } = LayoutAlignment.Center;

        public virtual void RunPresentationAnimation() { }
        public virtual void RunCompleteAnimation() { }
        public virtual void RunCancelAnimation() { }
        public virtual void Destroy() { }

    }
}
