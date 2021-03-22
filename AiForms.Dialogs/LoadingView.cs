using System;
using Xamarin.Forms;
using System.Security.Cryptography.X509Certificates;

namespace AiForms.Dialogs.Abstractions
{
    public class LoadingView:ExtraView
    {
        public static BindableProperty ProgressProperty =
            BindableProperty.Create(
                nameof(Progress),
                typeof(double),
                typeof(LoadingView),
                default(double),
                defaultBindingMode: BindingMode.OneWay
            );

        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        public static BindableProperty OverlayColorProperty =
            BindableProperty.Create(
                nameof(OverlayColor),
                typeof(Color),
                typeof(LoadingView),
                Color.FromRgba(0, 0, 0, 0.2),
                defaultBindingMode: BindingMode.OneWay
            );

        public Color OverlayColor
        {
            get { return (Color)GetValue(OverlayColorProperty); }
            set { SetValue(OverlayColorProperty, value); }
        }

    }
}
