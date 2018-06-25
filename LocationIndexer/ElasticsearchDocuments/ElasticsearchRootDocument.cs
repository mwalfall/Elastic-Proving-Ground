using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticsearchDocuments
{
    public abstract class ElasticsearchRootDocument
    {
        [ElasticProperty(Name = "id")]
        public long ID { get; set; }
    }
}
