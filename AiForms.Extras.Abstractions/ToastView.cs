using System;
using Xamarin.Forms;

namespace AiForms.Extras.Abstractions
{
    public class ToastView : ContentView
    {
        public float CornerRadius { get; set; }
        public LayoutAlignment LayoutAlignment { get; set; } = LayoutAlignment.Center;
        public double ToastWidth { get; set; } = 200;
        public double ToastHeight { get; set; } = 240;
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public int Duration { get; set; } = 1500;   // 0-3500 

        public virtual void RunPresentationAnimation() { }
        public virtual void Destroy() { }
    }
}
