using GitFile.DTO;
using LibGit2Sharp;

public class GitService : IGitService
{
    public void InitializeRepository(string repositoryPath)
    {
        if (!Repository.IsValid(repositoryPath))
        {
            Repository.Init(repositoryPath);
            Console.WriteLine($"Repository initialized at {repositoryPath}");
        }
        else
        {
            Console.WriteLine($"Repository already exists at {repositoryPath}");
        }
    }
    public void CommitChanges(string repositoryPath, string commitMessage, string userName, string userEmail)
    {
        //open the repository
        using var repo = new Repository(repositoryPath);

        // Stage all changes (add/edit/delete)
        Commands.Stage(repo, "*");

        // Prepare signature
        var signature = new Signature(userName, userEmail, DateTimeOffset.UtcNow);

        // Commit
        repo.Commit(commitMessage, signature, signature);
    }

    public void CreateBranch(string repoPath, string branchName)
    {
        using var repo = new Repository(repoPath);

        if (repo.Branches[branchName] != null)
            throw new Exception("Branch already exists.");

        repo.CreateBranch(branchName);
    }
    public void SwitchToBranch(string repoPath, string branchName)
    {
        using var repo = new Repository(repoPath);

        var branch = repo.Branches[branchName];
        if (branch == null)
            throw new Exception("Branch does not exist.");

        Commands.Checkout(repo, branch);
    }

    public string MergeBranch(string repoPath, string sourceBranch)
    {
        using var repo = new Repository(repoPath);

        var branch = repo.Branches[sourceBranch];
        if (branch == null)
            throw new Exception("Source branch does not exist.");

        var signature = new Signature("System", "system@gitfile.app", DateTimeOffset.Now);
        var result = repo.Merge(branch, signature);

        if (result.Status == MergeStatus.Conflicts)
            return "Merge completed with conflicts.";

        return "Merge completed successfully.";
    }

    public IReadOnlyList<DiffEntry> GetDiff(string repositoryPath)
    {
        using var repo = new Repository(repositoryPath);

        var changes = repo.Diff.Compare<TreeChanges>(
            repo.Head.Tip.Tree,
            DiffTargets.WorkingDirectory
        );

        return changes
            .Select(c => new DiffEntry(
                c.Path,
                c.Status.ToString()
            ))
            .ToList();
    }
}
