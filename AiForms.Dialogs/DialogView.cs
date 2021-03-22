using System;
using Xamarin.Forms;

namespace AiForms.Dialogs.Abstractions
{
    public class DialogView : ExtraView
    {
        public static BindableProperty IsCanceledOnTouchOutsideProperty =
            BindableProperty.Create(
                nameof(IsCanceledOnTouchOutside),
                typeof(bool),
                typeof(DialogView),
                true,
                defaultBindingMode: BindingMode.OneWay
            );

        public bool IsCanceledOnTouchOutside
        {
            get { return (bool)GetValue(IsCanceledOnTouchOutsideProperty); }
            set { SetValue(IsCanceledOnTouchOutsideProperty, value); }
        }

        public static BindableProperty OverlayColorProperty =
            BindableProperty.Create(
                nameof(OverlayColor),
                typeof(Color),
                typeof(DialogView),
                default(Color),
                defaultBindingMode: BindingMode.OneWay
            );

        public Color OverlayColor
        {
            get { return (Color)GetValue(OverlayColorProperty); }
            set { SetValue(OverlayColorProperty, value); }
        }

        public static BindableProperty UseCurrentPageLocationProperty =
            BindableProperty.Create(
                nameof(UseCurrentPageLocation),
                typeof(bool),
                typeof(DialogView),
                default(bool),
                defaultBindingMode: BindingMode.OneWay
            );

        public bool UseCurrentPageLocation
        {
            get { return (bool)GetValue(UseCurrentPageLocationProperty); }
            set { SetValue(UseCurrentPageLocationProperty, value); }
        }

        //public DialogNotifier DialogNotifier { get; } = new DialogNotifier();

        public static BindableProperty DialogNotifierProperty =
            BindableProperty.Create(
                nameof(DialogNotifier),
                typeof(IDialogNotifier),
                typeof(DialogView),
                default(IDialogNotifier),
                defaultBindingMode: BindingMode.OneWayToSource
            );

        public IDialogNotifier DialogNotifier
        {
            get { return (IDialogNotifier)GetValue(DialogNotifierProperty); }
            set { SetValue(DialogNotifierProperty, value); }
        }

        internal DialogNotifier DialogNotifierInternal => DialogNotifier as DialogNotifier;

        public virtual void SetUp() {}
        public virtual void TearDown() {}

        public DialogView()
        {
            DialogNotifier = new DialogNotifier();
        }

    }
}
