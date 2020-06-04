using System;
using Xamarin.Forms;
namespace Sample.ViewModels
{
    public class DialogTestViewModel
    {
        public double ProportionalWidth { get; set; } = -1d;
        public double ProportionalHeight { get; set; } = -1d;
        public double HeightRequest { get; set; } = -1d;
        public LayoutAlignment VerticalLayoutAlignment { get; set; } = LayoutAlignment.Center;
        public LayoutAlignment HorizontalLayoutAlignment { get; set; } = LayoutAlignment.Center;
        public int OffsetX { get; set; } = 0;
        public int OffsetY { get; set; } = 0;
        public float CornerRadius { get; set; } = 0f;

        public bool IsCanceledOnTouchOutside { get; set; } = true;
        public Color OverlayColor { get; set; }
        public bool UseCurrentPageLocation { get; set; }

        public int Duration { get; set; } = 1500;

        public string Message { get; set; }

        public string Description {
            get {
                return CreateDescriptionString();
            }
        }

        public DialogTestViewModel()
        {
        }

        public void Reset()
        {
            ProportionalWidth = -1d;
            ProportionalHeight = -1d;
            HeightRequest = -1d;
            Message = string.Empty;
            Duration = 1500;
            VerticalLayoutAlignment = LayoutAlignment.Center;
            HorizontalLayoutAlignment = LayoutAlignment.Center;
            OffsetX = 0;
            OffsetY = 0;
            CornerRadius = 0;
            UseCurrentPageLocation = false;
            OverlayColor = Color.Default;
            IsCanceledOnTouchOutside = true;
        }

        public void SetLayout(LayoutAlignment vertical,LayoutAlignment horizontal,int offset = 0)
        {
            VerticalLayoutAlignment = vertical;
            HorizontalLayoutAlignment = horizontal;
            OffsetX = offset;
            OffsetY = offset;
        }

        string CreateDescriptionString()
        {
            return $"{GetVAlignString()} / {GetHAlignString()}\nOffset: {OffsetX} / {OffsetY}\nPSize: {ProportionalWidth} / {ProportionalHeight}\nCorner: {CornerRadius}";
        }

        string GetHAlignString()
        {
            switch(HorizontalLayoutAlignment)
            {
                case LayoutAlignment.Start:
                    return "Left";
                case LayoutAlignment.End:
                    return "Right";
                default:
                    return "Center";
            }
        }

        string GetVAlignString()
        {
            switch (VerticalLayoutAlignment)
            {
                case LayoutAlignment.Start:
                    return "Top";
                case LayoutAlignment.End:
                    return "Bottom";
                default:
                    return "Center";
            }
        }
    }
}
