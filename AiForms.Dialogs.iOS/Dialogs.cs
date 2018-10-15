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

            var fWidth = view.ViewWidth <= 1 ? dWidth * view.ViewWidth : view.ViewWidth;
            var fHeight = view.ViewHeight <= 1 ? dHeight * view.ViewHeight : view.ViewHeight;

            if (view.ViewWidth < 0 || view.ViewHeight < 0)
            {
                var requestWidth = view.ViewWidth < 0 ? dWidth : fWidth;
                var requestHeight = view.ViewHeight < 0 ? dHeight : fHeight;

                var sizeRequest = view.Measure(requestWidth, requestHeight);

                return new Size(sizeRequest.Request.Width, sizeRequest.Request.Height);
            }

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
