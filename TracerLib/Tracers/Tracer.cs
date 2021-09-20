using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using TracerLib.Models;

namespace TracerLib.Tracers
{
    public class Tracer : ITracer
    {
        private readonly ConcurrentDictionary<int, ThreadTracer> _threadsTrace = new ();

        public void StartTrace()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            if (!_threadsTrace.TryGetValue(threadId, out var threadTrace))
            {
                threadTrace = _threadsTrace.GetOrAdd(threadId, new ThreadTracer());
            }
            threadTrace.StartTrace();
        }

        public void StopTrace()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            if (_threadsTrace.TryGetValue(threadId, out var threadTrace))
            {
                threadTrace.StopTrace(new StackTrace().GetFrame(1));
            }
        }

        public TraceResult GetTraceResult()
        {
            var traceResult = new TraceResult();
            foreach (var (id, value) in _threadsTrace)
            {
                var threadInfo = new ThreadInfo
                {
                    Id = id,
                    Methods = value.MethodsInfo.ToList()
                };
                traceResult.ThreadsInfo.Add(threadInfo);
            }
            return traceResult;
        }
        
    }
}