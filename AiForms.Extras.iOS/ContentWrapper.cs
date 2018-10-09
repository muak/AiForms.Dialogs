using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;

namespace AiForms.Extras
{
    public class ContentWrapper:UIView,INativeElementView
    {
        WeakReference<IVisualElementRenderer> _rendererRef;
        ContentView _contentView;

        Element INativeElementView.Element => _contentView;
        internal bool SupressSeparator { get; set; }
        bool _disposed;

        public ContentWrapper(ContentView contentView)
        {
            AutoresizingMask = UIViewAutoresizing.All;
            SetContent(contentView);
        }

        public override void LayoutSubviews()
        {
            var reference = Guid.NewGuid().ToString();
            Performance.Start(reference);

            //This sets the content views frame.
            base.LayoutSubviews();


            var contentFrame = Superview.Frame;
            var view = _contentView;

            Layout.LayoutChildIntoBoundingRegion(view, contentFrame.ToRectangle());

            if (_rendererRef == null)
                return;

            IVisualElementRenderer renderer;
            if (_rendererRef.TryGetTarget(out renderer))
            {
                Frame = view.Bounds.ToRectangleF();
                renderer.NativeView.Frame = view.Bounds.ToRectangleF();
            }

            renderer.NativeView.SetNeedsLayout();

            var fff = this.Frame;
            Performance.Stop(reference);
        }

        public override SizeF SizeThatFits(SizeF size)
        {
            var reference = Guid.NewGuid().ToString();
            Performance.Start(reference);

            IVisualElementRenderer renderer;
            if (!_rendererRef.TryGetTarget(out renderer))
                return base.SizeThatFits(size);

            if (renderer.Element == null)
                return SizeF.Empty;

            double width = size.Width;
            var height = size.Height > 0 ? size.Height : double.PositiveInfinity;
            var result = renderer.Element.Measure(width, height, MeasureFlags.IncludeMargins);

            // make sure to add in the separator if needed
            var finalheight = (float)result.Request.Height / UIScreen.MainScreen.Scale;

            Performance.Stop(reference);

            return new SizeF(size.Width, finalheight);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                IVisualElementRenderer renderer;
                if (_rendererRef != null && _rendererRef.TryGetTarget(out renderer) && renderer.Element != null)
                {
                    renderer.Dispose();
                    _rendererRef = null;
                }

                _contentView = null;
            }

            _disposed = true;

            base.Dispose(disposing);
        }

        IVisualElementRenderer GetNewRenderer()
        {
            var newRenderer = Platform.CreateRenderer(_contentView);

            newRenderer.NativeView.AutoresizingMask = UIViewAutoresizing.All;
            newRenderer.NativeView.ContentMode = UIViewContentMode.ScaleToFill;

            _rendererRef = new WeakReference<IVisualElementRenderer>(newRenderer);
            AddSubview(newRenderer.NativeView);

            return newRenderer;
        }

        void SetContent(ContentView view)
        {
            var reference = Guid.NewGuid().ToString();
            Performance.Start(reference);

            _contentView = view;

            IVisualElementRenderer renderer;
            renderer = GetNewRenderer();

            Platform.SetRenderer(_contentView, renderer);
            Performance.Stop(reference);
        }
    }
}
