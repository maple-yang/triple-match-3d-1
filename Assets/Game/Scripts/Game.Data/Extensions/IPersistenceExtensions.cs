using Game.Data.Models;

namespace Game.Data.Extensions
{
    public static class PersistenceExtensions
    {
        public static string GetDefaultSaveKey(this IPersistence persistence)
        {
            return persistence.GetType().FullName;
        }
    }
}