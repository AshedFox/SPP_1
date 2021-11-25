using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TracerLib.Models;

namespace TracerLib.Tracers
{
    public class MethodTracer : IMethodTracer
    {
        public Stopwatch Stopwatch { get; } = new ();
        public ConcurrentStack<MethodInfo> Methods { get; } = new();

        public void StartTrace()
        {
            Stopwatch.Start();
        }

        public void StopTrace()
        {
            Stopwatch.Stop();
        }

        public ITraceResult GetTraceResult()
        {
            return new MethodInfo()
            {
                ElapsedMilliseconds = Stopwatch.ElapsedMilliseconds,
                Methods = Methods.ToList()
            };
        }
    }
}