using Octokit;
using Platform.Exceptions;
using Storage.Local;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Storage.Remote.GitHub
{
    /// <summary>
    /// <para>
    /// Represents the git hub storage.
    /// </para>
    /// <para></para>
    /// </summary>
    public class GitHubStorage
    {
        /// <summary>
        /// <para>
        /// The client.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly GitHubClient Client;

        /// <summary>
        /// <para>
        /// The owner.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly string Owner;

        /// <summary>
        /// <para>
        /// Gets the minimum interaction interval value.
        /// </para>
        /// <para></para>
        /// </summary>
        public TimeSpan MinimumInteractionInterval { get; }

        /// <summary>
        /// <para>
        /// The from days.
        /// </para>
        /// <para></para>
        /// </summary>
        private DateTimeOffset lastIssue = DateTimeOffset.Now.Subtract(TimeSpan.FromDays(14));

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="GitHubStorage"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="owner">
        /// <para>A owner.</para>
        /// <para></para>
        /// </param>
        /// <param name="token">
        /// <para>A token.</para>
        /// <para></para>
        /// </param>
        /// <param name="name">
        /// <para>A name.</para>
        /// <para></para>
        /// </param>
        public GitHubStorage(string owner, string token, string name)
        {
            Owner = owner;
            Client = new GitHubClient(new ProductHeaderValue(name))
            {
                Credentials = new Credentials(token)
            };
            MinimumInteractionInterval = new(0, 0, 0, 0, 1200);
        }

        /// <summary>
        /// <para>
        /// Gets the issues.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <returns>
        /// <para>A read only list of issue</para>
        /// <para></para>
        /// </returns>
        public IReadOnlyList<Issue> GetIssues()
        {
            IssueRequest request = new()
            {
                Filter = IssueFilter.All,
                State = ItemStateFilter.Open,
                Since = lastIssue
            };
            var issues = Client.Issue.GetAllForCurrent(request).Result;
            if (issues.Count != 0)
            {
                lastIssue = issues.Max(x => x.CreatedAt);
                return issues;
            }
            return new List<Issue>();
        }

        /// <summary>
        /// <para>
        /// Gets the commits using the specified owner.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="owner">
        /// <para>The owner.</para>
        /// <para></para>
        /// </param>
        /// <param name="reposiroty">
        /// <para>The reposiroty.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>A read only list of git hub commit</para>
        /// <para></para>
        /// </returns>
        public IReadOnlyList<GitHubCommit> GetCommits(string owner, string reposiroty)
        {
            var date = DateTime.Today.AddMonths(-1);
            return Client.Repository.Commit.GetAll(owner, reposiroty, new CommitRequest() { Since = date }).Result;
        }

        /// <summary>
        /// <para>
        /// Gets the pull requests using the specified owner.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="owner">
        /// <para>The owner.</para>
        /// <para></para>
        /// </param>
        /// <param name="reposiroty">
        /// <para>The reposiroty.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>A read only list of pull request</para>
        /// <para></para>
        /// </returns>
        public IReadOnlyList<PullRequest> GetPullRequests(string owner, string reposiroty) => Client.PullRequest.GetAllForRepository(owner, reposiroty).Result;

        /// <summary>
        /// <para>
        /// Gets the issues using the specified owner.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="owner">
        /// <para>The owner.</para>
        /// <para></para>
        /// </param>
        /// <param name="reposiroty">
        /// <para>The reposiroty.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>A read only list of issue</para>
        /// <para></para>
        /// </returns>
        public IReadOnlyList<Issue> GetIssues(string owner, string reposiroty)
        {
            var date = DateTime.Today.AddMonths(-1);
            return Client.Issue.GetAllForRepository(owner, reposiroty, new RepositoryIssueRequest() { Since = date }).Result;
        }

        /// <summary>
        /// <para>
        /// Creates the or update file using the specified repository.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="repository">
        /// <para>The repository.</para>
        /// <para></para>
        /// </param>
        /// <param name="branch">
        /// <para>The branch.</para>
        /// <para></para>
        /// </param>
        /// <param name="file">
        /// <para>The file.</para>
        /// <para></para>
        /// </param>
        public void CreateOrUpdateFile(string repository, string branch, File file)
        {
            var repositoryContent = Client.Repository.Content;
            try
            {
                repositoryContent.UpdateFile(
                    Owner,
                    repository,
                    file.Path,
                    new UpdateFileRequest(
                        "Update file.",
                        file.Content,
                        repositoryContent.GetAllContentsByRef(
                            Owner,
                            repository,
                            file.Path,
                            branch
                        ).Result[0].Sha
                    )
                );
            }
            catch (Exception ex)
            {
                ex.Ignore();
                repositoryContent.CreateFile(Owner, repository, file.Path, new CreateFileRequest("Creation File", file.Content, branch));
            }
        }

        /// <summary>
        /// <para>
        /// Closes the issue using the specified issue.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="issue">
        /// <para>The issue.</para>
        /// <para></para>
        /// </param>
        public void CloseIssue(Issue issue)
        {
            IssueUpdate issueUpdate = new()
            {
                State = ItemState.Closed
            };
            Client.Issue.Update(issue.Repository.Owner.Login, issue.Repository.Name, issue.Number, issueUpdate);
        }
    }
}
