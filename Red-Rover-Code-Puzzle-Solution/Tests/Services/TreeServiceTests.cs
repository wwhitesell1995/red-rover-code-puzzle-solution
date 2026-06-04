using NUnit.Framework;
using System.Text;
using RedRoverCodePuzzle.Models;
using RedRoverCodePuzzle.Services;

namespace RedRoverCodePuzzle.UnitTests.Services
{
    [TestFixture]
    public class TreeService_Tests
    {
        private TreeService _treeService;

        [SetUp]
        public void SetUp()
        {
            _treeService = new TreeService();
        }

        [Test]
        public void TestValidInput()
        {
            string input = "(id, name, email, type(id, name, customFields(c1, c2, c3)), externalId)";
            string expectedMessage = "Input string is valid for building a tree.";
            (bool isValid, string message) = _treeService.IsTreeInputValid(input);
            
            Assert.That(isValid, Is.True);
            Assert.That(message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestEmptyInput()
        {
            string input = "";
            string expectedMessage = "Input string cannot be null, empty, or whitespace!";
            (bool isValid, string message) = _treeService.IsTreeInputValid(input);
            
            Assert.That(isValid, Is.False);
            Assert.That(message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestMissingOpeningParenthesis()
        {
            string input = "id, name, email, type(id, name, customFields(c1, c2, c3)), externalId)";
            string expectedMessage = "Input must start with ( and end with )";
            (bool isValid, string message) = _treeService.IsTreeInputValid(input);
            
            Assert.That(isValid, Is.False);
            Assert.That(message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestMissingClosingParenthesis()
        {
            string input = "(id, name, email, type(id, name, customFields(c1, c2, c3)), externalId";
            string expectedMessage = "Input must start with ( and end with )";
            (bool isValid, string message) = _treeService.IsTreeInputValid(input);
            
            Assert.That(isValid, Is.False);
            Assert.That(message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestInvalidSubstrings()
        {
            string input = "(id, name, email, type(id, name, customFields(c1, c2, c3),), externalId)";
            string expectedMessage = "Input cannot contain ,) as a substring.";
            (bool isValid, string message) = _treeService.IsTreeInputValid(input);
            
            Assert.That(isValid, Is.False);
            Assert.That(message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestExtraClosingParenthesis()
        {
            string input = "(id, name, email, type(id, name, customFields(c1, c2, c3)), externalId))";
            string expectedMessage = "Extra characters detected outside of the closing parenthesis. Multiple trees not currently supported.";
            (bool isValid, string message) = _treeService.IsTreeInputValid(input);
            
            Assert.That(isValid, Is.False);
            Assert.That(message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestExtraOpeningParenthesis()
        {
            string input = "((id, name, email, type(id, name, customFields(c1, c2, c3)), externalId)";
            string expectedMessage = "There are a greater number of opening parenthesis than closing.";
            (bool isValid, string message) = _treeService.IsTreeInputValid(input);
            
            Assert.That(isValid, Is.False);
            Assert.That(message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestMissingCommaFromClosingParenthesis()
        {
            string input = "(id, name, email, type(id, name, customFields(c1, c2, c3)) externalId)";
            string expectedMessage = "Missing comma from closing parenthesis";
            (bool isValid, string message) = _treeService.IsTreeInputValid(input);
            
            Assert.That(isValid, Is.False);
            Assert.That(message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestMultipleTreesInInput()
        {
            string input = "(id, name, email, type(id, name, customFields(c1, c2, c3)), externalId),t1(t1)";
            string expectedMessage = "Extra characters detected outside of the closing parenthesis. Multiple trees not currently supported.";
            (bool isValid, string message) = _treeService.IsTreeInputValid(input);
            
            Assert.That(isValid, Is.False);
            Assert.That(message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TestBuildTree()
        {
            string input = "(id, name, email, type(id, name, customFields(c1, c2, c3)), externalId)";
            TreeNode tree = _treeService.BuildTree(input);
            Assert.That(tree.Children.Any((n) => n.Value == "id" && n.Level == 0 && n.Parent == tree), Is.True);
            Assert.That(tree.Children.Any((n) => n.Value == "name" && n.Level == 0 && n.Parent == tree), Is.True);
            Assert.That(tree.Children.Any((n) => n.Value == "email" && n.Level == 0 && n.Parent == tree), Is.True);
            Assert.That(tree.Children.Any((n) => n.Value == "type" && n.Level == 0 && n.Parent == tree), Is.True);

            TreeNode? typeNode = tree.Children.FirstOrDefault((n) => n.Value == "type" && n.Level == 0 && n.Parent == tree );
            Assert.That(typeNode, Is.Not.Null);
            Assert.That(typeNode.Children.Any((n) => n.Value == "id" && n.Level == 1 && n.Parent == typeNode), Is.True);
            Assert.That(typeNode.Children.Any((n) => n.Value == "name" && n.Level == 1 && n.Parent == typeNode), Is.True);
            Assert.That(typeNode.Children.Any((n) => n.Value == "customFields" && n.Level == 1 && n.Parent == typeNode), Is.True);

            TreeNode? customFieldsNode = typeNode.Children.FirstOrDefault((n) => n.Value == "customFields" && n.Level == 1 && n.Parent == typeNode );
            Assert.That(customFieldsNode, Is.Not.Null);
            Assert.That(customFieldsNode.Children.Any((n) => n.Value == "c1" && n.Level == 2 && n.Parent == customFieldsNode), Is.True);
            Assert.That(customFieldsNode.Children.Any((n) => n.Value == "c2" && n.Level == 2 && n.Parent == customFieldsNode), Is.True);
            Assert.That(customFieldsNode.Children.Any((n) => n.Value == "c3" && n.Level == 2 && n.Parent == customFieldsNode), Is.True);
        }

        [Test]
        public void TestUnorderedGenerateTreeOutput()
        {
            string input = "(id, name, email, type(id, name, customFields(c1, c2, c3)), externalId)";
            TreeNode tree = _treeService.BuildTree(input);
            string output = _treeService.GenerateTreeOutput(tree, new StringBuilder(), false);
            string expectedOuput = @"- id
- name
- email
- type
  - id
  - name
  - customFields
    - c1
    - c2
    - c3
- externalId";

            Assert.That(output, Is.EqualTo(expectedOuput));
        }

        [Test]
        public void TestOrderedGenerateTreeOutput()
        {
            string input = "(id, name, email, type(id, name, customFields(c1, c2, c3)), externalId)";
            TreeNode tree = _treeService.BuildTree(input);
            string output = _treeService.GenerateTreeOutput(tree, new StringBuilder(), true);
            string expectedOuput = @"- email
- externalId
- id
- name
- type
  - customFields
    - c1
    - c2
    - c3
  - id
  - name";

            Assert.That(output, Is.EqualTo(expectedOuput));
        }
    }
}