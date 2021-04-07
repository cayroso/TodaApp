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
            public string UserId { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }

            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
        }
    }
}
