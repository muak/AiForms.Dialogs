using System;

namespace AiForms.Extras.Abstractions
{
    public interface IToast
    {
        void Show<TView>(object viewModel = null) where TView : ToastView, new();
        void Show(ToastView view,object viewModel =null);
    }
}
