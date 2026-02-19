namespace GitFile.DTO
{
  public record CommitRequest
  (
       string CommitMessage
  );

  public record BranchRequest
  (
       string RepositoryPath,
       string BranchName
  );

  public record MergeRequest(
    string RepositoryPath,
    string SourceBranchName

  );
  public record DiffRequest(
    string RepositoryPath,
    string FilePath

  );

  public record DiffEntry
  (
       string Path,
       string Status
  );
  public record SwitchBranchRequest(
      string RepositoryPath,
      string BranchName
  );
}
