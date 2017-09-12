using System.Collections.Generic;

namespace Foil.UnitTests.Conventions
{
    public class SampleInterceptee
    {
        public IEnumerable<string> GetList()
        {
            return new List<string>();
        }        
        
        public IEnumerable<string> LoadList()
        {
            return new List<string>();
        }            
        
        public void Insert()
        {
            
        }
    }
}