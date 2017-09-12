namespace Foil.Interceptions
{
    public interface IUseMethodConvention
    {
        void UseMethodConvention<TConvention>() where TConvention : IMethodConvention, new();
    }
}