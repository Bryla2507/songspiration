using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SongSpiration.DAL.Interfaces;
using SongSpiration.Models.Entities;

namespace SongSpiration.DAL.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly SongSpirationDbContext _context;

        public ReportRepository(SongSpirationDbContext context)
        {
            _context = context;
        }

        public async Task<Report?> GetByIdAsync(Guid id)
        {
            return await _context.Reports.FindAsync(id);
        }

        public async Task<IEnumerable<Report>> GetAllAsync()
        {
            return await _context.Reports
                .Include(r => r.ReportedUser)
                .Include(r => r.ReportingUser)
                .ToListAsync();
        }

        public async Task AddAsync(Report report)
        {
            await _context.Reports.AddAsync(report);
            await _context.SaveChangesAsync();
        }

        public void Update(Report report)
        {
            _context.Reports.Update(report);
        }

        public void Remove(Report report)
        {
            _context.Reports.Remove(report);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Report>> GetReportsByReportedUserIdAsync(Guid reportedUserId)
        {
            return await _context.Reports
                .Where(r => r.ReportedUserId == reportedUserId)
                .Include(r => r.ReportedUser)
                .Include(r => r.ReportingUser)
                .ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetReportsByReportingUserIdAsync(Guid reportingUserId)
        {
            return await _context.Reports
                .Where(r => r.ReportingUserId == reportingUserId)
                .Include(r => r.ReportedUser)
                .Include(r => r.ReportingUser)
                .ToListAsync();
        }
    }
}