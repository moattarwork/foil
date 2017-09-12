namespace Foil.UnitTests.Services
{
    public interface ISampleService
    {
        void Call();
        string State { get; }
        string GetName();
    }
}