using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AiForms.Extras.Abstractions
{
    public class LoadingConfig
    {
        public ContentView IndicatorView { get; set; }
        public double IndicatorViewWidth { get; set; } = 50;
        public double IndicatorViewHeight { get; set; } = 50;
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public Color IndicatorColor { get; set; } = Color.White;

        public double FontSize { get; set; } = 14;
        public Color FontColor { get; set; } = Color.White;

        public Color OverlayColor { get; set; } = Color.FromRgb(0,0,0);
        public double Opacity { get; set; } = 0.5;

        public string DefaultMessage { get; set; }
        public string ProgressMessageFormat { get; set; } = "{0}\n{1:P0}"; // 0:message, 1:progress
    }
}
