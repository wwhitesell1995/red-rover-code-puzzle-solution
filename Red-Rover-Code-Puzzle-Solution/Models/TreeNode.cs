namespace RedRoverCodePuzzle.Models
{
    public class TreeNode()
    {
        public TreeNode? Parent {get; set;}

        public int Level {get; set;} = -1;

        public string? Value {get; set;}

        public List<TreeNode> Children = new List<TreeNode>();
    }
}