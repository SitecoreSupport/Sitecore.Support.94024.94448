using System.Linq;

namespace Sitecore.Support.ContentSearch.SolrProvider
{
    public class SolrAnalyticsSearchContext : Sitecore.ContentSearch.SolrProvider.SolrAnalyticsSearchContext
    {
        public SolrAnalyticsSearchContext(Sitecore.ContentSearch.SolrProvider.SolrSearchIndex index, SearchSecurityOptions options = SearchSecurityOptions.Default) : base(index, options)
        {
        }

        public override IQueryable<TItem> GetQueryable<TItem>(params IExecutionContext[] executionContexts)
        {
            System.Globalization.CultureInfo cultureInfo = System.Globalization.CultureInfo.GetCultureInfo(ContentSearchManager.SearchConfiguration.AnalyticsDefaultLanguage);
            System.Collections.Generic.List<IExecutionContext> list = (from x in executionContexts
                                                                       where !(x is CultureExecutionContext)
                                                                       select x).ToList<IExecutionContext>();
            list.Add(new CultureExecutionContext(cultureInfo));
            return base.GetQueryable<TItem>(list.ToArray());
        }
    }
}
