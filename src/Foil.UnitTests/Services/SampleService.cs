using System;

namespace Foil.UnitTests.Services
{
    public class SampleService : ISampleService
    {
        public string State { get; private set; } = string.Empty;
        
        public virtual void Call()
        {
            State = "Changed";
            Console.WriteLine("Hello Sample");
        }
    }
}