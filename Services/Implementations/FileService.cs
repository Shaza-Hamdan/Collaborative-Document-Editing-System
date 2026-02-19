using GitFile.DTO;
using GitFile.FileCreate;
using Registration.Persistence.entity;
using Registration.Persistence.Repository;
public class FileService : IFileService
{
    private readonly IWebHostEnvironment _env;
    private readonly AppDBContext appdbContext;
    private readonly IGitService _git;

    public FileService(IWebHostEnvironment env, AppDBContext appDbContext, IGitService git)
    {
        _env = env;
        appdbContext = appDbContext;
        _git = git;
    }

    public Guid CreateFile(Guid userId, CreateFileDto dto)
    {
        var fileId = Guid.NewGuid();//creating file Id for each file

        var repoPath = Path.Combine( //creating private folder for each user, containing his files
            _env.ContentRootPath,
            "AppData", "Repositories", userId.ToString(), fileId.ToString());

        Directory.CreateDirectory(repoPath);

        var filePath = Path.Combine(repoPath, dto.FileName); //create the text file
        File.WriteAllText(filePath, dto.Content);

        _git.InitializeRepository(repoPath); //initialize repo in each file folder
        _git.CommitChanges(repoPath, "Initial commit", "System", "system@gitfile.app");

        var entity = new UserFile
        {
            Id = fileId,
            UserId = userId,
            FileName = dto.FileName,
            RepositoryPath = repoPath,
            CreatedAt = DateTime.UtcNow
        };

        appdbContext.userFiles.Add(entity);
        appdbContext.SaveChanges();

        return fileId;
    }
}
