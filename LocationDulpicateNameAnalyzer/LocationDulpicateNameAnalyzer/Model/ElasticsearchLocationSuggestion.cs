using Nest;
using System;
using System.Collections.Generic;

namespace LocationDulpicateNameAnalyzer.Model
{
    [Serializable]
    public class ElasticsearchLocationSuggestion
    {
        [ElasticProperty(Name = "input")]
        public List<string> Input { get; set; }

        [ElasticProperty(Name = "output")]
        public string Output { get; set; }

        [ElasticProperty(Name = "payload")]
        public ElasticsearchLocationPayload Payload { get; set; }

        [ElasticProperty(Name = "weight")]
        public uint? Weight { get; set; }
    }
}
