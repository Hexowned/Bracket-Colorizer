namespace BracketPairColorizer.Core.Utilities
{
    public interface ITokenizer
    {
        bool AtEnd { get; }
        string Token { get; }

        bool Next();
    }
}
