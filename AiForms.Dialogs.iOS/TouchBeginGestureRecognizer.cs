using System;
using UIKit;
namespace AiForms.Dialogs
{
    /// <summary>
    /// Touch begin gesture recognizer.
    /// </summary>
    public class TouchBeginGestureRecognizer:UIGestureRecognizer
    {
        public override void TouchesBegan(Foundation.NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            // Touch to recognize immediately
            State = UIGestureRecognizerState.Recognized;
        }
    }
}
