using System;
using AiForms.Dialogs.Abstractions;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using XF = Xamarin.Forms;

namespace AiForms.Dialogs
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public static class Dialogs
    {
        internal static Context Context;
        public static void Init(Context context)
        {
            Context = context;
        }

        internal static FragmentManager FragmentManager => (Context as Activity)?.FragmentManager;

        internal static IVisualElementRenderer CreateNativeView(XF.View view)
        {
            IVisualElementRenderer renderer = Platform.GetRenderer(view);
            if (renderer == null)
            {
                renderer = Platform.CreateRendererWithContext(view,Context);
                Platform.SetRenderer(view, renderer);
            }

            return renderer;
        }

        internal static Android.App.Dialog CreateFullScreenTransparentDialog(View contentView)
        {
            var dialog = new Android.App.Dialog(Context, Resource.Style.NoDimDialogFragmentStyle);

            dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            dialog.SetContentView(contentView);

            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));
            dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            
            return dialog;
        }

        internal static Xamarin.Forms.Size Measure(ExtraView view)
        {
            var display = (Context as Activity)?.WindowManager.DefaultDisplay;

            Point size = new Point();
            display.GetSize(size);

            var dWidth = Context.FromPixels(size.X);
            var dHeight = Context.FromPixels(size.Y);

            var fWidth = view.ViewWidth <= 1 ? dWidth * view.ViewWidth : view.ViewWidth;
            var fHeight = view.ViewHeight <= 1 ? dHeight * view.ViewHeight : view.ViewHeight;

            if(view.ViewWidth < 0 || view.ViewHeight < 0)
            {
                var requestWidth = view.ViewWidth < 0 ? dWidth : fWidth;
                var requestHeight = view.ViewHeight < 0 ? dHeight : fHeight;

                var sizeRequest = view.Measure(requestWidth, requestHeight);

                return new XF.Size(sizeRequest.Request.Width, sizeRequest.Request.Height);
            }

            return new XF.Size(fWidth, fHeight);
        }

        internal static void SetOffsetMargin(FrameLayout.LayoutParams layoutParams, ExtraView view)
        {
            var offsetX = (int)Context.ToPixels(view.OffsetX);
            var offsetY = (int)Context.ToPixels(view.OffsetY);

            layoutParams.LeftMargin = offsetX;
            layoutParams.TopMargin = offsetY;
        }

        internal static void SetOffsetMargin(FrameLayout.LayoutParams layoutParams, int offsetX,int offsetY)
        {
            layoutParams.LeftMargin = (int)Context.ToPixels(offsetX);
            layoutParams.TopMargin = (int)Context.ToPixels(offsetY);
        }

        internal static GravityFlags GetGravity(ExtraView view)
        {
            GravityFlags gravity = GravityFlags.NoGravity;
            switch (view.VerticalLayoutAlignment)
            {
                case XF.LayoutAlignment.Start:
                    gravity |= GravityFlags.Top;
                    break;
                case XF.LayoutAlignment.End:
                    gravity |= GravityFlags.Bottom;
                    break;
                default:
                    gravity |= GravityFlags.CenterVertical;
                    break;
            }

            switch (view.HorizontalLayoutAlignment)
            {
                case XF.LayoutAlignment.Start:
                    gravity |= GravityFlags.Left;
                    break;
                case XF.LayoutAlignment.End:
                    gravity |= GravityFlags.Right;
                    break;
                default:
                    gravity |= GravityFlags.CenterHorizontal;
                    break;
            }

            return gravity;
        }

        internal static (int top, int bottom) CalcWindowPadding()
        {
            var display = (Context as Activity)?.WindowManager.DefaultDisplay;

            Point size = new Point();
            display.GetSize(size);

            var activePage = XF.Application.Current.MainPage.GetActivePage();
            var activeRenderer = Platform.GetRenderer(activePage);

            var rect = new Rect();
            activeRenderer.View.GetGlobalVisibleRect(rect);

            activeRenderer = null;

            return (rect.Top, size.Y - rect.Bottom);
        }
    }
}
