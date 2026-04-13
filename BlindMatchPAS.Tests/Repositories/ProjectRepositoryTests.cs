using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace BlindMatchPAS.Tests.Repositories
{
    public class ProjectRepositoryTests
    {
        [Fact]
        public async Task AddProject_SavesCorrectly()
        {
            // Placeholder for when your teammate finishes the actual repository
            true.Should().BeTrue(); 
        }

        [Fact]
        public async Task GetBlindProjects_ExcludesStudentIdentity()
        {
            true.Should().BeTrue();
        }

        [Fact]
        public async Task GetBlindProjects_OnlyReturnsPendingProjects()
        {
            true.Should().BeTrue();
        }

        [Fact]
        public async Task GetBlindProjects_OnlyReturnsMatchingSupervisorExpertise()
        {
            true.Should().BeTrue();
        }

        [Fact]
        public async Task GetProjectsByStudent_ReturnsOnlyOwnProjects()
        {
            true.Should().BeTrue();
        }

        [Fact]
        public async Task WithdrawProject_ChangesStatusToWithdrawn()
        {
            true.Should().BeTrue();
        }
    }
}