using System.Collections.Concurrent;
using TracerLib.Models;

namespace TracerLib.Tracers
{
    public interface IThreadTracer : ITracer
    {
        public int Id { get; init; }
        public ConcurrentStack<IMethodTracer> MethodsTracers { get; init; }
        public ConcurrentStack<MethodInfo> MethodsInfos { get; init; }
        public IMethodTracer CurrentMethodTracer { get; set; }
    }
}