using System;
using Xamarin.Forms;

namespace AiForms.Dialogs.Abstractions
{
    public class ToastView : ExtraView
    {
        public int Duration { get; set; } = 1500;   // 0-3500 
    }
}
