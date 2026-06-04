using System.Text;
using RedRoverCodePuzzle.Models;

namespace RedRoverCodePuzzle.Services
{
    public interface ITreeService
    {
        public (bool, string) IsTreeInputValid(string input);
        public TreeNode BuildTree(string input);
        public string GenerateTreeOutput(TreeNode treeNode, StringBuilder solutionString, bool isOrdered);
    }
}