using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AiForms.Dialogs;
using AiForms.Dialogs.Abstractions;
using Xamarin.Forms;

namespace Sample.Views
{
    public partial class MyToastView : ToastView
    {
        public MyToastView()
        {
            InitializeComponent();
            container.BackgroundColor = Color.FromRgba(0.85, 0.85, 0.85, 0.9);
            Duration = 2000;
            Opacity = 1;
            CornerRadius = 20;

            image.Source = ImageSource.FromResource("Sample.Resources.ios7-paw-outline.png");
            label.TranslationY = 50;
        }

        public override void RunPresentationAnimation()
        {
            Task.WhenAll(
                image.RotateTo(360, 250),
                image.ScaleTo(1.0, 250),
                image.TranslateTo(0, 0, 250),
                label.TranslateTo(0,0,250)
            );
        }

        public override void RunDismissalAnimation()
        {
            Task.WhenAll(
                image.ScaleTo(3.0, 250),
                label.ScaleTo(3.0, 250),
                container.FadeTo(0,250)
            );
        }

        public override void Destroy()
        {
            base.Destroy();
        }
    }
}
