using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchService.Application
{
    public class ElasticsearchSettings
    {
        public string Uri { get; set; } = string.Empty;
        public string DefaultIndex { get; set; } = string.Empty;
    }
}
