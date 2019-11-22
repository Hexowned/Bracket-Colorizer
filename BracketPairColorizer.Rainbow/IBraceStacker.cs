using BracketPairColorizer.Languages;

namespace BracketPairColorizer.Rainbow
{
    public interface IBraceStacker
    {
        int Count(char brace);

        BracePosition Push(CharPosition brace);

        BracePosition Pop(char brace);

        BracePosition Peek(char brace);
    }
}
