using Cayent.Core.CQRS.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.Users.Common.Queries.Query
{
    public sealed class SearchUserQuery : AbstractPagedQuery<SearchUserQuery.User>
    {
        public SearchUserQuery(string criteria, int pageIndex, int pageSize, string sortField, int sortOrder)
            : base("", criteria, pageIndex, pageSize, sortField, sortOrder)
        {

        }

        public class User
        {

        }
    }
}
