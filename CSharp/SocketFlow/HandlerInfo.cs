using System.Reflection;

namespace SocketFlow
{
    public struct HandlerInfo
    {
        public MethodInfo Method;
        public object Target;

        public HandlerInfo(MethodInfo method, object target)
        {
            Method = method;
            Target = target;
        }
    }
}
