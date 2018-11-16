using System;

namespace Foil.Sample.Services
{
    public class SampleService : ISampleService
    {
        public virtual void Call(string sample)
        {
            Console.WriteLine($"Hello {sample}");
        }
    }
}