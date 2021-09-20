using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using TracerLib.Models;

namespace TracerLib.Tracers
{
    public class ThreadTracer
    {
        public ConcurrentStack<MethodTracer> MethodsTraces { get; init; } = new();
        public ConcurrentStack<MethodInfo> MethodsInfo { get; init; } = new();
        public MethodTracer CurrentMethodTracer { get; set; }

        public void StartTrace()
        {
            if (CurrentMethodTracer is not null)
            {
                MethodsTraces.Push(CurrentMethodTracer);
            }

            CurrentMethodTracer = new MethodTracer();
            CurrentMethodTracer.Stopwatch.Start();
        }

        public void StopTrace(StackFrame stackFrame)
        {
            CurrentMethodTracer.Stopwatch.Stop();
            var methodInfo = new MethodInfo()
            {
                Name = stackFrame.GetMethod()?.Name,
                Class = stackFrame.GetMethod()?.ReflectedType?.Name,
                ElapsedMilliseconds = CurrentMethodTracer.Stopwatch.ElapsedMilliseconds,
                Methods = CurrentMethodTracer.Methods.ToList()
            };
            
            if (MethodsTraces.IsEmpty)
            {
                MethodsInfo.Push(methodInfo);
                CurrentMethodTracer = null;
            }
            else
            {
                if (MethodsTraces.TryPop(out var methodTrace))
                {
                    CurrentMethodTracer = methodTrace;
                    CurrentMethodTracer.AddMethod(methodInfo);
                }
            }
        }
    }
}