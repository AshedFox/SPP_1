using System.Collections.Concurrent;
using System.Diagnostics;
using TracerLib.Models;

namespace TracerLib.Tracers
{
    public interface IMethodTracer : ITracer
    {
        public Stopwatch Stopwatch { get; }
        public ConcurrentStack<MethodInfo> Methods { get; }
    }
}