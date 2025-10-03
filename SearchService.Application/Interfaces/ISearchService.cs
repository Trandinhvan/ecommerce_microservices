using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchService.Application.Interfaces
{
    public interface ISearchService
    {
        Task IndexDocumentAsync<T>(T document, string id) where T : class;
        Task<T?> GetDocumentAsync<T>(string id) where T : class;
        Task DeleteDocumentAsync(string id);
        Task<IEnumerable<T>> SearchAsync<T>(string query) where T : class;

    }
}
