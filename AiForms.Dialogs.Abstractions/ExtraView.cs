using System;
using Xamarin.Forms;
namespace AiForms.Dialogs.Abstractions
{
    public class ExtraView:ContentView
    {
        public static BindableProperty ProportionalWidthProperty =
            BindableProperty.Create(
                nameof(ProportionalWidth),
                typeof(double),
                typeof(ExtraView),
                -1d,
                defaultBindingMode: BindingMode.OneWay
            );

        public double ProportionalWidth
        {
            get { return (double)GetValue(ProportionalWidthProperty); }
            set { SetValue(ProportionalWidthProperty, value); }
        }

        public static BindableProperty ProportionalHeightProperty =
            BindableProperty.Create(
                nameof(ProportionalHeight),
                typeof(double),
                typeof(ExtraView),
                -1d,
                defaultBindingMode: BindingMode.OneWay
            );

        public double ProportionalHeight
        {
            get { return (double)GetValue(ProportionalHeightProperty); }
            set { SetValue(ProportionalHeightProperty, value); }
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

        public static BindableProperty CornerRadiusProperty =
            BindableProperty.Create(
                nameof(CornerRadius),
                typeof(float),
                typeof(ExtraView),
                default(float),
                defaultBindingMode: BindingMode.OneWay
            );

        public float CornerRadius
        {
            get { return (float)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public virtual void RunPresentationAnimation() {}
        public virtual void RunDismissalAnimation() {}
        public virtual void Destroy() {}
    }
}
