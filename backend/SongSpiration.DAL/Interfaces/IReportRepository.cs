using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SongSpiration.Models.Entities;

namespace SongSpiration.DAL.Interfaces;

public interface IReportRepository : IRepository<Report>
{
    Task<IEnumerable<Report>> GetReportsByReportedUserIdAsync(Guid reportedUserId);
    Task<IEnumerable<Report>> GetReportsByReportingUserIdAsync(Guid reportingUserId);
}