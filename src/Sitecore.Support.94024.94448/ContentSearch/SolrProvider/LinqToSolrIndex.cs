namespace Sitecore.Support.ContentSearch.SolrProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Sitecore.ContentSearch.Linq;
    using Sitecore.ContentSearch.Linq.Common;
    using Sitecore.ContentSearch.Linq.Methods;
    using Sitecore.ContentSearch.Linq.Nodes;
    using Sitecore.ContentSearch.Linq.Solr;
    using Sitecore.ContentSearch.Utilities;
    using Sitecore.Diagnostics;
    using SolrNet;
    public class LinqToSolrIndex<TItem> : Sitecore.ContentSearch.SolrProvider.LinqToSolrIndex<TItem>
    {
        private static readonly FieldInfo contextFieldInfo;
        private static readonly MethodInfo executeMethodInfo;
        private static readonly MethodInfo applyScalarMethods;
        private static readonly FieldInfo queryMapperFieldInfo;
        static LinqToSolrIndex()
        {
            contextFieldInfo = typeof(Sitecore.ContentSearch.SolrProvider.LinqToSolrIndex<TItem>).GetField("context",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(contextFieldInfo,
                "Could not find 'context' field in type Sitecore.ContentSearch.SolrProvider.LinqToSolrIndex<TItem>");
            executeMethodInfo = typeof(Sitecore.ContentSearch.SolrProvider.LinqToSolrIndex<TItem>).GetMethod("Execute",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(executeMethodInfo,
                "Could not find 'Execute' method in type Sitecore.ContentSearch.SolrProvider.LinqToSolrIndex<TItem>");
            queryMapperFieldInfo = typeof(SolrIndex<TItem>).GetField("queryMapper",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(contextFieldInfo,
                "Could not find 'queryMapper' field in type SolrIndex<TItem>");
            applyScalarMethods =
                typeof(Sitecore.ContentSearch.SolrProvider.LinqToSolrIndex<TItem>).GetMethod("ApplyScalarMethods",
                    BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(executeMethodInfo,
                "Could not find 'ApplyScalarMethods' method in type Sitecore.ContentSearch.SolrProvider.LinqToSolrIndex<TItem>");
        }

        public LinqToSolrIndex(Sitecore.ContentSearch.SolrProvider.SolrSearchContext context, Sitecore.ContentSearch.Linq.Common.IExecutionContext executionContext) : this(context, new[] { executionContext })
        {
        }

        public LinqToSolrIndex(Sitecore.ContentSearch.SolrProvider.SolrSearchContext context, Sitecore.ContentSearch.Linq.Common.IExecutionContext[] executionContexts) : base(context, executionContexts)
        {
            Sitecore.Support.ContentSearch.SolrProvider.LinqToSolrIndex<TItem>.queryMapperFieldInfo.SetValue(this, new Sitecore.Support.ContentSearch.Linq.Solr.SolrQueryMapper(base.Parameters));
        }

        public override IEnumerable<TElement> FindElements<TElement>(SolrCompositeQuery compositeQuery)
        {
            var failResistantSolrSearchIndex = ((Sitecore.ContentSearch.SolrProvider.SolrSearchContext)contextFieldInfo.GetValue(this)).Index as Sitecore.Support.ContentSearch.SolrProvider.SolrSearchIndex;
            if (failResistantSolrSearchIndex != null)
            {
                if (failResistantSolrSearchIndex.PreviousConnectionStatus == ConnectionStatus.Succeded)
                {
                    return base.FindElements<TElement>(compositeQuery);
                }
                else
                {
                    Log.Error("SUPPORT: unable to execute a search query. Solr core [" + failResistantSolrSearchIndex.Core + "] is unavailable.", typeof(SolrSearchContext));
                }
            }
            var failResistantSwitchOnRebuildSolrSearchIndex = ((Sitecore.ContentSearch.SolrProvider.SolrSearchContext)contextFieldInfo.GetValue(this)).Index as Sitecore.Support.ContentSearch.SolrProvider.SwitchOnRebuildSolrSearchIndex;
            if (failResistantSwitchOnRebuildSolrSearchIndex != null)
            {
                if (failResistantSwitchOnRebuildSolrSearchIndex.PreviousConnectionStatus == ConnectionStatus.Succeded)
                {
                    return base.FindElements<TElement>(compositeQuery);
                }
                else
                {
                    Log.Error("SUPPORT: unable to execute a search query. Solr core [" + failResistantSwitchOnRebuildSolrSearchIndex.Core + "] is unavailable.", typeof(SolrSearchContext));
                }
            }
            return new System.Collections.Generic.List<TElement>();
        }

        public override TResult Execute<TResult>(SolrCompositeQuery compositeQuery)
        {
            Assembly assembly = Assembly.GetAssembly(typeof(Sitecore.ContentSearch.SolrProvider.SolrSearchIndex));
            Type solrSearchResultsType = assembly.GetType("Sitecore.ContentSearch.SolrProvider.SolrSearchResults`1", true);
            if (typeof(TResult).IsGenericType && (typeof(TResult).GetGenericTypeDefinition() == typeof(SearchResults<>)))
            {
                System.Type resultType = typeof(TResult).GetGenericArguments()[0];

                SolrQueryResults<System.Collections.Generic.Dictionary<string, object>> results = null;
                var failResistantSolrSearchIndex = ((Sitecore.ContentSearch.SolrProvider.SolrSearchContext)contextFieldInfo.GetValue(this)).Index as Sitecore.Support.ContentSearch.SolrProvider.SolrSearchIndex;
                if (failResistantSolrSearchIndex != null)
                {
                    if (failResistantSolrSearchIndex.PreviousConnectionStatus != ConnectionStatus.Succeded)
                    {
                        results = new SolrQueryResults<Dictionary<string, object>>();
                        Log.Error("SUPPORT: unable to execute a search query. Solr core [" + failResistantSolrSearchIndex.Core + "] is unavailable.", typeof(SolrSearchContext));
                    }
                }
                var failResistantSwitchOnRebuildSolrSearchIndex = ((Sitecore.ContentSearch.SolrProvider.SolrSearchContext)contextFieldInfo.GetValue(this)).Index as Sitecore.Support.ContentSearch.SolrProvider.SwitchOnRebuildSolrSearchIndex;
                if (failResistantSwitchOnRebuildSolrSearchIndex != null)
                {
                    if (failResistantSwitchOnRebuildSolrSearchIndex.PreviousConnectionStatus != ConnectionStatus.Succeded)
                    {
                        results = new SolrQueryResults<Dictionary<string, object>>();
                        Log.Error("SUPPORT: unable to execute a search query. Solr core [" + failResistantSwitchOnRebuildSolrSearchIndex.Core + "] is unavailable.", typeof(SolrSearchContext));
                    }
                }
                if (results == null)
                { 
                    results = (SolrQueryResults<System.Collections.Generic.Dictionary<string, object>>)executeMethodInfo.Invoke(this, new object[] { compositeQuery, typeof(TResult) });
                }
                System.Type type3 = solrSearchResultsType.MakeGenericType(new System.Type[] { resultType });
                System.Reflection.MethodInfo info2 = base.GetType().BaseType.GetMethod("ApplyScalarMethods", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).MakeGenericMethod(new System.Type[] { typeof(TResult), resultType });
                SelectMethod method = LinqToSolrIndex<TItem>.GetSelectMethod(compositeQuery);
                object obj2 = ReflectionUtility.CreateInstance(type3, (object[])new object[] { (Sitecore.ContentSearch.SolrProvider.SolrSearchContext)contextFieldInfo.GetValue(this), results, method, compositeQuery.ExecutionContexts, compositeQuery.VirtualFieldProcessors });
                return (TResult)info2.Invoke(this, (object[])new object[] { compositeQuery, obj2, results });
            }
            SolrQueryResults<System.Collections.Generic.Dictionary<string, object>> searchResults = null;
            var failResistantSolrSearchIndex2 = ((Sitecore.ContentSearch.SolrProvider.SolrSearchContext)contextFieldInfo.GetValue(this)).Index as Sitecore.Support.ContentSearch.SolrProvider.SolrSearchIndex;
            if (failResistantSolrSearchIndex2 != null)
            {
                if (failResistantSolrSearchIndex2.PreviousConnectionStatus != ConnectionStatus.Succeded)
                {
                    searchResults = new SolrQueryResults<Dictionary<string, object>>();
                    Log.Error("SUPPORT: unable to execute a search query. Solr core [" + failResistantSolrSearchIndex2.Core + "] is unavailable.", typeof(SolrSearchContext));
                }
            }
            var failResistantSwitchOnRebuildSolrSearchIndex2 = ((Sitecore.ContentSearch.SolrProvider.SolrSearchContext)contextFieldInfo.GetValue(this)).Index as Sitecore.Support.ContentSearch.SolrProvider.SwitchOnRebuildSolrSearchIndex;
            if (failResistantSwitchOnRebuildSolrSearchIndex2 != null)
            {
                if (failResistantSwitchOnRebuildSolrSearchIndex2.PreviousConnectionStatus != ConnectionStatus.Succeded)
                {
                    searchResults = new SolrQueryResults<Dictionary<string, object>>();
                    Log.Error("SUPPORT: unable to execute a search query. Solr core [" + failResistantSwitchOnRebuildSolrSearchIndex2.Core + "] is unavailable.", typeof(SolrSearchContext));
                }
            }

            if (searchResults == null)
            { 
                searchResults =
                    (SolrQueryResults<System.Collections.Generic.Dictionary<string, object>>)
                        executeMethodInfo.Invoke(this, new object[] { compositeQuery, typeof(TResult) });
            }
            SelectMethod selectMethod = LinqToSolrIndex<TItem>.GetSelectMethod(compositeQuery);
            var processedResults = (solrSearchResultsType.MakeGenericType(typeof(TResult)).GetConstructor(new Type[] { typeof(Sitecore.ContentSearch.SolrProvider.SolrSearchContext), typeof(SolrQueryResults<Dictionary<string, object>>), typeof(SelectMethod), typeof(IEnumerable<IExecutionContext>), typeof(IEnumerable<IFieldQueryTranslator>) })).Invoke(new object[] { (Sitecore.ContentSearch.SolrProvider.SolrSearchContext)contextFieldInfo.GetValue(this), searchResults, selectMethod, compositeQuery.ExecutionContexts, compositeQuery.VirtualFieldProcessors });
            var ApplyScalarMethodsMethodInfo = base.GetType().BaseType.GetMethod("ApplyScalarMethods", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).MakeGenericMethod(new System.Type[] { typeof(TResult), typeof(TResult) });
            return (TResult)ApplyScalarMethodsMethodInfo.Invoke(this, new object[] { compositeQuery, processedResults, searchResults });
        }

        /// <summary>Gets the select method.</summary>
        /// <param name="compositeQuery">The composite query.</param>
        /// <returns>Select method</returns>
        private static SelectMethod GetSelectMethod(SolrCompositeQuery compositeQuery)
        {
            var selectMethods = compositeQuery.Methods.Where(m => m.MethodType == QueryMethodType.Select).Select(m => (SelectMethod)m).ToList();

            return selectMethods.Count() == 1 ? selectMethods[0] : null;
        }

        protected object ApplyScalarMethods<TResult, TDocument>(SolrCompositeQuery compositeQuery, object processedResults, SolrQueryResults<Dictionary<string, object>> results)
        {
            return applyScalarMethods.MakeGenericMethod(new Type[] { typeof(TResult), typeof(TDocument) }).Invoke(this, new object[] { compositeQuery, processedResults, results });
        }
    }
}
