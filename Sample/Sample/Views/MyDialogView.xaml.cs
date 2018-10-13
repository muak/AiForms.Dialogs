using System;
using System.Collections.Generic;
using Xamarin.Forms;
using AiForms.Dialogs;
using System.Threading.Tasks;
using AiForms.Dialogs.Abstractions;

namespace Sample.Views
{
    public partial class MyDialogView : DialogView
    {
        public MyDialogView()
        {
            InitializeComponent();
            OverlayColor = Color.FromRgba(1, 0, 0, 0.2);
            //BackgroundColor = Color.Transparent;

            //this.Scale = 0.3;
        }

        public override void SetUp()
        {
            title.TranslationY = -100;
            title.Scale = 0.5;
            desc.TranslationY = 200;
            desc.Scale = 0.5;
            container.Scale = 0.3;
            container.Opacity = 1;
        }

        public override async void RunPresentationAnimation()
        {
            await Task.WhenAll(
                //desc.RotateTo(360,500),
                title.ScaleTo(1.0,250),
                title.TranslateTo(0,0,250),
                desc.ScaleTo(1.0, 250),
                desc.TranslateTo(0,0, 250),
                container.ScaleTo(1.0,250)
                //this.ScaleTo(1.0,125)
            );
        }


        public override async void RunDismissalAnimation()
        {
            await Task.WhenAll(
                title.ScaleTo(0, 150),
                //title.TranslateTo(0, -100, 150),
                desc.ScaleTo(0, 150),
                //desc.TranslateTo(0, 200, 150),
                //this.ScaleTo(0.3, 250)
                container.ScaleTo(0.0,250),
                container.FadeTo(0,250)
            );
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            DialogNotifier.Complete();
        }
    }
}
