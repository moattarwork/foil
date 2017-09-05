using System;

namespace Foil.Sample.Services
{
    public class SampleService : ISampleService
    {
        public virtual void Call()
        {
            Console.WriteLine("Hello Sample");
        }
    }
}