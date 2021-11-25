using System.Collections.Concurrent;

namespace TracerLib.Tracers
{
    public interface IMainTracer : ITracer
    {
        public ConcurrentDictionary<int, IThreadTracer> ThreadsTracers { get; }
    }
}