using System.Threading;
using TracerLib.Tracers;
using Xunit;

namespace TracerLib.Tests
{
    public class TracerTests
    {
        private class TestClass
        {
            private readonly Tracer _tracer;

            public TestClass(Tracer tracer)
            {
                _tracer = tracer;
            }
            
            public void TestMethod1()
            {
                _tracer.StartTrace();
                Thread.Sleep(10);
                _tracer.StopTrace();
            }

            public void TestMethod2()
            {
                _tracer.StartTrace();
                Thread.Sleep(20);
                TestMethod1();
                _tracer.StopTrace();
            }
        }

        [Fact]
        public void GetTraceResult_ThreadsTraceIsEmpty_ReturnedTraceResultWithEmptyThreadInfos()
        {
            var tracer = new Tracer();

            var result = tracer.GetTraceResult();
            Assert.Empty(result.ThreadsInfo);
        }
        
        [Fact]
        public void GetTraceResult_OneMethodInOneThread_ReturnedTraceResultWithOneMethodInOneThread()
        {
            var tracer = new Tracer();
            var testClass = new TestClass(tracer);
            testClass.TestMethod1();

            var result = tracer.GetTraceResult();
            Assert.True(result.ThreadsInfo.Count == 1);
            Assert.True(result.ThreadsInfo[0].Methods.Count == 1);
        }
        
        [Fact]
        public void GetTraceResult_OneMethodInOneThread_ReturnedTraceResultWithCorrectElapsedMilliseconds()
        {
            var tracer = new Tracer();
            var testClass = new TestClass(tracer);
            testClass.TestMethod1();

            var result = tracer.GetTraceResult();
            Assert.True(result.ThreadsInfo[0].Methods[0].ElapsedMilliseconds >= 10);
            Assert.True(result.ThreadsInfo[0].TotalElapsedMilliseconds >=
                        result.ThreadsInfo[0].Methods[0].ElapsedMilliseconds);
        }
        
        [Fact]
        public void GetTraceResult_OneMethodInOneThread_ReturnedTraceResultWithCorrectNames()
        {
            var tracer = new Tracer();
            var testClass = new TestClass(tracer);
            testClass.TestMethod1();

            var result = tracer.GetTraceResult();
            Assert.True(result.ThreadsInfo[0].Methods[0].Name == nameof(testClass.TestMethod1));
            Assert.True(result.ThreadsInfo[0].Methods[0].Class == nameof(TestClass));
        }
        
        [Fact]
        public void GetTraceResult_NestedMethodsInOneThread_ReturnedTraceResultWithCorrectNesting()
        {
            var tracer = new Tracer();
            var testClass = new TestClass(tracer);
            testClass.TestMethod2();

            var result = tracer.GetTraceResult();
            Assert.True(result.ThreadsInfo.Count == 1);
            Assert.True(result.ThreadsInfo[0].Methods.Count == 1);
            Assert.True(result.ThreadsInfo[0].Methods[0].Methods.Count == 1);
        }
        
        [Fact]
        public void GetTraceResult_NestedMethodsInOneThread_ReturnedTraceResultWithCorrectNames()
        {
            var tracer = new Tracer();
            var testClass = new TestClass(tracer);
            testClass.TestMethod2();

            var result = tracer.GetTraceResult();
            Assert.True(result.ThreadsInfo[0].Methods[0].Name == nameof(testClass.TestMethod2));
            Assert.True(result.ThreadsInfo[0].Methods[0].Class == nameof(testClass));
            Assert.True(result.ThreadsInfo[0].Methods[0].Methods[0].Name == nameof(testClass.TestMethod1));
            Assert.True(result.ThreadsInfo[0].Methods[0].Methods[0].Class == nameof(testClass));
        }
        
        [Fact]
        public void GetTraceResult_NestedMethodsInOneThread_ReturnedTraceResultWithCorrectElapsedMilliseconds()
        {
            var tracer = new Tracer();
            var testClass = new TestClass(tracer);
            testClass.TestMethod2();

            var result = tracer.GetTraceResult();
            Assert.True(result.ThreadsInfo[0].Methods[0].ElapsedMilliseconds >= 20);
            Assert.True(result.ThreadsInfo[0].TotalElapsedMilliseconds >=
                        result.ThreadsInfo[0].Methods[0].ElapsedMilliseconds);
            Assert.True(result.ThreadsInfo[0].Methods[0].Methods[0].ElapsedMilliseconds >= 10);
            Assert.True(result.ThreadsInfo[0].Methods[0].ElapsedMilliseconds >=
                        result.ThreadsInfo[0].Methods[0].Methods[0].ElapsedMilliseconds);
        }
        
        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void GetTraceResult_ManyThreads_ReturnedTraceResultWithCorrectThreadsCount(int countOfThreads)
        {
            var tracer = new Tracer();
            var testClass = new TestClass(tracer);
            for (int i = 0; i < countOfThreads; i++)
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    testClass.TestMethod1();
                });
            }

            var result = tracer.GetTraceResult();
            Assert.True(result.ThreadsInfo.Count == countOfThreads);
        }
    }
}