using Game.Data.Extensions;
using Game.Data.Models;
using Game.Data.Storage;
using UniRx;
using Zenject;

namespace Game.Data.Command
{
    public class LoadDataCommand : AbstractCommand
    {
        private readonly IStorage _storage;
        private readonly IPersistence _data;
        private readonly SaveDataCommand.Factory _saveDataFactory;

        public LoadDataCommand(IStorage storage, IPersistence data, SaveDataCommand.Factory saveDataFactory)
        {
            _storage = storage;
            _data = data;
            _saveDataFactory = saveDataFactory;
        }

        public override void Execute()
        {
            _storage
                .HasProperty(_data.GetDefaultSaveKey())
                .Subscribe(result =>
                {
                    if (result)
                    {
                        LoadData();
                    }
                    else
                    {
                        var createDataCommand = _saveDataFactory.Create(_data);
                        createDataCommand.Completed += OnCreatedUserResult;
                        createDataCommand.Execute();
                    }
                });
        }

        private void OnCreatedUserResult(ICommand command, bool result)
        {
            command.Completed -= OnCreatedUserResult;

            if (result)
            {
                LoadData(true);
            }
            else
            {
                OnCompleted(false);
            }
        }

        private void LoadData(bool justCreated = false)
        {
            _storage.Load(_data, _data.GetDefaultSaveKey()).Subscribe((res) =>
            {
                _data.JustCreate = justCreated;
                OnCompleted(res);
            });
        }

        public class Factory : PlaceholderFactory<IPersistence, LoadDataCommand>
        {
        }
    }
}