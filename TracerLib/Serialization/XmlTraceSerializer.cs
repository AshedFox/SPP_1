using System.IO;
using System.Xml.Serialization;
using TracerLib.Models;

namespace TracerLib.Serialization
{
    public class XmlTraceSerializer : ITraceSerializer
    {
        public string Serialize(ITraceResult traceResult)
        {
            using var memoryStream = new MemoryStream();

            new XmlSerializer(traceResult.GetType()).Serialize(memoryStream, traceResult);
            memoryStream.Position = 0;
            return new StreamReader(memoryStream).ReadToEnd();
        }
    }
}