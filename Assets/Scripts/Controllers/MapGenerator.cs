using Controllers;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.HableCurve;

namespace TerrainSystem
{
    public class MapGenerator
    {
        GameManager gameManager;
        EndPoint start => gameManager.chain.start;
        EndPoint end => gameManager.chain.end;
        public MapGenerator(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public List<Vector3> GenerateMap(int segements, float lengthVariation, float curviness, float heightChance)
        {
            // Get the start and end positions
            Vector3 startPos = start.transform.position; 
            Vector3 endPos = end.transform.position; 

            // Initialize the list of points
            List<Vector3> points = new List<Vector3>();

            // Add the start point
            points.Add(startPos);

            // Generate random points between start and end
            Vector3 direction = (endPos - startPos).normalized; // Base direction
            float totalDistance = Vector3.Distance(startPos, endPos); // Total length of the spline

            float accumulatedDistance = 0f;

            for (int i = 1; i < segements; i++)
            {
                // Calculate random offset for the length variation
                float segmentLength = totalDistance / segements;
                float randomLengthOffset = UnityEngine.Random.Range(-lengthVariation, lengthVariation);
                float stepLength = Mathf.Clamp(segmentLength + randomLengthOffset, 0.1f, totalDistance - accumulatedDistance);
                accumulatedDistance += stepLength;

                // Calculate the base point along the straight line
                Vector3 straightPoint = startPos + direction * accumulatedDistance;

                // Add curviness by introducing a perpendicular offset
                Vector3 perp = Vector3.Cross(direction, Vector3.up).normalized; // Perpendicular vector
                float curveOffset = UnityEngine.Random.Range(-curviness, curviness);

                // Introduce height variation
                float heightOffset = UnityEngine.Random.value < heightChance ? UnityEngine.Random.Range(-1f, 1f) : 0f;

                // Final point calculation
                Vector3 finalPoint = straightPoint + perp * curveOffset + Vector3.up * heightOffset;

                points.Add(finalPoint);
            }

            // Add the end point
            points.Add(endPos);

            return points;
        }

        
    }
}