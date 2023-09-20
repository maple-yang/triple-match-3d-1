using UnityEngine;

namespace Game.Utils.Utils
{
    public static class GameObjectTools
    {
        /// <summary>
        /// Получение границ объекта
        /// </summary>
        /// <returns>The game object bounds.</returns>
        /// <param name="bounds">Bounds.</param>
        public static Bounds GetGameObjectBounds(GameObject gObject)
        {
            Bounds resultBounds = default(Bounds);

            // Получение всех рендереров
            Renderer[] renderers = gObject.GetComponentsInChildren<Renderer>();
            if (renderers != null && renderers.Length > 0)
            {
                for(int i = 0, l = renderers.Length; i < l; i++)
                {
                    // Получение границ рендерера
                    Renderer renderer = renderers[i];

                    MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
                    SkinnedMeshRenderer skinnedMeshRenderer = renderer.GetComponent<SkinnedMeshRenderer>();

                    Mesh mesh = null;
                    if (meshFilter)
                    {
                        mesh = Application.isPlaying ? meshFilter.mesh : meshFilter.sharedMesh;
                    }
                    bool hasMesh = (Mesh)mesh;
                    if (!hasMesh && skinnedMeshRenderer)
                    {
                        if (Application.isPlaying)
                        {
                            mesh = new Mesh();
                            skinnedMeshRenderer.BakeMesh(mesh);
                        }
                        else
                        {
                            mesh = skinnedMeshRenderer.sharedMesh;
                        }
                    }

                    if (mesh)
                        mesh.RecalculateBounds();
                    else
                    {
                        if (Application.isPlaying && !hasMesh)
                            Object.Destroy(mesh);
                        continue;
                    }

                    Bounds rendererBounds = renderer.bounds;
                    if (Application.isPlaying && !hasMesh)
                        Object.Destroy(mesh);

                    if (rendererBounds.size.sqrMagnitude > 0)
                    {
                        if (resultBounds.size.sqrMagnitude > 0)
                        {
                            // Перенастройка результирующих границ
                            resultBounds.min = new Vector3(
                                (resultBounds.min.x > rendererBounds.min.x) ? rendererBounds.min.x : resultBounds.min.x,
                                (resultBounds.min.y > rendererBounds.min.y) ? rendererBounds.min.y : resultBounds.min.y,
                                (resultBounds.min.z > rendererBounds.min.z) ? rendererBounds.min.z : resultBounds.min.z
                            );
                            resultBounds.max = new Vector3(
                                (resultBounds.max.x < rendererBounds.max.x) ? rendererBounds.max.x : resultBounds.max.x,
                                (resultBounds.max.y < rendererBounds.max.y) ? rendererBounds.max.y : resultBounds.max.y,
                                (resultBounds.max.z < rendererBounds.max.z) ? rendererBounds.max.z : resultBounds.max.z
                            );
                        }
                        else
                            resultBounds = rendererBounds;
                    }
                }
            }

            resultBounds = new Bounds(
                resultBounds.center,
                new Vector3(
                    Mathf.Abs(resultBounds.size.x),
                    Mathf.Abs(resultBounds.size.y),
                    Mathf.Abs(resultBounds.size.z)
                )
            );

            return resultBounds;
        }
    }
}
