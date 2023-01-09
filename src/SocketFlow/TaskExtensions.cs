using System.Threading;
using System.Threading.Tasks;

namespace SocketFlow
{
    public static class TaskExtensions
    {
        public static async Task<bool> WaitAsync(this WaitHandle waitHandle)
        {
            return await Task.Run(waitHandle.WaitOne);
        }
    }
}
