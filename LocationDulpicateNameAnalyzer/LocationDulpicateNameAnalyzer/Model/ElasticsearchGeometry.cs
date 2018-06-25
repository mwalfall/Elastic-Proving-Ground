using Nest;
using System;

namespace LocationDulpicateNameAnalyzer.Model
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
