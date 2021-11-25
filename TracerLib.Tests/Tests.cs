using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Moq;
using TracerLib.Models;
using TracerLib.Serialization;
using TracerLib.Tracers;
using Xunit;

namespace TracerLib.Tests
{
    public class Tests
    {
        public static IEnumerable<object[]> TestMethodInfos
        {
            get
            {
                yield return new object[] { new MethodInfo() { Class = "1", Name = "1", ElapsedMilliseconds = 1} };
                yield return new object[] { new MethodInfo() { Class = "2", Name = "2", ElapsedMilliseconds = 2} };
                yield return new object[] { new MethodInfo() { Class = "3", Name = "3", ElapsedMilliseconds = 3} };
                yield return new object[] { new MethodInfo() { Class = "4", Name = "4", ElapsedMilliseconds = 4} };
                yield return new object[] { new MethodInfo() { Class = "5", Name = "5", ElapsedMilliseconds = 5} };
            }
        }
        
        public static IEnumerable<object[]> TestThreadsInfos
        {
            get
            {
                yield return new object[] { new ThreadInfo() { Id = 1 } };
                yield return new object[] { new ThreadInfo() { Id = 2 } };
                yield return new object[] { new ThreadInfo() { Id = 3 } };
                yield return new object[] { new ThreadInfo() { Id = 4 } };
                yield return new object[] { new ThreadInfo() { Id = 5 } };
            }
        }
        
        public static IEnumerable<object[]> TestThreadsInfosWithMethods
        {
            get
            {
                yield return new object[]
                {
                    new ThreadInfo()
                    {
                        Id = 1,
                        Methods = new List<MethodInfo>()
                        {
                            new() { Name = "1", Class = "1", ElapsedMilliseconds = 1 },
                            new() { Name = "2", Class = "2", ElapsedMilliseconds = 2 },
                            new() { Name = "3", Class = "3", ElapsedMilliseconds = 3 }
                        }
                    }
                };
                yield return new object[]
                {
                    new ThreadInfo()
                    {
                        Id = 3,
                        Methods = new List<MethodInfo>() { new() { Name = "1", Class = "1", ElapsedMilliseconds = 1 } }
                    }
                };
                yield return new object[]
                {
                    new ThreadInfo()
                    {
                        Id = 5,
                        Methods = new List<MethodInfo>()
                        {
                            new() { Name = "1", Class = "1", ElapsedMilliseconds = 9999 },
                            new() { Name = "9", Class = "1", ElapsedMilliseconds = 1 },
                        }
                    }
                };
            }
        }
        
        public static IEnumerable<object[]> TestTraceResults
        {
            get
            {
                yield return new object[] { new TraceResult() { ThreadsInfo = new List<ThreadInfo>() { new() { Id = 0 } }} };
                yield return new object[] { new TraceResult() { ThreadsInfo = new List<ThreadInfo>() { new() { Id = 999 } }} };
                yield return new object[] { new TraceResult() { ThreadsInfo = new List<ThreadInfo>() { new() { Id = int.MaxValue } }} };
            }
        }

        [Theory]
        [MemberData(nameof(TestMethodInfos))]
        public void XmlTraceSerializerSerialize_SerializingMethodInfo_SerializedCorrect(MethodInfo testMethodInfo)
        {
            var mock = new Mock<ITracer>();
            mock.Setup(tracer => tracer.GetTraceResult()).Returns(testMethodInfo);
            var xmlSerializer = new XmlTraceSerializer();

            var actual = xmlSerializer.Serialize(mock.Object.GetTraceResult());
            
            Assert.NotNull(actual);
            Assert.IsType<string>(actual);
            actual.Should().ContainAll($"Name=\"{testMethodInfo.Name}\"", $"Class=\"{testMethodInfo.Class}\"",
                $"ElapsedMilliseconds=\"{testMethodInfo.ElapsedMilliseconds}\"");
            actual.Should().StartWith("<");
            actual.Should().EndWith(">");
        }
        
        [Theory]
        [MemberData(nameof(TestThreadsInfos))]
        public void XmlTraceSerializerSerialize_SerializingThreadInfo_SerializedIdAndTotalElapsedMillisecondsCorrect(ThreadInfo testThreadInfo)
        {
            var mock = new Mock<ITracer>();
            mock.Setup(tracer => tracer.GetTraceResult()).Returns(testThreadInfo);
            var xmlSerializer = new XmlTraceSerializer();

            var actual = xmlSerializer.Serialize(mock.Object.GetTraceResult());
            
            Assert.NotNull(actual);
            Assert.IsType<string>(actual);
            actual.Should().ContainAll($"Id=\"{testThreadInfo.Id}\"",
                $"TotalElapsedMilliseconds=\"{testThreadInfo.TotalElapsedMilliseconds}\"");
            actual.Should().StartWith("<");
            actual.Should().EndWith(">");
        }
        
