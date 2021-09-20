using System.Collections.Generic;
using TracerLib.Models;
using TracerLib.Tracers;
using Xunit;

namespace TracerLib.Tests
{
    public class MethodTracerTests
    {
        public static IEnumerable<object[]> TestMethodsInfo
        {
            get
            {
                yield return new object[] { new MethodInfo() { Class = "1", Name = "1" } };
                yield return new object[] { new MethodInfo() { Class = "2", Name = "2" } };
                yield return new object[] { new MethodInfo() { Class = "3", Name = "3" } };
                yield return new object[] { new MethodInfo() { Class = "4", Name = "4" } };
                yield return new object[] { new MethodInfo() { Class = "5", Name = "5" } };
            }
        }

        [Theory]
        [MemberData(nameof(TestMethodsInfo))]
        public void AddMethod_AddingNewMethodToContainer_NewMethodAdded(MethodInfo methodInfo)
        {
            var methodTracer = new MethodTracer();
            methodTracer.AddMethod(methodInfo);
            Assert.Contains(methodInfo, methodTracer.Methods);
        }
    }
}