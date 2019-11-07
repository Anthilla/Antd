using System;
using System.Threading.Tasks;

namespace Antd.Fuse {
    public interface IFuseMount : IDisposable {
        Task WaitForUnmountAsync();
        void LazyUnmount();
        Task<bool> UnmountAsync(int timeout = -1);
    }
}