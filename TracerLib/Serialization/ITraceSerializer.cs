using TracerLib.Models;

namespace TracerLib.Serialization
{
    public interface ITraceSerializer
    {
        string Serialize(TraceResult traceResult);
        TraceResult Deserialize(string traceResult);
    }
}