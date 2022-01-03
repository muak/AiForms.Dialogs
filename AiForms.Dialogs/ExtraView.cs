using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
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

        public static BindableProperty BorderColorProperty = BindableProperty.Create(
            nameof(BorderColor),
            typeof(Color),
            typeof(ExtraView),
            default(Color),
            defaultBindingMode: BindingMode.OneWay
        );

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public static BindableProperty BorderWidthProperty = BindableProperty.Create(
            nameof(BorderWidth),
            typeof(double),
            typeof(ExtraView),
            default(double),
            defaultBindingMode: BindingMode.OneWay
        );

        public double BorderWidth
        {
            get { return (double)GetValue(BorderWidthProperty); }
            set { SetValue(BorderWidthProperty, value); }
        }

        public static BindableProperty AutoRotateForIOSProperty = BindableProperty.Create(
            nameof(AutoRotateForIOS),
            typeof(bool),
            typeof(ExtraView),
            true,
            defaultBindingMode: BindingMode.OneWay
        );

        public bool AutoRotateForIOS{
            get { return (bool)GetValue(AutoRotateForIOSProperty); }
            set { SetValue(AutoRotateForIOSProperty, value); }
        }

        public virtual void RunPresentationAnimation() {}
        public virtual void RunDismissalAnimation() {}
        public virtual void Destroy() {}
        internal Action LayoutNative { get; set; }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if(propertyName == HeightRequestProperty.PropertyName ||
               propertyName == WidthRequestProperty.PropertyName)
            {
                var width = WidthRequest < 0 ? Width : WidthRequest;
                var height = HeightRequest < 0 ? Height : HeightRequest;

                Layout(new Rectangle(X, Y, width, height));
                LayoutNative?.Invoke();
            }
        }

        internal static class InstanceCreator<TInstance>
        {
            public static Func<TInstance> Create { get; } = CreateInstance();

            static Func<TInstance> CreateInstance()
            {
                return Expression.Lambda<Func<TInstance>>(Expression.New(typeof(TInstance))).Compile();
            }
        }
    }
}
