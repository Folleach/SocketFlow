using System;
using System.Collections.Generic;
using System.Text;

namespace SocketFlow.Server.Modules
{
    public interface IModule
    {
        void Initialize<T>(SocketFlowServer<T> server);
    }
}
