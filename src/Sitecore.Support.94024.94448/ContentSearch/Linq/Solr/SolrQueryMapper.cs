namespace Sitecore.Support.ContentSearch.Linq.Solr
{
    using Sitecore.ContentSearch.Linq.Nodes;
    using Sitecore.ContentSearch.Linq.Solr;
    using SolrNet;
    public class SolrQueryMapper : Sitecore.ContentSearch.Linq.Solr.SolrQueryMapper
    {
        public SolrQueryMapper(SolrIndexParameters parameters) : base(parameters)
        {
        }

        protected override AbstractSolrQuery VisitNot(NotNode node, SolrQueryMapperState state)
        {
            var operand = this.Visit(node.Operand, state);

            var field = operand as SolrQueryByField;

            if (field != null)
            {
                if (string.IsNullOrEmpty(field.FieldValue))
                {
                    return new SolrMultipleCriteriaQuery(new ISolrQuery[] { new SolrNotQuery(operand), new SolrHasValueQuery(field.FieldName) });
                }
            }

            var notField = operand as SolrNotQuery;

            if (notField != null)
            {
                return (AbstractSolrQuery)notField.Query;
            }

            var result = new SolrMultipleCriteriaQuery(new ISolrQuery[] { new SolrNotQuery(operand), SolrQuery.All });
            return result;
        }
    }
}

