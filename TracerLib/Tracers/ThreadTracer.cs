using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TracerLib.Models;

namespace TracerLib.Tracers
{
    public class ThreadTracer : IThreadTracer
    {
        public int Id { get; init; }
        public ConcurrentStack<IMethodTracer> MethodsTracers { get; init; } = new();
        public ConcurrentStack<MethodInfo> MethodsInfos { get; init; } = new();
        public IMethodTracer CurrentMethodTracer { get; set; }

        public void StartTrace()
        {
            if (CurrentMethodTracer is not null)
            {
                MethodsTracers.Push(CurrentMethodTracer);
            }

            CurrentMethodTracer = new MethodTracer();
            CurrentMethodTracer.Stopwatch.Start();
        }

        public void StopTrace()
        {
            if (CurrentMethodTracer is not null)
            {
                CurrentMethodTracer.Stopwatch.Stop();
                var methodInfo = new MethodInfo()
                {
                    Name = new StackFrame(2, false).GetMethod()?.Name,
                    Class = new StackFrame(2, false).GetMethod()?.ReflectedType?.Name,
                    ElapsedMilliseconds = CurrentMethodTracer.Stopwatch.ElapsedMilliseconds,
                    Methods = CurrentMethodTracer.Methods.ToList()
                };

                if (MethodsTracers.IsEmpty)
                {
                    MethodsInfos.Push(methodInfo);
                    CurrentMethodTracer = null;
                }
                else
                {
                    if (MethodsTracers.TryPop(out var methodTrace))
                    {
                        CurrentMethodTracer = methodTrace;
                        CurrentMethodTracer.Methods.Push(methodInfo);
                    }
                }
            }
        }

        public ITraceResult GetTraceResult()
        {
            return new ThreadInfo()
            {
                Id = Id,
                Methods = MethodsInfos.ToList()
            };
        }
    }
}