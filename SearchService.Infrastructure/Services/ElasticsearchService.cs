using Nest;
using SearchService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SearchService.Application;

namespace SearchService.Infrastructure.Services
{
    public class ElasticsearchService : ISearchService
    {
        private readonly IElasticClient _client;
        private readonly ILogger<ElasticsearchService> _logger;

        public ElasticsearchService(IOptions<ElasticsearchSettings> settings, ILogger<ElasticsearchService> logger)
        {
            _logger = logger;

            var connSettings = new ConnectionSettings(new Uri(settings.Value.Uri))
                .DefaultIndex(settings.Value.DefaultIndex);

            _client = new ElasticClient(connSettings);
        }

        public async Task IndexDocumentAsync<T>(T document, string id) where T : class
        {
            var response = await _client.IndexAsync(document, i => i.Id(id));
            if (!response.IsValid)
                _logger.LogError("Index error: {Error}", response.OriginalException.Message);
        }

        public async Task<T?> GetDocumentAsync<T>(string id) where T : class
        {
            var response = await _client.GetAsync<T>(id);
            return response.Source;
        }

        public async Task DeleteDocumentAsync(string id)
        {
            await _client.DeleteAsync<object>(id);
        }

        public async Task<IEnumerable<T>> SearchAsync<T>(string query) where T : class
        {
            var response = await _client.SearchAsync<T>(s => s
                .Query(q => q.QueryString(d => d.Query(query))));

            return response.Documents;
        }
    }
}
