using System;
using SocketFlow.DataWrappers;

namespace SocketFlow
{
    public struct WrapperInfo
    {
        public Type Type;
        public IDataWrapper<object> DataWrapper;

        public WrapperInfo(Type type, IDataWrapper<object> wrapper)
        {
            Type = type;
            DataWrapper = wrapper;
        }
    }
}
