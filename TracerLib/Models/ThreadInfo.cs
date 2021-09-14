using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TracerLib.Models
{
    [Serializable]
    public class ThreadInfo
    {
        [XmlAttribute]
        public int Id { get; init; }

        private long _totalElapsedMilliseconds;

        [XmlAttribute]
        public long TotalElapsedMilliseconds
        {
            get
            {
                _totalElapsedMilliseconds = 0;
                foreach (var methodInfo in Methods)
                {
                    _totalElapsedMilliseconds += methodInfo.ElapsedMilliseconds;
                }

                return _totalElapsedMilliseconds;
            }
            init => _totalElapsedMilliseconds = value;
        }

        [XmlElement(ElementName = "Method")]
        public List<MethodInfo> Methods { get; init; } = new();
    }
}