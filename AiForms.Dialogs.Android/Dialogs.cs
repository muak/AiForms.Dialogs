using System;
using System.Drawing;
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

        static int? _statusbarHeight;
        internal static int StatusBarHeight => _statusbarHeight ??=
            Context.Resources.GetDimensionPixelSize(Context.Resources.GetIdentifier("status_bar_height", "dimen", "android"));

        static int? _navigationBarHeight;
        internal static int NavigationBarHeight => _navigationBarHeight ??=
            Context.Resources.GetDimensionPixelSize(Context.Resources.GetIdentifier("navigation_bar_height", "dimen", "android"));

        static Size? _contentSize;
        internal static Size ContentSize
        {
            get
            {
                if(_contentSize != null)
                {
                    return _contentSize.Value;
                }

                Rect contentSize = new Rect();
                (Context as Activity)?.Window.DecorView.GetWindowVisibleDisplayFrame(contentSize);
                _contentSize = new Size(contentSize.Width(), contentSize.Height());
                return _contentSize.Value;
            }
        }

        internal static int DisplayHeight => StatusBarHeight + ContentSize.Height;

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
            var dWidth = Context.FromPixels(ContentSize.Width);
            var dHeight = Context.FromPixels(ContentSize.Height);

            var fWidth = view.ProportionalWidth >= 0 ? dWidth * view.ProportionalWidth : dWidth;
            var fHeight = view.ProportionalHeight >= 0 ? dHeight * view.ProportionalHeight : dHeight;

            if (view.ProportionalWidth < 0 || view.ProportionalHeight < 0)
            {
                var sizeRequest = view.Measure(fWidth, fHeight);

                var reqWidth = view.ProportionalWidth >= 0 ? fWidth : sizeRequest.Request.Width;
                var reqHeight = view.ProportionalHeight >= 0 ? fHeight : sizeRequest.Request.Height;

                return new XF.Size(reqWidth, reqHeight);
            }

            return new XF.Size(fWidth, fHeight);
        }

        internal static void SetOffsetMargin(FrameLayout.LayoutParams layoutParams, ExtraView view)
        {
            var offsetX = (int)Context.ToPixels(view.OffsetX);
            var offsetY = (int)Context.ToPixels(view.OffsetY);

            // the offset direction is reversed when GravityFlags contains Left or Bottom.
            if (view.HorizontalLayoutAlignment == XF.LayoutAlignment.End)
            {
                layoutParams.RightMargin = offsetX * -1;
            }
            else
            {
                layoutParams.LeftMargin = offsetX;
            }

            if (view.VerticalLayoutAlignment == XF.LayoutAlignment.End)
            {
                layoutParams.BottomMargin = offsetY * -1;
            }
            else
            {
                layoutParams.TopMargin = offsetY;
            }
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
            var activePage = XF.Application.Current.MainPage.GetActivePage();
            var activeRenderer = Platform.GetRenderer(activePage);

            var rect = new Rect();
            activeRenderer.View.GetGlobalVisibleRect(rect);

            return (rect.Top, DisplayHeight - rect.Bottom);
        }
    }
}
