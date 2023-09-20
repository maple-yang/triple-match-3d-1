using System.Collections.Generic;

namespace Game.Utils.Utils
{
    public static class ListExt
    {
        /// <summary>
        /// Shuffle the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static void Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        /// <summary>
        /// Shuffle the list using the specified seed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static void Shuffle<T>(this List<T> list, int seed)
        {
            UnityEngine.Random.InitState(seed: seed);
            Shuffle(list);
        }
    }
}
