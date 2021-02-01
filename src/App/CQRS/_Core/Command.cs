using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS
{
    public interface ICommand
    {
        /// <summary>
        /// Business transaction id
        /// </summary>
        string CorrelationId { get; }
        string TenantId { get; }
        string UserId { get; }

    }

    public abstract class AbstractCommand : ICommand
    {
        public AbstractCommand(string correlationId, string tenantId, string userId)
        {
            CorrelationId = correlationId;
            TenantId = tenantId;
            UserId = userId;
        }

        public string CorrelationId { get; }
        public string TenantId { get; }
        public string UserId { get; }

    }

    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        Task HandleAsync(TCommand command);
    }

    public interface ICommandHandlerDispatcher
    {

        /// <summary>
        /// Calls the handle method of the command handler for the given command
        /// </summary>
        /// <typeparam name="TCommand">subclass of ICommand</typeparam>
        /// <param name="command">command to execute</param>
        Task HandleAsync<TCommand>(TCommand command) where TCommand : ICommand;

        //void Handle<TCommand>(TCommand[] commands) where TCommand : ICommand;
    }

    public sealed class DefaultCommandHandlerDispatcher : ICommandHandlerDispatcher
    {
        private readonly ICommandHandlerFactory _factory;

        public DefaultCommandHandlerDispatcher(ICommandHandlerFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException("factory");
        }

        //public void Handle<TCommand>(TCommand command) where TCommand : ICommand
        //{
        //    var handler = _factory.Create<TCommand>();

        //    handler.Handle(command);
        //}

        Task ICommandHandlerDispatcher.HandleAsync<TCommand>(TCommand command)
        {
            var handler = _factory.Create<TCommand>();

            var result = handler.HandleAsync(command);

            return result;
        }

    }

    public interface ICommandHandlerFactory
    {
        /// <summary>
        /// Creates a command handler for the command
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <returns></returns>
        ICommandHandler<TCommand> Create<TCommand>() where TCommand : ICommand;
    }

    public sealed class DefaultCommandHandlerFactory : ICommandHandlerFactory
    {
        private readonly IContainer _container;

        public DefaultCommandHandlerFactory(IContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        ICommandHandler<TCommand> ICommandHandlerFactory.Create<TCommand>()
        {
            var handler = _container.Resolve<ICommandHandler<TCommand>>();

            return handler;
        }
    }
}
