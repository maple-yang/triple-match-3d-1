using UnityEngine;

namespace Game.Utils.Utils
{
    public static class TransformTools
    {
        /// <summary>
        /// Удаление всех дочерних элементов у трансформа
        /// </summary>
        /// <param name="transform">Transform.</param>
        public static void RemoveAllChildren(Transform transform)
        {
            for(int i = (transform.childCount - 1); i >= 0; i--)
                if (Application.isPlaying)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                    GameObject.Destroy(transform.GetChild(i).gameObject);
                }
                else
                    GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
        }

        /// <summary>
        /// Расчет масштаба объекта в зависимости от граничной области
        /// </summary>
        /// <returns>The D object scale.</returns>
        /// <param name="gObjectBounds">G object bounds.</param>
        /// <param name="borderBounds">Border bounds.</param>
        public static float Calculate3DObjectScale(Bounds gObjectBounds, Bounds borderBounds)
        {
            // Поиск максимального изменения масштаба
            float factorX = Mathf.Abs(borderBounds.size.x / gObjectBounds.size.x);
            float factorY = Mathf.Abs(borderBounds.size.y / gObjectBounds.size.y);
            float factorZ = Mathf.Abs(borderBounds.size.z / gObjectBounds.size.z);
            float resultScale = Mathf.Min(factorX, factorY, factorZ);

            return resultScale;
        }
    }
}
