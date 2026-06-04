using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using RedRoverCodePuzzle.Controllers;
using RedRoverCodePuzzle.Services;

namespace RedRoverCodePuzzle.UnitTests.Controllers
{
    [TestFixture]
    public class TreeController_Tests : Controller
    {
        private TreeController _treeController;

        [SetUp]
        public void SetUp()
        {
            _treeController = new TreeController(new TreeService());
        }
        
        [Test]
        public async Task TestUnorderedGenerateTreeControllerOutput()
        {
            string input = "(id, name, email, type(id, name, customFields(c1, c2, c3)), externalId)";
            ActionResult<string> response = await _treeController.GetTreeOutput(input);
            string expectedResult = @"- id
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

            Assert.IsNotNull(response?.Result);
            Assert.IsInstanceOf<OkObjectResult>(response.Result);
            
            OkObjectResult result = (OkObjectResult)response.Result;
            Assert.AreEqual(result.Value, expectedResult);
         }

                 
        [Test]
        public async Task TestOrderedGenerateTreeOutput()
        {
            string input = "(id, name, email, type(id, name, customFields(c1, c2, c3)), externalId)";
            ActionResult<string> response = await _treeController.GetTreeOutput(input, true);
            string  expectedResult = @"- email
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
            Assert.IsNotNull(response?.Result);
            Assert.IsInstanceOf<OkObjectResult>(response.Result);
            
            OkObjectResult result = (OkObjectResult)response.Result;
            Assert.AreEqual(result.Value, expectedResult);
         }

        [Test]
        public async Task TestInvalidInput()
        {
            string input = "id, name, email, type(id, name, customFields(c1, c2, c3)), externalId)";
            ActionResult<string> response = await _treeController.GetTreeOutput(input);
            string expectedResult = "Input must start with ( and end with )";

            Assert.IsNotNull(response?.Result);
            Assert.IsInstanceOf<BadRequestObjectResult>(response.Result);

            BadRequestObjectResult result = (BadRequestObjectResult)response.Result;
            Assert.AreEqual(result.Value, expectedResult);
        }

        [TearDown]
        public void TearDownController()
        {
            _treeController.Dispose();
        }
    }
}