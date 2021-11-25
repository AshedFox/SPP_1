using TracerLib.Models;

namespace TracerLib.Serialization
{
    public interface ITraceSerializer
    {
        string Serialize(ITraceResult traceResult);
    }
}