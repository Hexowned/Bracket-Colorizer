using BracketPairColorizer.Languages;
using BracketPairColorizer.Rainbow;
using Xunit;

namespace BracketPairColorizer.Tests.Rainbow
{
    public class PerBraceStackerTests
    {
        private string braceList = "{}()[]";

        [Fact]
        public void WhenEmptyCountIsZero()
        {
            var stack = new PerBraceStacker(braceList);
            Assert.Equal(0, stack.Count('{'));
        }

        [Fact]
        public void WhenAddingOneOfEachCountIs1()
        {
            var stack = new PerBraceStacker(braceList);
            stack.Push(new CharPosition('{', 0));
            stack.Push(new CharPosition('(', 0));
            Assert.Equal(1, stack.Count('{'));
            Assert.Equal(1, stack.Count('('));
        }

        [Fact]
        public void WhenPushingMultipleBracesDepthIsIncreased()
        {
            var stack = new PerBraceStacker(braceList);
            stack.Push(new CharPosition('{', 0));
            stack.Push(new CharPosition('{', 0));
            Assert.Equal(1, stack.Pop('{').Depth);
            Assert.Equal(0, stack.Pop('{').Depth);
        }

        [Fact]
        public void WhenRemovingOneTheCountOfTheOtherIsNotReduced()
        {
            var stack = new PerBraceStacker(braceList);
            stack.Push(new CharPosition('{', 0));
            stack.Push(new CharPosition('(', 0));
            stack.Pop('{');
            Assert.Equal(0, stack.Count('{'));
            Assert.Equal(1, stack.Count('('));
        }
    }
}
