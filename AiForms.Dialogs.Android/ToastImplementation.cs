using System.Threading.Tasks;
using AiForms.Dialogs.Abstractions;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Xamarin.Forms.Platform.Android;
using XF = Xamarin.Forms;

namespace AiForms.Dialogs
{
    [Android.Runtime.Preserve(AllMembers = true)]
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
            view.Parent = XF.Application.Current.MainPage;
            view.BindingContext = viewModel;

            var toast = new Android.Widget.Toast(Dialogs.Context);

            var offsetX = (int)Dialogs.Context.ToPixels(view.OffsetX);
            var offsetY = (int)Dialogs.Context.ToPixels(view.OffsetY);

            // HACK: For some reason, the offset direction is reversed when GravityFlags contains Left or Bottom.
            if(view.HorizontalLayoutAlignment == XF.LayoutAlignment.End)
            {
                offsetX *= -1;
            }
            if(view.VerticalLayoutAlignment == XF.LayoutAlignment.End)
            {
                offsetY *= -1;
            }

            toast.SetGravity(Dialogs.GetGravity(view), offsetX, offsetY);
            toast.Duration = Android.Widget.ToastLength.Long;

            var renderer = Dialogs.CreateNativeView(view);

            var measure = Dialogs.Measure(view);

            view.Layout(new XF.Rectangle(new XF.Point(0,0),measure));

            var realW = (int)Dialogs.Context.ToPixels(measure.Width);
            var realH = (int)Dialogs.Context.ToPixels(measure.Height);

            var layout = new LinearLayout(Dialogs.Context);
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
                border.SetCornerRadius(Dialogs.Context.ToPixels(view.CornerRadius));
                if (!view.BackgroundColor.IsDefault)
                {
                    border.SetColor(view.BackgroundColor.ToAndroid());
                    border.Alpha = (int)(view.Opacity * 255);
                }
                layout.ClipToOutline = true;
                layout.SetBackground(border);
            }
            
            toast.View = layout;

            view.RunPresentationAnimation();

            toast.Show();

            var duration = Math.Max(Math.Min(view.Duration - 260, 3500),0); // give a bit millisecond margin 

            var handler = new Handler();

            handler.PostDelayed(new Runnable(view.RunDismissalAnimation),duration);

            handler.PostDelayed(new Runnable(() =>
            {
                //view.RunDismissalAnimation();
                //await Task.Delay(250);
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
                view.BindingContext = null;
            }), view.Duration);
        }
    }
}
