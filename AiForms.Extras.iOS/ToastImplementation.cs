using System;
using AiForms.Extras.Abstractions;
using CoreGraphics;
using UIKit;
using Foundation;
using Xamarin.Forms;
using System.Threading.Tasks;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Platform.iOS;

namespace AiForms.Extras
{
    public class ToastImplementation:IToast
    {
        public ToastImplementation()
        {
        }

        public void Show<TView>(object viewModel = null) where TView : ToastView, new()
        {
            var view = new TView();
            Show(view, viewModel);
        }

        public void Show(ToastView view, object viewModel = null)
        {
            view.BindingContext = viewModel;

            view.Parent = Application.Current.MainPage;

            var renderer = Extras.CreateNativeView(view);

          
            renderer.NativeView.Alpha = 0;
            if (view.CornerRadius > 0)
            {
                renderer.NativeView.Layer.CornerRadius = view.CornerRadius;
                renderer.NativeView.Layer.MasksToBounds = true;
            }

            SetView(view,renderer.NativeView,renderer);

            view.Parent = null;

            view.RunPresentationAnimation();
            UIView.Animate(
                0.25,
                () => renderer.NativeView.Alpha = (System.nfloat)view.Opacity
            );

            Device.StartTimer(TimeSpan.FromMilliseconds(view.Duration), () =>
            {
                UIView.Animate(
                    0.5,
                    () => renderer.NativeView.Alpha = 0,
                    () =>{
                        renderer.NativeView.RemoveFromSuperview();
                        renderer.Dispose();
                        view.Destroy();
                    }
                );

                return false;
            });
        }

        void SetView(ToastView view,UIView nativeView,IVisualElementRenderer renderer)
        {
            var window = UIApplication.SharedApplication.KeyWindow;

            nativeView.TranslatesAutoresizingMaskIntoConstraints = false;

            window.AddSubview(nativeView);

            var width = view.ToastWidth;
            var height = view.ToastHeight;

            var parentRect = window.Frame;

            var fWidth = width <= 1 ? parentRect.Width * width : width;
            var fHeight = height <= 1 ? parentRect.Height * height : height;

            var size = view.Measure(double.PositiveInfinity,double.PositiveInfinity);
            renderer.SetElementSize(new Size(size.Request.Width, size.Request.Height));
            view.Layout(new Xamarin.Forms.Rectangle(0, 0, fWidth, fHeight));

            if(width <= 1){
                nativeView.WidthAnchor.ConstraintEqualTo(window.WidthAnchor, (System.nfloat)width).Active = true;
            }
            else{
                nativeView.WidthAnchor.ConstraintEqualTo((System.nfloat)width).Active = true;
            }

            if(height <= 1){
                nativeView.HeightAnchor.ConstraintEqualTo(window.HeightAnchor, (System.nfloat)height).Active = true;
            }
            else{
                nativeView.HeightAnchor.ConstraintEqualTo((System.nfloat)height).Active = true;
            }

            nativeView.CenterXAnchor.ConstraintEqualTo(window.CenterXAnchor, view.OffsetX).Active = true;

            switch(view.LayoutAlignment){
                case Xamarin.Forms.LayoutAlignment.Start:
                    nativeView.TopAnchor.ConstraintEqualTo(window.TopAnchor, view.OffsetY).Active = true;
                    break;
                case Xamarin.Forms.LayoutAlignment.Center:
                    nativeView.CenterYAnchor.ConstraintEqualTo(window.CenterYAnchor, view.OffsetY).Active = true;
                    break;
                case Xamarin.Forms.LayoutAlignment.End:
                    nativeView.BottomAnchor.ConstraintEqualTo(window.BottomAnchor, view.OffsetY).Active = true;
                    break;               
            }
        }

    }

}
