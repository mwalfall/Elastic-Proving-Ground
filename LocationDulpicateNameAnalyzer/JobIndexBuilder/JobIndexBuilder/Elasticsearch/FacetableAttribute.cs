using JobIndexBuilder.Domain;
using System;

namespace JobIndexBuilder.Elasticsearch
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FacetableAttribute : Attribute
    {
        public FacetableAttribute(string displayName, JobFieldType fieldDataType)
        {
            this.DisplayName = displayName;
            this.FieldDataType = fieldDataType;
        }

        public string DisplayName { get; set; }
        public JobFieldType FieldDataType { get; set; }
    }
}
