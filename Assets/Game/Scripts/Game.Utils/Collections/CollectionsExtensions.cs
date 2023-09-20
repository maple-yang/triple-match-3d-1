using System.Collections.Generic;
using System.Linq;

namespace Game.Utils.Collections
{
    public static class CollectionsExtensions
    {
        public static IList<T> MoveArray<T>(this IList<T> array, int startIndex)
        {
            return MoveArray(array.ToArray(), startIndex);
        }

        public static T[] MoveArray<T>(this T[] array, int startIndex)
        {
            var movedArray = new T[array.Length];
            for (int i = 0; i < movedArray.Length - 1; i++)
            {
                var item = array[i];
                if (i >= startIndex)
                {
                    movedArray[i + 1] = item;
                }
                else
                {
                    movedArray[i] = item;
                }
            }

            return movedArray;
        }
    }
}