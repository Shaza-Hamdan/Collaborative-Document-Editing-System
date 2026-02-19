using GitFile.DTO;
using GitFile.Extensions;
using Microsoft.AspNetCore.Mvc;
using GitFile.FileCreate;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/files")]
[Authorize]
public class FileController : ControllerBase
{
    private readonly IFileService _files;

    public FileController(IFileService files)
    {
        _files = files;
    }

    [HttpPost("create New File")]
    public IActionResult Create([FromBody] CreateFileDto dto)
    {
        var userId = User.GetUserId();
        var id = _files.CreateFile(userId, dto);
        return Ok(new { FileId = id, Message = "File created successfully" });
    }
}
