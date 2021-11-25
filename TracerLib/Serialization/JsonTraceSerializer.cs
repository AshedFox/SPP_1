using System.Runtime.Serialization;
using System.Text.Json;
using Newtonsoft.Json;
using TracerLib.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TracerLib.Serialization
{
    public class JsonTraceSerializer : ITraceSerializer
    {
        public string Serialize(ITraceResult traceResult)
        { 
            return JsonConvert.SerializeObject(traceResult, Formatting.Indented);
        }
    }
}