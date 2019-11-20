namespace BracketPairColorizer.Languages
{
    public struct CharPosition
    {
        private readonly char ch;
        private readonly int state;
        private readonly int position;
        public static CharPosition Empty = new CharPosition('\0', 0);

        public char Char => this.ch;
        public int State => this.state;
        public int Position => this.position;

        public CharPosition(char ch, int pos)
            : this(ch, pos, 0)
        {
        }

        public CharPosition(char ch, int pos, int state)
        {
            this.ch = ch;
            this.state = state;
            this.position = pos;
        }

        public static implicit operator char(CharPosition cp)
        {
            return cp.Char;
        }

        public override string ToString()
        {
            return string.Format("'{0}' ({1})", Char, Position);
        }
    }
}
