using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TracerLib.Models
{
    [Serializable]
    public record TraceResult : ITraceResult
    {
        [XmlElement(ElementName = "Thread")]
        public List<ThreadInfo> ThreadsInfo { get; init; } = new();
    }
}