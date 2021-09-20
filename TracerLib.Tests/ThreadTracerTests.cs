using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using TracerLib.Models;
using TracerLib.Tracers;
using Xunit;

namespace TracerLib.Tests
{
    public class ThreadTracerTests
    {
        [Fact]
        public void StartTrace_CurrentMethodTraceIsNotNull_AddCurrentMethodTraceToMethodsTraces()
        {
            var currentMethodTracer = new MethodTracer();
            var threadTracer = new ThreadTracer()
            {
                CurrentMethodTracer = currentMethodTracer
            };
            
            threadTracer.StartTrace();
            
            Assert.Contains(currentMethodTracer, threadTracer.MethodsTraces);
        }

        [Fact]
        public void StartTrace_CurrentMethodTraceIsNotNull_NewCurrentMethodTraceInitializes()
        {
            var currentMethodTracer = new MethodTracer();
            var threadTracer = new ThreadTracer()
            {
                CurrentMethodTracer = currentMethodTracer
            };
            
            threadTracer.StartTrace();
            
            Assert.NotEqual(currentMethodTracer, threadTracer.CurrentMethodTracer);
        }

        [Fact]
        public void StartTrace_CurrentMethodTraceIsNull_CurrentMethodTraceInitializes()
        {
            var threadTracer = new ThreadTracer()
            {
                CurrentMethodTracer = null
            };
            
            threadTracer.StartTrace();
            
            Assert.NotNull(threadTracer.CurrentMethodTracer);
        }
        
        [Fact]
        public void StartTrace_RunningStopwatch_StopwatchRun()
        {
            var threadTracer = new ThreadTracer()
            {
                CurrentMethodTracer = null
            };
            
            threadTracer.StartTrace();
            
            Assert.True(threadTracer.CurrentMethodTracer.Stopwatch.IsRunning);
        }

        [Fact]
        public void StopTrace_StoppingStopwatch_StopwatchStopped()
        {
            var threadTracer = new ThreadTracer()
            {
                CurrentMethodTracer = null
            };
            threadTracer.StartTrace();

            var currentMethodTracer = threadTracer.CurrentMethodTracer;
            
            threadTracer.StopTrace(new StackTrace().GetFrame(1));
            
            Assert.False(currentMethodTracer.Stopwatch.IsRunning);
        }

        [Fact]
        public void StopTrace_MethodsTracesIsEmpty_AddedMethodInfoToMethodsInfo()
        {
            var threadTracer = new ThreadTracer()
            {
                CurrentMethodTracer = null
            };
            threadTracer.StartTrace();
            
            threadTracer.StopTrace(new StackTrace().GetFrame(1));
            
            Assert.NotEmpty(threadTracer.MethodsInfo);
        }
        
        [Fact]
        public void StopTrace_MethodsTracesIsNotEmpty_AddedMethodInfoToCurrentMethodTracer()
        {
            var threadTracer = new ThreadTracer()
            {
                CurrentMethodTracer = null,
                MethodsTraces = new ConcurrentStack<MethodTracer>(new []
                {
                    new MethodTracer()
                })
            };

            threadTracer.StartTrace();

            threadTracer.StopTrace(new StackTrace().GetFrame(1));
            
            Assert.NotEmpty(threadTracer.CurrentMethodTracer.Methods);
        }
    }
}