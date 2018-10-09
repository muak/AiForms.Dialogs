using System;
using Xamarin.Forms;
namespace AiForms.Extras.Abstractions
{
    public class ExtraView:ContentView
    {
        public static BindableProperty ViewWidthProperty =
            BindableProperty.Create(
                nameof(ViewWidth),
                typeof(double),
                typeof(ExtraView),
                -1d,
                defaultBindingMode: BindingMode.OneWay
            );

        public double ViewWidth
        {
            get { return (double)GetValue(ViewWidthProperty); }
            set { SetValue(ViewWidthProperty, value); }
        }

        public static BindableProperty ViewHeightProperty =
            BindableProperty.Create(
                nameof(ViewHeight),
                typeof(double),
                typeof(ExtraView),
                -1d,
                defaultBindingMode: BindingMode.OneWay
            );

        public double ViewHeight
        {
            get { return (double)GetValue(ViewHeightProperty); }
            set { SetValue(ViewHeightProperty, value); }
        }

        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public float CornerRadius { get; set; }
        public LayoutAlignment VerticalLayoutAlignment { get; set; } = LayoutAlignment.Center;


        public virtual void RunPresentationAnimation() {}
        public virtual void RunDismissalAnimation() {}
        public virtual void Destroy() {}
    }
}
