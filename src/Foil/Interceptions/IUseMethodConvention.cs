namespace Foil.Interceptions
{
    public interface IUseMethodConvention
    {
        void UseMethodSelectionConvention<TConvention>() where TConvention : IMethodSelectionConvenstion, new();
    }
}