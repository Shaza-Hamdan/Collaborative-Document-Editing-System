using System.Security.Claims;
using GitFile.DTO;
using GitFile.Extensions;
using LibGit2Sharp;
using Microsoft.AspNetCore.Mvc;
using Registration.Persistence.Repository;

[ApiController]
[Route("api/[controller]")]
public class GitController : ControllerBase
{
    private readonly IGitService _gitService;
    private readonly AppDBContext appdbContext;
    public GitController(AppDBContext appDbContext, IGitService gitService)
    {
        appdbContext = appDbContext;
        _gitService = gitService;
    }

    [HttpPost("{fileId}/commit")]
    public IActionResult CommitChanges(Guid fileId, string CommitMessage)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // Get the authenticated user ID
            var userId = User.GetUserId();

            // Lookup the file and validate ownership
            var file = appdbContext.userFiles.FirstOrDefault(f => f.Id == fileId && f.UserId == userId);
            if (file == null)
                return NotFound("File not found or access denied.");

            // Get the repository path from DB
            var repoPath = file.RepositoryPath;

            // Get user identity info for commit signature
            var userName = User.Identity?.Name;
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userEmail))
            {
                return BadRequest("User identity or email is missing from the token.");
            }

            // Perform commit via service
            _gitService.CommitChanges(repoPath, CommitMessage, userName, userEmail);

            return Ok(new { Message = "Changes committed successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Commit failed.", Error = ex.Message });
        }
    }

    [HttpPost("branch")]
    public IActionResult CreateBranch(Guid fileId, string BranchName)
    {
        var userId = User.GetUserId();

        var file = appdbContext.userFiles.SingleOrDefault(f =>
            f.Id == fileId && f.UserId == userId);

        if (file == null)
            return NotFound("File not found or access denied.");

        _gitService.CreateBranch(file.RepositoryPath, BranchName);
        return Ok($"Branch '{BranchName}' created successfully.");
    }

    [HttpPost("switch to branch")]
    public IActionResult SwitchBranch(Guid fileId, string BranchName)
    {
        var userId = User.GetUserId();

        var file = appdbContext.userFiles.SingleOrDefault(f =>
            f.Id == fileId && f.UserId == userId);

        if (file == null)
            return NotFound("File not found or access denied.");

        _gitService.SwitchToBranch(file.RepositoryPath, BranchName);
        return Ok($"Switched to branch '{BranchName}'");
    }

    [HttpPost("merge")]
    public IActionResult MergeBranch(Guid fileId, string SourceBranch)
    {
        var userId = User.GetUserId();

        var file = appdbContext.userFiles.SingleOrDefault(f =>
            f.Id == fileId && f.UserId == userId);

        if (file == null)
            return NotFound("File not found or access denied.");

        var result = _gitService.MergeBranch(file.RepositoryPath, SourceBranch);

        return Ok(result);
    }

    [HttpGet("diff")]
    public IActionResult GetDiff(Guid fileId)
    {
        var userId = User.GetUserId();

        var file = appdbContext.userFiles.SingleOrDefault(f =>
            f.Id == fileId && f.UserId == userId);

        if (file == null)
            return NotFound("File not found or access denied.");

        var diff = _gitService.GetDiff(file.RepositoryPath);
        return Ok(diff);
    }

}





