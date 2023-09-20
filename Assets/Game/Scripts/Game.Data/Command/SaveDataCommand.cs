using Game.Data.Extensions;
using Game.Data.Models;
using Game.Data.Storage;
using Zenject;

namespace Game.Data.Command
{
    public class SaveDataCommand : AbstractCommand
    {
        private readonly IStorage _storage;
        private readonly IPersistence _data;

        public SaveDataCommand(IStorage storage, IPersistence data)
        {
            _storage = storage;
            _data = data;
        }

        public override void Execute()
        {
            _storage.Save(_data, _data.GetDefaultSaveKey());
            OnCompleted(true);
        }

        public class Factory : PlaceholderFactory<IPersistence, SaveDataCommand>
        {
        }
    }
}