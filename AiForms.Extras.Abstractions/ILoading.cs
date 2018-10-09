using System;

namespace AiForms.Extras.Abstractions
{
    public interface ILoading
    {
        IDisposable Start(string message = null,bool isCurrentScope = false);
        IDisposable Start(out IProgress<double> progress,string message = null,bool isCurrentScope = false);
        void Show(string message = null,bool isCurrentScope = false);
        void Show(out IProgress<double> progress, string message = null,bool isCurrentScope = false);
        void Hide();
        void SetMessage(string message);
    }

    public interface IIndicatorView
    {
        void StartAnimating();
        void StopAnimating();
    }
}
