using System;
using System.Threading.Tasks;

namespace AiForms.Extras.Abstractions
{
    public interface IReusableDialog: IDisposable
    {
        Task<bool> ShowAsync();
    }
}
