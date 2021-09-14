using TracerLib.Models;

namespace TracerLib.Tracers
{
    public interface ITracer
    {
        void StartTrace();

        void StopTrace();

        TraceResult GetTraceResult();
    }
}