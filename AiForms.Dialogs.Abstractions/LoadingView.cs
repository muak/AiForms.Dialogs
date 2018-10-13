using System;
using Xamarin.Forms;

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

        public Color OverlayColor { get; set; }

        public LoadingView()
        {

        }

    }
}
