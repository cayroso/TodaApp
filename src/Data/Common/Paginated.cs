using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cayent.Core.Common;
using Microsoft.EntityFrameworkCore;

namespace Data.Common
{
    public static class IQueryableExtensions
    {
        public static async Task<Paged<T>> ToPagedItemsAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize) where T : class
        {
            var count = await source.CountAsync().ConfigureAwait(false);
            var collection = await source
                .AsNoTracking()
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync()
                .ConfigureAwait(false);

            return new Paged<T>(collection, count, pageIndex, pageSize);
        }

    }

}
