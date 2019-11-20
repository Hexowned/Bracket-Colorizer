using System.IO;

namespace BracketPairColorizer.Tests
{
    public class VsSolutionTest
    {
        [Fact]
        public void MakeRelativePath_NestedPath()
        {
            string solutionPath = "";
            string filePath = Path.Combine(solutionPath, "");
            string relative = VsSolution.MakeRelativePath(solutionPath, filePath);
            Assert.Equal("", relative);
        }

        [Fact]
        public void MakeRelativePath_ParentPath()
        {
            string solutionPath = "";
            string filePath = Path.Combine(solutionPath, "");
            string relative = VsSolution.MakeRelativePath(solutionPath, Path.GetFullPath(filePath));
            Assert.Equal("", relative);
        }
    }
}
