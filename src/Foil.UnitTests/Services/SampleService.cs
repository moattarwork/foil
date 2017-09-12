using System;

namespace Foil.UnitTests.Services
{
    public class SampleService : ISampleService
    {
        public string State { get; private set; } = string.Empty;

        public string GetName()
        {
            return nameof(SampleService);
        }
        
        public void Call()
        {
            State = "Changed";
            Console.WriteLine("Hello Sample");
        }
    }
}