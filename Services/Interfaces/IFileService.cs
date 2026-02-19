using GitFile.DTO;
namespace GitFile.FileCreate
{
    public interface IFileService
    {
        Guid CreateFile(Guid userId, CreateFileDto dto);
    }
}