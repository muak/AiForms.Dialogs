using System;
using System.Reflection;
using AiForms.Dialogs.Abstractions;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;

namespace AiForms.Dialogs
{
    [Foundation.Preserve(AllMembers = true)]
    public static class Dialogs
    {
        public static void Init(){}

        // Get internal members
        static BindableProperty RendererProperty = (BindableProperty)typeof(Platform).GetField("RendererProperty", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null);
        static Type DefaultRenderer = typeof(Platform).Assembly.GetType("Xamarin.Forms.Platform.iOS.Platform+DefaultRenderer");
        static Type ModalWrapper = typeof(Platform).Assembly.GetType("Xamarin.Forms.Platform.iOS.ModalWrapper");
        static MethodInfo ModalWapperDispose = ModalWrapper.GetMethod("Dispose");

        internal static UIViewController RootViewController{
            get{
                var vc = UIApplication.SharedApplication.KeyWindow.RootViewController;
                while (vc.PresentedViewController != null)
                {
                    vc = vc.PresentedViewController;
                }
                return vc;
            }
        }

        internal static IVisualElementRenderer CreateNativeView(VisualElement view)
        {
            IVisualElementRenderer renderer = Platform.GetRenderer(view);
            if(renderer == null){
                renderer = Platform.CreateRenderer(view);
                Platform.SetRenderer(view, renderer);
            }

            renderer.NativeView.AutoresizingMask = UIViewAutoresizing.All;
            renderer.NativeView.ContentMode = UIViewContentMode.ScaleToFill;

            return renderer;
        }

        internal static Size Measure(ExtraView view)
        {
            var window = UIApplication.SharedApplication.KeyWindow;

            var dWidth = window.Bounds.Width;
            var dHeight = window.Bounds.Height;

            double fWidth = dWidth;
            if(view.ProportionalWidth >= 0)
            {
                fWidth = dWidth * view.ProportionalWidth;
            }
            else if (view.HorizontalLayoutAlignment == LayoutAlignment.Fill)
            {
                fWidth = dWidth;
            }
            else if(view.WidthRequest == -1)
            {
                fWidth = double.PositiveInfinity;
            }
            else if(view.WidthRequest >= 0)
            {
                fWidth = view.WidthRequest;
            }

            double fHeight = dHeight;
            if(view.ProportionalHeight >= 0)
            {
                fHeight = dHeight * view.ProportionalHeight;
            }
            else if (view.VerticalLayoutAlignment == LayoutAlignment.Fill)
            {
                fHeight = dHeight;
            }
            else if(view.HeightRequest == -1)
            {
                fHeight = double.PositiveInfinity;
            }
            else if (view.HeightRequest >= 0)
            {
                fHeight = view.HeightRequest;
            }

            if (view.ProportionalWidth < 0 || view.ProportionalHeight < 0)
            {
                var sizeRequest = view.Measure(fWidth, fHeight, MeasureFlags.IncludeMargins);

                var reqWidth = view.ProportionalWidth >= 0 ? fWidth : sizeRequest.Request.Width;
                var reqHeight = view.ProportionalHeight >= 0 ? fHeight : sizeRequest.Request.Height;

                return new Size(reqWidth,reqHeight);
            }

            // If both width and height are proportional, Measure is not called.
            return new Size(fWidth, fHeight);
        }

        internal static CGRect GetCurrentPageRect(UIView parentView)
        {
            var activePage = Application.Current.MainPage.GetActivePage();
            var activeRenderer = Platform.GetRenderer(activePage);

            var rect = activeRenderer.NativeView.ConvertRectToView(activeRenderer.NativeView.Bounds, parentView);

            activeRenderer = null;

            return rect;
        }

        internal static UIView GetCurrentPageView()
        {
            var activePage = Application.Current.MainPage.GetActivePage();
            var activeRenderer = Platform.GetRenderer(activePage);

            var view = activeRenderer.NativeView;

            activeRenderer = null;

            return view;
        }

        internal static void SetLayoutAlignment(UIView targetView,UIView parentView,ExtraView dialog)
        {
            switch (dialog.VerticalLayoutAlignment)
            {
                case Xamarin.Forms.LayoutAlignment.Start:
                    targetView.TopAnchor.ConstraintEqualTo(parentView.TopAnchor, dialog.OffsetY).Active = true;
                    break;
                case Xamarin.Forms.LayoutAlignment.End:
                    targetView.BottomAnchor.ConstraintEqualTo(parentView.BottomAnchor, dialog.OffsetY).Active = true;
                    break;
                default:
                    targetView.CenterYAnchor.ConstraintEqualTo(parentView.CenterYAnchor, dialog.OffsetY).Active = true;
                    break;
            }

            switch (dialog.HorizontalLayoutAlignment)
            {
                case Xamarin.Forms.LayoutAlignment.Start:
                    targetView.LeftAnchor.ConstraintEqualTo(parentView.LeftAnchor, dialog.OffsetX).Active = true;
                    break;
                case Xamarin.Forms.LayoutAlignment.End:
                    targetView.RightAnchor.ConstraintEqualTo(parentView.RightAnchor, dialog.OffsetX).Active = true;
                    break;
                default:
                    targetView.CenterXAnchor.ConstraintEqualTo(parentView.CenterXAnchor, dialog.OffsetX).Active = true;
                    break;
            }
        }

        // From internal Platform class
        internal static void DisposeModelAndChildrenRenderers(Element view)
        {
            IVisualElementRenderer renderer;
            foreach (VisualElement child in view.Descendants())
            {
                renderer = Platform.GetRenderer(child);
                child.ClearValue(RendererProperty);

                if (renderer != null)
                {
                    renderer.NativeView.RemoveFromSuperview();
                    renderer.Dispose();
                }
            }

            renderer = Platform.GetRenderer((VisualElement)view);
            if (renderer != null)
            {
                if (renderer.ViewController != null)
                {
                    if (renderer.ViewController.ParentViewController.GetType() == ModalWrapper)
                    {
                        var modalWrapper = Convert.ChangeType(renderer.ViewController.ParentViewController, ModalWrapper);
                        ModalWapperDispose.Invoke(modalWrapper, new object[] { });
                    }
                }

                renderer.NativeView.RemoveFromSuperview();
                renderer.Dispose();
            }

            view.ClearValue(RendererProperty);
        }
    }
}
