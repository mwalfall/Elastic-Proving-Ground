using Nest;
using System;

namespace Domain.ElasticsearchDocuments
{
    [Serializable]
    public class ElasticsearchGeometry
    {

        [ElasticProperty(Name = "lat")]
        public float Latitude { get; set; }

        [ElasticProperty(Name = "lon")]
        public float Longitude { get; set; }
    }
}
