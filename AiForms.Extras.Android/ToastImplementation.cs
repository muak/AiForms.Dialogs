using AiForms.Extras.Abstractions;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Xamarin.Forms.Platform.Android;
using XF = Xamarin.Forms;
using System.Threading.Tasks;

namespace AiForms.Extras
{
    [Android.Runtime.Preserve(AllMembers = true)]
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
            view.Parent = XF.Application.Current.MainPage;
            view.BindingContext = viewModel;

            var toast = new Android.Widget.Toast(Extras.Context);

            toast.SetGravity(view.VerticalLayoutAlignment.ToNativeVertical(),view.OffsetX,view.OffsetY);
            toast.Duration = Android.Widget.ToastLength.Long;

            var renderer = Extras.CreateNativeView(view);

            var measure = Extras.Measure(view);

            view.Layout(new XF.Rectangle(new XF.Point(0,0),measure));

            var realW = (int)Extras.Context.ToPixels(measure.Width);
            var realH = (int)Extras.Context.ToPixels(measure.Height);

            var layout = new LinearLayout(Extras.Context);
            using (var param = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
            {
                Width = realW,
                Height = realH
            }){
                layout.LayoutParameters = param;
            }

            using(var param = new LinearLayout.LayoutParams(realW,realH){
                Width = realW,Height = realH
            }){
                layout.AddView(renderer.View, param);
            }

            if (view.CornerRadius > 0)
            {
                var border = new GradientDrawable();
                border.SetCornerRadius(Extras.Context.ToPixels(view.CornerRadius));
                if (!view.BackgroundColor.IsDefault)
                {
                    border.SetColor(view.BackgroundColor.ToAndroid());
                    border.Alpha = (int)(view.Opacity * 255);
                }
                layout.ClipToOutline = true;
                layout.SetBackground(border);
            }

            toast.View = layout;

            renderer.UpdateLayout();
            renderer.View.Invalidate();

            view.RunPresentationAnimation();

            toast.Show();

            var duration = Math.Max(Math.Min(view.Duration - 250, 3500),0);

            var handler = new Handler();
            handler.PostDelayed(new Runnable(async () =>
            {
                view.RunDismissalAnimation();
                await Task.Delay(250);
                toast?.Cancel();

                view.Parent = null;

                if (!renderer.View.IsDisposed())
                {
                    renderer.View.RemoveFromParent();
                    renderer.View.Dispose();
                }

                layout.Dispose();
                toast.View = null;

                renderer.Dispose();
                renderer = null;
                toast?.Dispose();
                toast = null;

                view.Destroy();
            }),duration);
        }
    }
}
