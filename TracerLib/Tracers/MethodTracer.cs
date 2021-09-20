using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using MethodInfo = TracerLib.Models.MethodInfo;

namespace TracerLib.Tracers
{
    public class MethodTracer
    {
        public Stopwatch Stopwatch { get; } = new ();
        public ConcurrentStack<MethodInfo> Methods { get; } = new ();

        public void AddMethod(MethodInfo methodInfo)
        {
            Methods.Push(methodInfo);
        }
    }
}