

namespace Sitecore.Support.ContentSearch.SolrProvider
{
    using Sitecore.ContentSearch.Diagnostics;
    using Sitecore.ContentSearch.Pipelines;
    using Sitecore.ContentSearch.Pipelines.QueryGlobalFilters;
    using Sitecore.ContentSearch.SearchTypes;
    using Sitecore.ContentSearch.Utilities;
    using System.Linq;
    using System.Collections.Generic;
    using Sitecore.ContentSearch;
    using Sitecore.ContentSearch.Security;
    using Sitecore.ContentSearch.Linq.Common;
    using Sitecore.Diagnostics;
    public class SolrAnalyticsSearchContext : Sitecore.ContentSearch.SolrProvider.SolrAnalyticsSearchContext
    {
        public SolrAnalyticsSearchContext(Sitecore.ContentSearch.SolrProvider.SolrSearchIndex index, SearchSecurityOptions options = SearchSecurityOptions.Default) : base(index, options)
        {
        }

        public override IQueryable<TItem> GetQueryable<TItem>(params IExecutionContext[] executionContexts)
        {
            System.Collections.Generic.List<IExecutionContext> updatedContexts = (from x in executionContexts
                                                                       where !(x is CultureExecutionContext)
                                                                       select x).ToList<IExecutionContext>();            

            var failResistantSolrSearchIndex = this.Index as Sitecore.Support.ContentSearch.SolrProvider.SolrSearchIndex;
            if (failResistantSolrSearchIndex != null && failResistantSolrSearchIndex.PreviousConnectionStatus != ConnectionStatus.Succeded)
            {
                Log.Error("SUPPORT: unable to execute a search query. Solr core [" + failResistantSolrSearchIndex.Core + "] is unavailable.", typeof(SolrSearchContext));
                return new EnumerableQuery<TItem>(new List<TItem>());
            }
            var failResistantSwitchOnRebuildSolrSearchIndex = this.Index as Sitecore.Support.ContentSearch.SolrProvider.SwitchOnRebuildSolrSearchIndex;
            if (failResistantSwitchOnRebuildSolrSearchIndex != null && failResistantSwitchOnRebuildSolrSearchIndex.PreviousConnectionStatus != ConnectionStatus.Succeded)
            {
                Log.Error("SUPPORT: unable to execute a search query. Solr core [" + failResistantSwitchOnRebuildSolrSearchIndex.Core + "] is unavailable.", typeof(SolrSearchContext));
                return new EnumerableQuery<TItem>(new List<TItem>());
            }

            var linqIndex = new Sitecore.Support.ContentSearch.SolrProvider.LinqToSolrIndex<TItem>(this, updatedContexts.ToArray());

            if (ContentSearchConfigurationSettings.EnableSearchDebug)
            {
                (linqIndex as IHasTraceWriter).TraceWriter = new LoggingTraceWriter(SearchLog.Log);
            }

            var queryable = linqIndex.GetQueryable();

            if (typeof(TItem).IsAssignableFrom(typeof(SearchResultItem)))
            {
                var args = new QueryGlobalFiltersArgs(queryable, typeof(TItem), executionContexts.ToList());
                this.Index.Locator.GetInstance<Sitecore.Abstractions.ICorePipeline>().Run(PipelineNames.QueryGlobalFilters, args);
                queryable = (IQueryable<TItem>)args.Query;
            }

            return queryable;
        }
    }
}
