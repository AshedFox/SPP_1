using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TracerLib.Models;

namespace TracerLib.Tracers
{
    public class Tracer : IMainTracer
    {
        public ConcurrentDictionary<int, IThreadTracer> ThreadsTracers { get; } = new();

        public void StartTrace()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            if (!ThreadsTracers.TryGetValue(threadId, out var threadTrace))
            {
                threadTrace = ThreadsTracers.GetOrAdd(threadId, new ThreadTracer() { Id = threadId });
            }
            threadTrace.StartTrace();
        }

        public void StopTrace()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            if (ThreadsTracers.TryGetValue(threadId, out var threadTrace))
            {
                threadTrace.StopTrace();
            }
        }

        public ITraceResult GetTraceResult()
        {
            return new TraceResult()
            {
                ThreadsInfo = ThreadsTracers.Select(pair => pair.Value.GetTraceResult() as ThreadInfo).ToList()
            };
        }
    }
}