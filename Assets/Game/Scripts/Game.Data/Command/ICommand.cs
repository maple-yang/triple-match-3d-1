using System;

namespace Game.Data.Command
{
    public interface ICommand
    {
        event Action<ICommand, bool> Completed;
        
        void Execute();
    }
}