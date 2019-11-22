using BracketPairColorizer.Languages.CommentParsers;
using BracketPairColorizer.Languages.Utilities;
using Xunit;

namespace BracketPairColorizer.Tests.CommentParsers
{
    public class GenericCommentParserTest
    {
        [Fact]
        public void CBasedSingleLineComment()
        {
            const string input = "// vim: ts=4:sw=8";
            var parser = new GenericCommentParser();
            string result = parser.Parse(new StringCharacters(input));
            Assert.Equal("vim: ts=4:sw=8", result);
        }

        [Fact]
        public void SQLBasedSingleLineComment()
        {
            const string input = "-- vim: ts=4:sw=8";
            var parser = new GenericCommentParser();
            string result = parser.Parse(new StringCharacters(input));
            Assert.Equal("vim: ts=4:sw=8", result);
        }

        [Fact]
        public void VbBasedSingleLineComment()
        {
            const string input = "' vim: ts=4:sw=8";
            var parser = new GenericCommentParser();
            string result = parser.Parse(new StringCharacters(input));
            Assert.Equal("vim: ts=4:sw=8", result);
        }

        [Fact]
        public void PythonBasedSingleLineComment()
        {
            const string input = "# vim: ts=4:sw=8";
            var parser = new GenericCommentParser();
            string result = parser.Parse(new StringCharacters(input));
            Assert.Equal("vim: ts=4:sw=8", result);
        }

        [Fact]
        public void CBasedMultiLineComment()
        {
            const string input = "/* vim: ts=4:sw=8 */";
            var parser = new GenericCommentParser();
            string result = parser.Parse(new StringCharacters(input));
            Assert.Equal("vim: ts=4:sw=8", result);
        }

        [Fact]
        public void CBasedMultiLineWithoutTerminatorComment()
        {
            const string input = "/* vim: ts=4:sw=8     ";
            var parser = new GenericCommentParser();
            string result = parser.Parse(new StringCharacters(input));
            Assert.Equal("vim: ts=4:sw=8", result);
        }

        [Fact]
        public void FSharpMultiLineComment()
        {
            const string input = "(* vim: ts=4:sw=8 *)";
            var parser = new GenericCommentParser();
            string result = parser.Parse(new StringCharacters(input));
            Assert.Equal("vim: ts=4:sw=8", result);
        }

        [Fact]
        public void XmlComment()
        {
            const string input = "<!-- vim: ts=4:sw=8 -->";
            var parser = new GenericCommentParser();
            string result = parser.Parse(new StringCharacters(input));
            Assert.Equal("vim: ts=4:sw=8", result);
        }

        [Fact]
        public void ParsesLeadingWhiteSpace()
        {
            const string input = "\t   // vim: ts=4:sw=8";
            var parser = new GenericCommentParser();
            string result = parser.Parse(new StringCharacters(input));
            Assert.Equal("vim: ts=4:sw=8", result);
        }
    }
}
