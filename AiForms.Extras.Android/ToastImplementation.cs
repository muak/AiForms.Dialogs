using System;
using AiForms.Extras.Abstractions;
using Android.Content;
using System.Threading.Tasks;
using Android.OS;
using Android.Runtime;
using Java.Lang;
using Android.Widget;
using Android.Views;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;

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
            view.Parent = Xamarin.Forms.Application.Current.MainPage;
            view.BindingContext = viewModel;

            var toast = new Android.Widget.Toast(Extras.Context);

            toast.SetGravity(view.LayoutAlignment.ToNativeVertical(),view.OffsetX,view.OffsetY);
            toast.Duration = Android.Widget.ToastLength.Long;

            var renderer = Extras.CreateNativeView(view);

            var measure = view.Measure(double.PositiveInfinity, double.PositiveInfinity);
            var size = Extras.CalcViewSize(view.ToastWidth, view.ToastHeight);

            view.Layout(new Xamarin.Forms.Rectangle(0, 0, measure.Request.Width, measure.Request.Height));

            var realW = (int)Extras.Context.ToPixels(measure.Request.Width);
            var realH = (int)Extras.Context.ToPixels(measure.Request.Height);

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
            //toast.Show();

            renderer.UpdateLayout();
            renderer.View.Invalidate();

            view.RunPresentationAnimation();
            //new ExtraToast(toast, view.Duration).Execute();

            toast.Show();

            var handler = new Handler();
            handler.PostDelayed(new Runnable(() =>
            {
                toast?.Cancel();
            }),view.Duration);
        }
    }


    public class ExtraToast : AsyncTask<string, int, int>
    {
        Android.Widget.Toast _toast;
        long _durationMs;

        public ExtraToast(Android.Widget.Toast toast,long durationMs)
        {
            _toast = toast;
            _durationMs = durationMs;
        }

        protected override int RunInBackground(params string[] @params)
        {
            try{
                Thread.Sleep(_durationMs);
            }
            catch(InterruptedException ex)
            {
                ex.PrintStackTrace();
            }
            return 0;
        }

        protected override void OnPreExecute()
        {
            _toast.Show();
        }

        protected override void OnPostExecute(int result)
        {
            _toast.Cancel();
        }
    }


}
