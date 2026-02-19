using GitFile.DTO;

public interface IGitService
{
    void InitializeRepository(string repositoryPath);
    void CommitChanges(string repositoryPath, string commitMessage, string userName, string userEmail);
    void CreateBranch(string repositoryPath, string branchName);
    void SwitchToBranch(string repositoryPath, string branchName);
    string MergeBranch(string repositoryPath, string sourceBranchName);
    IReadOnlyList<DiffEntry> GetDiff(string repositoryPath);
}
