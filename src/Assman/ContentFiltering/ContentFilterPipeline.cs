using System;
using System.Collections.Generic;
using System.Linq;

namespace Assman.ContentFiltering
{
    public class ContentFilterPipeline
    {
        private readonly List<IContentFilter> _filters = new List<IContentFilter>();

        public void AddAt(int index, IContentFilter filter)
        {
            _filters.Insert(index, filter);
        }

        public void Add(IContentFilter filter)
        {
            _filters.Add(filter);
        }

        public void Clear()
        {
            _filters.Clear();
        }

        public void Remove<TFilter>(Predicate<TFilter> predicate) where TFilter : IContentFilter
        {
            var filtersToRemove = _filters.Where(f => f is TFilter && predicate((TFilter)f));

            foreach (var filterToRemove in filtersToRemove)
            {
                _filters.Remove(filterToRemove);
            }
        }

        public string FilterContent(string unfilteredContent, ContentFilterContext context)
        {
            foreach (var filter in _filters)
            {
                unfilteredContent = filter.FilterContent(unfilteredContent, context);
            }

            return unfilteredContent;
        }
    }
}