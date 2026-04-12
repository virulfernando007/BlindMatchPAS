using BlindMatchPAS.Data;
using BlindMatchPAS.Models;
using Microsoft.EntityFrameworkCore;

namespace BlindMatchPAS.Repositories
{
    // ─── Generic Repository Interface ─────────────────────────────────────────

    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task SaveChangesAsync();
    }

    // ─── Project Repository ───────────────────────────────────────────────────

    public interface IProjectRepository : IRepository<Project>
    {
        Task<IEnumerable<Project>> GetBlindProjectsForSupervisorAsync(string supervisorId);
        Task<IEnumerable<Project>> GetProjectsByStudentAsync(string studentId);
        Task<Project?> GetProjectWithDetailsAsync(int projectId);
    }

    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Project?> GetByIdAsync(int id) =>
            await _context.Projects.FindAsync(id);

        public async Task<IEnumerable<Project>> GetAllAsync() =>
            await _context.Projects
                .Include(p => p.ResearchArea)
                .ToListAsync();

        public async Task AddAsync(Project entity) =>
            await _context.Projects.AddAsync(entity);

        public async Task UpdateAsync(Project entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _context.Projects.Update(entity);
        }

        public async Task DeleteAsync(Project entity) =>
            _context.Projects.Remove(entity);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();

        /// <summary>
        /// CRITICAL: Deliberately excludes Student navigation property.
        /// Anonymity is enforced at the data layer — not just the view.
        /// </summary>
        public async Task<IEnumerable<Project>> GetBlindProjectsForSupervisorAsync(string supervisorId)
        {
            var expertiseAreaIds = await _context.SupervisorExpertises
                .Where(se => se.SupervisorId == supervisorId)
                .Select(se => se.ResearchAreaId)
                .ToListAsync();

            var interactedProjectIds = await _context.Matches
                .Where(m => m.SupervisorId == supervisorId)
                .Select(m => m.ProjectId)
                .ToListAsync();

            return await _context.Projects
                .Include(p => p.ResearchArea)
                // Deliberately NO .Include(p => p.Student)
                .Where(p => p.Status == ProjectStatus.Pending
                         && expertiseAreaIds.Any(id => p.ResearchAreaIds.Contains(id.ToString()))
                         && !interactedProjectIds.Contains(p.Id))
                .OrderByDescending(p => p.SubmittedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetProjectsByStudentAsync(string studentId) =>
            await _context.Projects
                .Include(p => p.ResearchArea)
                .Include(p => p.Match)
                    .ThenInclude(m => m!.Supervisor)
                .Where(p => p.StudentId == studentId)
                .OrderByDescending(p => p.SubmittedAt)
                .ToListAsync();

        public async Task<Project?> GetProjectWithDetailsAsync(int projectId) =>
            await _context.Projects
                .Include(p => p.ResearchArea)
                .Include(p => p.Student)
                .Include(p => p.Match)
                    .ThenInclude(m => m!.Supervisor)
                .FirstOrDefaultAsync(p => p.Id == projectId);
    }

    // ─── Match Repository ─────────────────────────────────────────────────────

    public interface IMatchRepository : IRepository<Match>
    {
        Task<Match?> GetMatchWithDetailsAsync(int matchId);
        Task<IEnumerable<Match>> GetMatchesBySupervisorAsync(string supervisorId);
        Task<IEnumerable<Match>> GetAllMatchesWithDetailsAsync();
        Task<int> GetConfirmedMatchCountForSupervisorAsync(string supervisorId);
    }

    public class MatchRepository : IMatchRepository
    {
        private readonly ApplicationDbContext _context;

        public MatchRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Match?> GetByIdAsync(int id) =>
            await _context.Matches.FindAsync(id);

        public async Task<IEnumerable<Match>> GetAllAsync() =>
            await _context.Matches.ToListAsync();

        public async Task AddAsync(Match entity) =>
            await _context.Matches.AddAsync(entity);

        public async Task UpdateAsync(Match entity) =>
            _context.Matches.Update(entity);

        public async Task DeleteAsync(Match entity) =>
            _context.Matches.Remove(entity);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();

        public async Task<Match?> GetMatchWithDetailsAsync(int matchId) =>
            await _context.Matches
                .Include(m => m.Project)
                    .ThenInclude(p => p!.ResearchArea)
                .Include(m => m.Project)
                    .ThenInclude(p => p!.Student)
                .Include(m => m.Supervisor)
                .FirstOrDefaultAsync(m => m.Id == matchId);

        public async Task<IEnumerable<Match>> GetMatchesBySupervisorAsync(string supervisorId) =>
            await _context.Matches
                .Include(m => m.Project)
                    .ThenInclude(p => p!.ResearchArea)
                .Include(m => m.Project)
                    .ThenInclude(p => p!.Student)
                .Where(m => m.SupervisorId == supervisorId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<Match>> GetAllMatchesWithDetailsAsync() =>
            await _context.Matches
                .Include(m => m.Project)
                    .ThenInclude(p => p!.ResearchArea)
                .Include(m => m.Project)
                    .ThenInclude(p => p!.Student)
                .Include(m => m.Supervisor)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

        public async Task<int> GetConfirmedMatchCountForSupervisorAsync(string supervisorId) =>
            await _context.Matches
                .CountAsync(m => m.SupervisorId == supervisorId
                              && m.Status == MatchStatus.Confirmed);
    }

    // ─── Research Area Repository ─────────────────────────────────────────────

    public interface IResearchAreaRepository : IRepository<ResearchArea>
    {
        Task<IEnumerable<ResearchArea>> GetActiveAreasAsync();
    }

    public class ResearchAreaRepository : IResearchAreaRepository
    {
        private readonly ApplicationDbContext _context;

        public ResearchAreaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ResearchArea?> GetByIdAsync(int id) =>
            await _context.ResearchAreas.FindAsync(id);

        public async Task<IEnumerable<ResearchArea>> GetAllAsync() =>
            await _context.ResearchAreas
                .OrderBy(r => r.Name)
                .ToListAsync();

        public async Task<IEnumerable<ResearchArea>> GetActiveAreasAsync() =>
            await _context.ResearchAreas
                .Where(r => r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync();

        public async Task AddAsync(ResearchArea entity) =>
            await _context.ResearchAreas.AddAsync(entity);

        public async Task UpdateAsync(ResearchArea entity) =>
            _context.ResearchAreas.Update(entity);

        public async Task DeleteAsync(ResearchArea entity) =>
            _context.ResearchAreas.Remove(entity);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}