        [Theory]
        [MemberData(nameof(TestThreadsInfosWithMethods))]
        public void XmlTraceSerializerSerialize_SerializingThreadInfo_SerializedMethodsCorrect(ThreadInfo testThreadInfo)
        {
            var mock = new Mock<ITracer>();
            mock.Setup(tracer => tracer.GetTraceResult()).Returns(testThreadInfo);
            var xmlSerializer = new XmlTraceSerializer();

            var actual = xmlSerializer.Serialize(mock.Object.GetTraceResult());
            
            Assert.NotNull(actual);
            Assert.IsType<string>(actual);
            
            actual.Should().ContainAll(testThreadInfo.Methods.Select(info => $"Name=\"{info.Name}\""));
            actual.Should().ContainAll(testThreadInfo.Methods.Select(info => $"Class=\"{info.Class}\""));
            actual.Should().ContainAll(testThreadInfo.Methods.Select(info => $"ElapsedMilliseconds=\"{info.ElapsedMilliseconds}\""));
        }
        
        [Theory]
        [MemberData(nameof(TestTraceResults))]
        public void XmlTraceSerializerSerialize_SerializingTraceResult_SerializedCorrect(TraceResult testTraceInfo)
        {
            var mock = new Mock<ITracer>();
            mock.Setup(tracer => tracer.GetTraceResult()).Returns(testTraceInfo);
            var xmlSerializer = new XmlTraceSerializer();

            var actual = xmlSerializer.Serialize(mock.Object.GetTraceResult());
            
            Assert.NotNull(actual);
            Assert.IsType<string>(actual);
            actual.Should().ContainAll(testTraceInfo.ThreadsInfo.Select(info => $"Id=\"{info.Id}\""));
            actual.Should().ContainAll(testTraceInfo.ThreadsInfo.Select(info => $"TotalElapsedMilliseconds=\"{info.TotalElapsedMilliseconds}\""));
            actual.Should().StartWith("<");
            actual.Should().EndWith(">");
        }

        [Theory]
        [MemberData(nameof(TestMethodInfos))]
        public void ThreadTracerGetTraceResult_MethodsNotEmpty_ReturnedCorrect(MethodInfo testMethodInfo)
        {
            var mock = new Mock<ITracer>();
            mock.Setup(tracer => tracer.GetTraceResult()).Returns(testMethodInfo);
            var threadTracer = new ThreadTracer() { Id = 1 };
            var expected = new ThreadInfo()
            {
                Id = 1,
                Methods = new List<MethodInfo> { testMethodInfo }
            };
            
            threadTracer.MethodsInfos.Push(mock.Object.GetTraceResult() as MethodInfo);
            var actual = threadTracer.GetTraceResult();

            Assert.NotNull(actual);
            Assert.IsType<ThreadInfo>(actual);
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void TracerStartTrace_ThreadsTracersIsNotEmpty_StartThreadTraceInside()
        {
            var mock = new Mock<IThreadTracer>();
            var tracer = new Tracer()
            {
                ThreadsTracers = { [Thread.CurrentThread.ManagedThreadId] = mock.Object }
            };
            
            tracer.StartTrace();

            mock.Verify(threadTracer => threadTracer.StartTrace(), Times.Once);
        }
        
        [Fact]
        public void TracerStopTrace_ThreadsTracersIsNotEmpty_StartThreadTraceInside()
        {
            var mock = new Mock<IThreadTracer>();
            var tracer = new Tracer()
            {
                ThreadsTracers = { [Thread.CurrentThread.ManagedThreadId] = mock.Object }
            };
            
            tracer.StopTrace();

            mock.Verify(threadTracer => threadTracer.StopTrace(), Times.Once);
        }

        [Fact]
        public void ThreadTracerStartTrace_CurrentMethodTraceIsNotNull_AddCurrentMethodTraceToMethodsTraces()
        {
            var mock = new Mock<IMethodTracer>();
            var threadTracer = new ThreadTracer()
            {
                CurrentMethodTracer = mock.Object
            };
            
            threadTracer.StartTrace();
            
            Assert.Contains(mock.Object, threadTracer.MethodsTracers);
        }
        
        [Fact]
        public void ThreadTracerStopTrace_MethodsTracesIsNotEmptyAndCurrentMethodTracerNotNull_StopTraceCorrectly()
        {
            var mock = new Mock<IMethodTracer>();
            mock.Setup(tracer => tracer.Methods).Returns(() => new ConcurrentStack<MethodInfo>());
            mock.Setup(tracer => tracer.Stopwatch).Returns(() => new Stopwatch());
            var threadTracer = new ThreadTracer()
            {
                CurrentMethodTracer = mock.Object,
                MethodsTracers = new ConcurrentStack<IMethodTracer>(new[] { mock.Object })
            };
            
            threadTracer.StopTrace();
            
            Assert.NotNull(threadTracer.CurrentMethodTracer);
            Assert.Equal(mock.Object, threadTracer.CurrentMethodTracer);
        }
        
        [Fact]
        public void ThreadTracerStopTrace_MethodsTracesIsEmptyAndCurrentMethodTracerNotNull_StopTraceCorrectly()
        {
            var mock = new Mock<IMethodTracer>();
            mock.Setup(tracer => tracer.Methods).Returns(() => new ConcurrentStack<MethodInfo>());
            mock.Setup(tracer => tracer.Stopwatch).Returns(() => new Stopwatch());
            var threadTracer = new ThreadTracer()
            {
                CurrentMethodTracer = mock.Object,
            };
            
            threadTracer.StopTrace();
            
            Assert.Null(threadTracer.CurrentMethodTracer);
            Assert.NotEmpty(threadTracer.MethodsInfos);
        }
    }
}