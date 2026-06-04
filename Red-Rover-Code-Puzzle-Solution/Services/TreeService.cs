using System.Text;
using Microsoft.AspNetCore.Components.Forms;
using RedRoverCodePuzzle.Models;

namespace RedRoverCodePuzzle.Services
{
    public class TreeService : ITreeService
    {
        public (bool, string) IsTreeInputValid(string input)
        {
            // Checks if string is null, empty, or only whitespace
            if (String.IsNullOrWhiteSpace(input))
            {
                return (false, "Input string cannot be null, empty, or whitespace!");
            }

            // Checks if input starts with ( and ends with )
            int n = input.Length;
            if(input[0] != '(' || input[n - 1] != ')')
            {
                return (false, "Input must start with ( and end with )");
            }

            // Checks if invalid substrings exist that could generate empty or invalid output.
            var invalidSubstrings = new List<string> { "()", ")(", ",,", ",)", ",(" };
            foreach(string invalidSubstring in invalidSubstrings)
            {
                if(input.Contains(invalidSubstring))
                {
                    return (false, String.Format("Input cannot contain {0} as a substring.", invalidSubstring));
                }
            }

            // Checks if paraenthesis are balanced and if there are multiple trees in the input.
            int stack = 0;

            for(int i=0; i<n; i++)
            {
                if(input[i] == '(') { 
                    stack++;
                }
                if(input[i] == ')') {
                    // This handles cases such as the parenthesis being unbalanced or a comma missing after the ending parenthesis (unless it's the end of the input).
                    while(i<n)
                    {
                        if(input[i] == ')')
                        {
                            stack--;
                            if(stack < 0) 
                            { 
                                return (false, "The number of closing parenthesis is greater than the number of opening."); 
                            }

                            if (stack == 0 && i<(n - 1))
                            {
                                return (false, "Extra characters detected outside of the closing parenthesis. Multiple trees not currently supported.");
                            }
                        }
                        else if(input[i] == ',')
                        {
                            break;
                        }
                        else
                        {
                            return (false, "Missing comma from closing parenthesis");
                        }

                        i++;
                    }
                }
            }

            // Checks to make sure that parenthesis are balanced for the tree input.
            if(stack > 0)
            {
                return (false, "There are a greater number of opening parenthesis than closing.");
            }

            return (true, "Input string is valid for building a tree.");
        }

        public TreeNode BuildTree(string input)
        {
            (bool isValid, string validationMessage) = IsTreeInputValid(input);
            if (!isValid)
            {
                throw new Exception(validationMessage);
            }

            var root = new TreeNode();
            var word = new StringBuilder();
            int n = input.Length;
            TreeNode? currNode = root;

            for(int i=0; i < n; i++) {                
                char c = input[i];

                if (currNode == null)
                {
                   throw new NullReferenceException("Current tree node should not be null");
                }

                var newNode = new TreeNode {
                    Parent = currNode,
                    Level = currNode.Level + 1,
                    Value = word.ToString().Trim()
                };

                if(c == '(' && i > 0)
                {
                    currNode.Children.Add(newNode);
                    word.Clear();
                    currNode = newNode;
                }
                else if(c == ')')
                {
                    currNode.Children.Add(newNode);
                    word.Clear();

                    while(i<n && c != ',')
                    {
                        c = input[i];
                        if(c == ')')
                        {
                            currNode = currNode?.Parent;
                        }

                        i++;
                    }                    
                }
                else if (c == ',')
                {
                    currNode.Children.Add(newNode);
                    word.Clear();
                }
                else if(c != '(')
                {
                    word.Append(c);
                }
            }

            return root;
        }

        public string GenerateTreeOutput(TreeNode treeNode, StringBuilder solutionString, bool isOrdered)
        {
            List<TreeNode> nodes =  treeNode.Children;
            
            //Orders the list alphabetically if isOrdered is true.
            if (isOrdered) { nodes = nodes.OrderBy((n) => n.Value).ToList(); }
            var queue = new Queue<TreeNode>();

            // Goes through each node's children and outputs the value based on the level.
            foreach(TreeNode node in nodes)
            {
                solutionString.Append(String.Concat(Enumerable.Repeat("  ", node.Level)));
                solutionString.Append("- ");
                solutionString.Append(node.Value);
                solutionString.Append("\n");

                if(node.Children.Any())
                {
                    GenerateTreeOutput(node, solutionString, isOrdered);
                }
            }

            return solutionString.ToString().Trim();
        }
    }
}