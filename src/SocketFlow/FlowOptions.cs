namespace SocketFlow
{
    public class FlowOptions
    {
        public bool DefaultNonPrimitivesObjectUsingAsJson = false;

        public static readonly FlowOptions Lazy = new FlowOptions()
        {
            DefaultNonPrimitivesObjectUsingAsJson = true
        };
    }
}
