using System;
using System.Threading.Tasks;

namespace AiForms.Dialogs.Abstractions
{
    public interface IReusableDialog: IDisposable
    {
        Task<bool> ShowAsync();
    }
}
