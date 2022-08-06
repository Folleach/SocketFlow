using System.Net.Sockets;
using System.Threading.Tasks;

namespace SocketFlow
{
    public static class NetworkStreamExtensions
    {
        public static async Task<bool> ReadAllAsync(this NetworkStream stream, byte[] buffer, int count)
        {
            var read = 0;
            try
            {
                while (read != count)
                {
                    var currentRead = await stream.ReadAsync(buffer, read, count - read);
                    if (currentRead == 0)
                        return false;
                    read += currentRead;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static async Task WriteAllAsync(this NetworkStream stream, byte[] data)
        {
            await stream.WriteAsync(data, 0, data.Length);
        }
    }
}
