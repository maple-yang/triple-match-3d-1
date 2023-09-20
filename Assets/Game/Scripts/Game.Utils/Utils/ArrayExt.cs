using System;
using System.Linq;

namespace Game.Utils.Utils
{
    public static class ArrayExt
    {
        /// <summary>
        /// Get slice of array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        /// <summary>
        /// Shuffle array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T[] Shuffle<T>(this T[] array)
        {
            Random rnd = new Random();
            return array.OrderBy(x => rnd.Next()).ToArray();
        }
        /// <summary>
        /// Shuffle array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T[] Shuffle<T>(this T[] array, int seed)
        {
            Random rnd = new Random(seed);
            return array.OrderBy(x => rnd.Next()).ToArray();
        }
    }
}
