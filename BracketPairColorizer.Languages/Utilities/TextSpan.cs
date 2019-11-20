namespace BracketPairColorizer.Languages.Utilities
{
    public struct TextSpan
    {
        public int Start { get; }
        public int Length { get; }
        public int End => Start + Length;

        public TextSpan(int start, int length)
        {
            this.Start = start;
            this.Length = length;
        }

        public override bool Equals(object obj)
        {
            var other = (TextSpan)obj;

            return Start == other.Start && Length == other.Length;
        }

        public override int GetHashCode()
        {
            return Start.GetHashCode() ^ Length.GetHashCode();
        }

        public override string ToString()
        {
            return $"({Start}, {End})";
        }

        public static bool operator ==(TextSpan left, TextSpan right)
        {
            return left.Start == right.Start && left.Length == right.Length;
        }

        public static bool operator !=(TextSpan left, TextSpan right)
        {
            return !(left == right);
        }
    }
}
