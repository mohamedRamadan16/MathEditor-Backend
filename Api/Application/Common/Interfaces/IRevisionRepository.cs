using System;
using System.Threading.Tasks;
using Api.Domain.Entities;

namespace Api.Application.Common.Interfaces
{
    public interface IRevisionRepository
    {
        Task<Revision?> FindByIdAsync(Guid id);
        Task<Revision> CreateAsync(Revision revision);
        Task<Revision> UpdateAsync(Revision revision);
        Task DeleteAsync(Guid id);
    }
}
