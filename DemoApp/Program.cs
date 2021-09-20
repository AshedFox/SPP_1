using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TracerLib.Serialization;
using TracerLib.Tracers;

namespace DemoApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var tracer = new Tracer();
            DemoClass1 demoClass1 = new(tracer);
            demoClass1.DemoMethod1();
            demoClass1.DemoMethod1();

            var xml = new XmlTraceSerializer().Serialize(tracer.GetTraceResult());
            var json = new JsonTraceSerializer().Serialize(tracer.GetTraceResult());
            Console.WriteLine(json);
            Console.WriteLine(xml);
            File.WriteAllText("json.json", json);
            File.WriteAllText("xml.xml", xml);
        }
    }

    public class DemoClass1
    {
        private readonly ITracer _tracer;
        private readonly DemoClass2 _demoClass2;
        public DemoClass1(ITracer tracer)
        {
            _tracer = tracer;
            _demoClass2 = new DemoClass2(_tracer);
        }

        public void DemoMethod1()
        {
            _tracer.StartTrace();

            var events = new List<WaitHandle>();

            for (int i = 0; i < 2; i++)
            {   
                var resetEvent = new ManualResetEvent(false);
                ThreadPool.QueueUserWorkItem(
                    _ =>
                    {
                        _demoClass2.DemoMethod2();
                        resetEvent.Set();
                    });
                events.Add(resetEvent);
            }

            _demoClass2.DemoMethod2(); _demoClass2.DemoMethod2();
            
            
            WaitHandle.WaitAll(events.ToArray());
            _tracer.StopTrace();
        }
    }

    public class DemoClass2
    {
        private readonly ITracer _tracer;

        public DemoClass2(ITracer tracer)
        {
            _tracer = tracer;
        }
        
        public void DemoMethod2()
        {
            _tracer.StartTrace();

            for (int i = 0; i < 2; i++)
            {
                DemoMethod3();
            }
            
            _tracer.StopTrace();
        }
        
        public void DemoMethod3()
        {
            _tracer.StartTrace();

            for (int i = 0; i < 3; i++)
            {
                DemoMethod4();
            }
            
            _tracer.StopTrace();
        }
        
        public void DemoMethod4()
        {
            _tracer.StartTrace();

            for (int i = 0; i < 999999; i++)
            {
                int b = 1 + i;
            }
            
            _tracer.StopTrace();
        }
    }
}