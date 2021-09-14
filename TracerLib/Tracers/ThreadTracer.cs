using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TracerLib.Models;

namespace TracerLib.Tracers
{
    public class ThreadTracer
    {
        public ConcurrentStack<MethodTracer> MethodsTraces { get; set; } = new();
        public ConcurrentStack<MethodInfo> MethodsInfo { get; set; } = new();
        public MethodTracer CurrentMethodTracer { get; set; }

        public void StartTrace()
        {
            if (CurrentMethodTracer is not null)
            {
                MethodsTraces.Push(CurrentMethodTracer);
            }

            CurrentMethodTracer = new MethodTracer();
            CurrentMethodTracer.StartTrace();
        }

        public void StopTrace()
        {
            CurrentMethodTracer.StopTrace();
            var stackTrace = new StackTrace();
            var methodInfo = new MethodInfo()
            {
                Name = stackTrace.GetFrame(2)?.GetMethod()?.Name,
                Class = stackTrace.GetFrame(2)?.GetMethod()?.ReflectedType?.Name,
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
                MethodsTraces.TryPop(out var methodTrace);
                CurrentMethodTracer = methodTrace;
                CurrentMethodTracer!.AddMethod(methodInfo);
            }
        }
    }
}