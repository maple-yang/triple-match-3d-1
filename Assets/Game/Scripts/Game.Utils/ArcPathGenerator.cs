using UnityEngine;

namespace ProjectAssets.Scripts.Extensions
{
    public class ArcPathGenerator
    {
        public Vector3[] GetPath(Vector2 fromPosition, Vector2 toPosition, int numberPoints, float z = 0)
        {
            Vector3[] linePosition = new Vector3[numberPoints];

            Vector2 perpendicularPosition = GetPerpendicularPosition(fromPosition, toPosition);

            for (int i = 0; i < linePosition.Length; i++)
            {
                var point = GetArcPosition(i, linePosition.Length - 1, fromPosition, toPosition, perpendicularPosition);
                linePosition[i] = new Vector3(point.x, point.y, z);
            }

            return linePosition;
        }

        private Vector3 GetArcPosition(int i, int count, Vector3 fromPosition, Vector3 toPosition, Vector3 perpendicularPosition)
        {
            float t = i / (float)count;

            Vector3 m1 = Vector3.Lerp(fromPosition, perpendicularPosition, t);
            Vector3 m2 = Vector3.Lerp(perpendicularPosition, toPosition, t);

            return Vector3.Lerp(m1, m2, t);
        }

        private Vector2 GetPerpendicularPosition(Vector2 fromPosition, Vector2 toPosition)
        {
            float centerPosX = Mathf.Lerp(fromPosition.x, toPosition.x, 0.5f);
            float centerPosY = Mathf.Lerp(fromPosition.y, toPosition.y, 0.5f);
            Vector2 centerPosition = new Vector2(centerPosX, centerPosY);

            float radius = Vector3.Distance(fromPosition, toPosition) / 2;

            Vector2 direction = (fromPosition - toPosition).normalized;
            Vector2 perpendicular = Vector2.Perpendicular(centerPosition);

            return (perpendicular.normalized + direction) * radius;
        }


        public Vector3[] GetPath2(Vector2 fromPosition, Vector2 toPosition, int numberPoints, float distance)
        {
            Vector3[] linePosition = new Vector3[numberPoints];

            Vector2 perpendicularPosition = GetPerpendicularPosition2(fromPosition, toPosition, distance);

            if (fromPosition.x > toPosition.x)
            {
                perpendicularPosition = -perpendicularPosition;
            }

            for (int i = 0; i < linePosition.Length; i++)
            {
                linePosition[i] = GetArcPosition(i, linePosition.Length - 1, fromPosition, toPosition, perpendicularPosition);
            }

            return linePosition;
        }

        private Vector2 GetPerpendicularPosition2(Vector2 fromPosition, Vector2 toPosition, float distance)
        {
            Vector2 centerPosition = (fromPosition + toPosition) * 0.5f;

            Vector2 direction = (fromPosition - toPosition);
            Vector2 directionNormalized = direction.normalized;
            direction = distance * directionNormalized;

            return centerPosition + new Vector2(direction.y, -direction.x);
        }

        public Vector3[] InvertPathPoints(Vector3[] points)
        {
            Vector3[] result = new Vector3[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                result[i] = points[points.Length - i - 1];
            }

            return result;
        }
    }
}