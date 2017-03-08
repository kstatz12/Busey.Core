using Busey.Core.Shared;

namespace Busey.Core.Command
{
    public interface ICommandHandler<TCommand> : IHandler<TCommand> where TCommand : ICommand
    {
    }
}
