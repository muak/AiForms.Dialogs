using System;
using Xamarin.Forms;
namespace AiForms.Dialogs.Abstractions
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

        public static BindableProperty VerticalLayoutAlignmentProperty =
            BindableProperty.Create(
                nameof(VerticalLayoutAlignment),
                typeof(LayoutAlignment),
                typeof(ExtraView),
                LayoutAlignment.Center,
                defaultBindingMode: BindingMode.OneWay
            );

        public LayoutAlignment VerticalLayoutAlignment
        {
            get { return (LayoutAlignment)GetValue(VerticalLayoutAlignmentProperty); }
            set { SetValue(VerticalLayoutAlignmentProperty, value); }
        }

        public static BindableProperty HorizontalLayoutAlignmentProperty =
            BindableProperty.Create(
                nameof(HorizontalLayoutAlignment),
                typeof(LayoutAlignment),
                typeof(ExtraView),
                LayoutAlignment.Center,
                defaultBindingMode: BindingMode.OneWay
            );

        public LayoutAlignment HorizontalLayoutAlignment
        {
            get { return (LayoutAlignment)GetValue(HorizontalLayoutAlignmentProperty); }
            set { SetValue(HorizontalLayoutAlignmentProperty, value); }
        }

        public static BindableProperty OffsetXProperty =
            BindableProperty.Create(
                nameof(OffsetX),
                typeof(int),
                typeof(ExtraView),
                default(int),
                defaultBindingMode: BindingMode.OneWay
            );

        public int OffsetX
        {
            get { return (int)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }

        public static BindableProperty OffsetYProperty =
            BindableProperty.Create(
                nameof(OffsetY),
                typeof(int),
                typeof(ExtraView),
                default(int),
                defaultBindingMode: BindingMode.OneWay
            );

        public int OffsetY
        {
            get { return (int)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }

        public float CornerRadius { get; set; }

        public virtual void RunPresentationAnimation() {}
        public virtual void RunDismissalAnimation() {}
        public virtual void Destroy() {}
    }
}
