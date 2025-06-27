using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using Api.Domain.Entities;

namespace Api.Application.Common.Interfaces
{
    public interface IDocumentRepository
    {
        Task<Document?> FindByIdAsync(Guid id);
        Task<Document?> FindByHandleAsync(string handle);
        Task<Document> CreateAsync(Document doc);
        Task<Document> UpdateAsync(Document doc);
        Task DeleteAsync(Guid id);
        Task<(List<Document> Items, int TotalCount)> GetPublishedPagedAsync(int page, int pageSize, bool tracked = false);
    }
}
