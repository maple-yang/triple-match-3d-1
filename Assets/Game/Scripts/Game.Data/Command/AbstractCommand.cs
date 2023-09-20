using System;

namespace Game.Data.Command
{
    public abstract class AbstractCommand : ICommand
    {
        public bool IsCompleted { get; private set; } = false;
        public event Action<ICommand, bool> Completed;
        public abstract void Execute();

        protected void OnCompleted(bool result)
        {
            IsCompleted = true;
            Completed?.Invoke(this, result);
        }
    }
}