using System.Runtime.Serialization;
using System.Text.Json;
using Newtonsoft.Json;
using TracerLib.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TracerLib.Serialization
{
    public class JsonTraceSerializer : ITraceSerializer
    {
        public string Serialize(TraceResult traceResult)
        { 
            return JsonConvert.SerializeObject(traceResult, Formatting.Indented);
        }

        public TraceResult Deserialize(string traceResult)
        {
            return JsonConvert.DeserializeObject<TraceResult>(traceResult);
        }
    }
}