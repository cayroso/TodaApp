using Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS
{
    public interface IQuery<out TResponse> where TResponse : class// IResponse
    {
        string CorrelationId { get; }
        string TenantId { get; }
        string UserId { get; }
    }

    public abstract class AbstractQuery<TResponse> : IQuery<TResponse> where TResponse : class
    {
        public AbstractQuery(string correlationId, string tenantId, string userId)
        {
            CorrelationId = correlationId;
            TenantId = tenantId;
            UserId = userId;
        }

        public string CorrelationId { get; }
        public string TenantId { get; }
        public string UserId { get; }
    }

    public abstract class AbstractPagedQuery<TResponse> : IQuery<Paged<TResponse>> where TResponse : class
    {
        public AbstractPagedQuery(string correlationId, string tenantId, string userId, string criteria, int pageIndex, int pageSize, string sortField, int sortOrder)
        {

            CorrelationId = correlationId;
            TenantId = tenantId;
            UserId = userId;
            Criteria = criteria;
            PageIndex = pageIndex;
            PageSize = pageSize;
            SortField = sortField;
            SortOrder = sortOrder;
        }

        public string CorrelationId { get; }
        public string TenantId { get; }
        public string UserId { get; }
        public string Criteria { get; }
        public int PageIndex { get; }
        public int PageSize { get; }
        public string SortField { get; }
        public int SortOrder { get; }
    }

    public interface IQueryHandler<in TQuery, TResponse>
        where TQuery : IQuery<TResponse>
        where TResponse : class// IResponse
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TResponse> HandleAsync(TQuery query);

    }

    public interface IQueryHandlerDispatcher
    {
        /// <summary>
        /// Creates the query handler for the given query, then executes it, then returns TResult
        /// </summary>
        /// <typeparam name="TQuery">iquery</typeparam>
        /// <typeparam name="TResult">object</typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TResult> HandleAsync<TQuery, TResult>(TQuery query)
            where TQuery : IQuery<TResult>
            where TResult : class;// IResponse;
    }

    public sealed class QueryHandlerDispatcher : IQueryHandlerDispatcher
    {
        readonly IQueryHandlerFactory _factory;

        public QueryHandlerDispatcher(IQueryHandlerFactory factory)
        {
            _factory = factory;
        }

        public Task<TResult> HandleAsync<TQuery, TResult>(TQuery query)
            where TQuery : IQuery<TResult>
            where TResult : class// IResponse
        {
            var handler = _factory.Create<TQuery, TResult>();

            var result = handler.HandleAsync(query);

            return result;
        }
    }

    public interface IQueryHandlerFactory
    {
        /// <summary>
        /// Creates a query handler for the given query and result
        /// </summary>
        /// <typeparam name="TQuery"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        IQueryHandler<TQuery, TResult> Create<TQuery, TResult>() where TQuery : IQuery<TResult> where TResult : class;// IResponse;
    }

    public sealed class DefaultQueryHandlerFactory : IQueryHandlerFactory
    {
        private readonly IContainer _container;

        public DefaultQueryHandlerFactory(IContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        IQueryHandler<TQuery, TResult> IQueryHandlerFactory.Create<TQuery, TResult>()
        {
            var handler = _container.Resolve<IQueryHandler<TQuery, TResult>>();

            return handler;
        }
    }
}
