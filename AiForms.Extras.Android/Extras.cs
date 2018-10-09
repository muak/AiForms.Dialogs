using AiForms.Extras.Abstractions;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Xamarin.Forms.Platform.Android;
using XF = Xamarin.Forms;

namespace AiForms.Extras
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public static class Extras
    {
        public static Context Context;
        public static void Init(Context context)
        {
            Context = context;
        }

        public static IVisualElementRenderer CreateNativeView(XF.View view)
        {
            IVisualElementRenderer renderer = Platform.GetRenderer(view);
            if (renderer == null)
            {
                renderer = Platform.CreateRendererWithContext(view,Context);
                Platform.SetRenderer(view, renderer);
            }

            return renderer;
        }

        public static Android.App.Dialog CreateFullScreenTransparentDialog(View contentView)
        {
            var dialog = new Android.App.Dialog(Context, Resource.Style.NoDimDialogFragmentStyle);

            dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            dialog.SetContentView(contentView);

            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));
            dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);

            return dialog;
        }

        public static Xamarin.Forms.Size Measure(ExtraView view)
        {
            var display = (Extras.Context as Activity)?.WindowManager.DefaultDisplay;


            Point size = new Point();
            display.GetSize(size);


            var dWidth = Context.FromPixels(size.X);
            var dHeight = Context.FromPixels(size.Y) - 24d; // 24 is statusbar height.

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

        public static (int width,int height) CalcViewSize(double width,double height)
        {
            var display = (Context as Activity)?.WindowManager.DefaultDisplay;

            Point size = new Point();
            display.GetRealSize(size);

            var dWidth = Context.FromPixels(size.X);
            var dHeight = Context.FromPixels(size.Y);

            var fWidth = width <= 1 ? dWidth * width : width;
            var fHeight = height <= 1 ? dHeight * height : height;

            return ((int)fWidth, (int)fHeight);
        }
    }
}
