﻿using App.CQRS.Users.Common.Queries.Query;
using Cayent.Core.Common;
using Cayent.Core.CQRS.Queries;
using Data.App.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cayent.Core.Common.Extensions;

namespace App.CQRS.Users.Common.Queries.Handler
{
    public sealed class UserCommonQueryHandler :
        IQueryHandler<GetUserByIdQuery, GetUserByIdQuery.User>,
        IQueryHandler<SearchUserQuery, Paged<SearchUserQuery.User>>
    {
        readonly AppDbContext _dbContext;
        public UserCommonQueryHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        async Task<GetUserByIdQuery.User> IQueryHandler<GetUserByIdQuery, GetUserByIdQuery.User>.HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            var sql = from u in _dbContext.Users

                      where u.UserId == query.UserIdToGet

                      select new GetUserByIdQuery.User
                      {
                          UserId = u.UserId,
                          FirstName = u.FirstName,
                          MiddleName = u.MiddleName,
                          LastName = u.LastName,
                          Email = u.Email,
                          PhoneNumber = u.PhoneNumber,
                          Roles = u.UserRoles.Select(e => new GetUserByIdQuery.Role
                          {
                              RoleId = e.Role.RoleId,
                              Name = e.Role.Name
                          })
                      };

            var dto = await sql.FirstOrDefaultAsync();

            return dto;
        }

        async Task<Paged<SearchUserQuery.User>> IQueryHandler<SearchUserQuery, Paged<SearchUserQuery.User>>.HandleAsync(SearchUserQuery query, CancellationToken cancellationToken)
        {
            var sql = from u in _dbContext.Users

                      select new SearchUserQuery.User
                      {
                          UserId = u.UserId,
                          FirstName = u.FirstName,
                          MiddleName = u.MiddleName,
                          LastName = u.LastName,
                          Email = u.Email,
                          PhoneNumber = u.PhoneNumber,
                      };

            var dto = await sql.ToPagedItemsAsync(query.PageIndex, query.PageSize);

            return dto;
        }
    }
}
