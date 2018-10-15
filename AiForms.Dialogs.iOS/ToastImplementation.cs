using System;
using AiForms.Dialogs.Abstractions;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace AiForms.Dialogs
{
    [Foundation.Preserve(AllMembers = true)]
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

            var renderer = Dialogs.CreateNativeView(view);

            var measure = Dialogs.Measure(view);
            renderer.SetElementSize(measure);
          
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
                view.RunDismissalAnimation();
                UIView.Animate(
                    0.5,
                    () => renderer.NativeView.Alpha = 0,
                    () =>{
                        view.Parent = null;
                        renderer.NativeView.RemoveConstraints(renderer.NativeView.Constraints);
                        Dialogs.DisposeModelAndChildrenRenderers(view);
                        renderer = null;
                        view.Destroy();
                        view = null;
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

            var width = view.ViewWidth;
            var height = view.ViewHeight;

            var parentRect = window.Frame;

            var fWidth = width <= 1 ? parentRect.Width * width : width;
            var fHeight = height <= 1 ? parentRect.Height * height : height;

            nativeView.WidthAnchor.ConstraintEqualTo((System.nfloat)view.Bounds.Width).Active = true;
            nativeView.HeightAnchor.ConstraintEqualTo((System.nfloat)view.Bounds.Height).Active = true;

            switch(view.VerticalLayoutAlignment){
                case Xamarin.Forms.LayoutAlignment.Start:
                    nativeView.TopAnchor.ConstraintEqualTo(window.TopAnchor, view.OffsetY).Active = true;
                    break;
                case Xamarin.Forms.LayoutAlignment.End:
                    nativeView.BottomAnchor.ConstraintEqualTo(window.BottomAnchor, view.OffsetY).Active = true;
                    break;
                default:
                    nativeView.CenterYAnchor.ConstraintEqualTo(window.CenterYAnchor, view.OffsetY).Active = true;
                    break;
            }

            switch(view.HorizontalLayoutAlignment){
                case Xamarin.Forms.LayoutAlignment.Start:
                    nativeView.LeftAnchor.ConstraintEqualTo(window.LeftAnchor, view.OffsetX).Active = true;
                    break;
                case Xamarin.Forms.LayoutAlignment.End:
                    nativeView.RightAnchor.ConstraintEqualTo(window.RightAnchor, view.OffsetX).Active = true;
                    break;
                default:
                    nativeView.CenterXAnchor.ConstraintEqualTo(window.CenterXAnchor, view.OffsetX).Active = true;
                    break;
            }
        }

    }

}
