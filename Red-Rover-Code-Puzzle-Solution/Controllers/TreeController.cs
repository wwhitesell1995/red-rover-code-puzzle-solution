using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using RedRoverCodePuzzle.Models;
using RedRoverCodePuzzle.Services;


namespace RedRoverCodePuzzle.Controllers;

[Route("[controller]")]
public class TreeController : Controller
{
    private ITreeService _treeService;
    public TreeController(ITreeService treeService)
    {
        _treeService = treeService;
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetTreeOutput(string input, bool isOrdered = false)
    {
        try
        {
            TreeNode node = _treeService.BuildTree(input);
            string output = _treeService.GenerateTreeOutput(node, new StringBuilder(), isOrdered);
            return Ok(output);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        } 
    }
}
