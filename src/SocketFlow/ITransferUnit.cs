using System.Threading.Tasks;

namespace SocketFlow
{
    public interface ITransferUnit<in TKey>
    {
        Task Send<TValue>(TKey key, TValue value);
    }
}
