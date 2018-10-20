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

        public void Show<TView>(object viewModel = null) where TView : ToastView
        {
            var view = ExtraView.InstanceCreator<TView>.Create();
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

            Device.StartTimer(TimeSpan.FromMilliseconds(Math.Max(view.Duration - 250, 0)), () =>
            {
                view.RunDismissalAnimation();
                UIView.Animate(
                    0.25,
                    () => renderer.NativeView.Alpha = 0,
                    () =>{
                        view.Parent = null;
                        renderer.NativeView.RemoveConstraints(renderer.NativeView.Constraints);
                        Dialogs.DisposeModelAndChildrenRenderers(view);
                        renderer = null;
                        view.Destroy();
                        view.BindingContext = null;
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

            nativeView.WidthAnchor.ConstraintEqualTo((System.nfloat)view.Bounds.Width).Active = true;
            nativeView.HeightAnchor.ConstraintEqualTo((System.nfloat)view.Bounds.Height).Active = true;

            Dialogs.SetLayoutAlignment(nativeView, window, view);
        }

    }

}
