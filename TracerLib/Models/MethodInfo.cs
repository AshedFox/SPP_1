using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TracerLib.Models
{
    [Serializable]
    public record MethodInfo
    {
        [XmlAttribute]
        public string Name { get; init; }
        [XmlAttribute]
        public string Class { get; init; }
        [XmlAttribute]
        public long ElapsedMilliseconds { get; init; }
        [XmlElement(ElementName = "Method")]
        public List<MethodInfo> Methods { get; init; } = new ();
    }
}