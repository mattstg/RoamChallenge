using Controllers;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using static UnityEngine.Rendering.HableCurve;

namespace TerrainSystem
{
    public class MapGenerator
    {
        public static readonly float WORLD_FLOOR = -5;
        const float GAP_CHANCE = .2f;
        const float MIN_BUFFER = .3f;
        const float MIN_END_BUFFER = 2f;
        const float HEIGHT_INCREMENT = 1f;

        GameManager gameManager;
        EndPoint start => gameManager.startPt;
        EndPoint end => gameManager.endPt;
        public MapGenerator(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public List<Vector3> GeneratePoints(int segements, float lengthVariation, float curviness, float heightChance)
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
            float segmentLength = totalDistance / segements;
            float minSegementLength = segmentLength * .3f;

            for (int i = 1; i < segements; i++)
            {
                // Calculate random offset for the length variation
                float randomLengthOffset = UnityEngine.Random.Range(-lengthVariation, lengthVariation);
                float stepLength = Mathf.Clamp(segmentLength + randomLengthOffset, minSegementLength, totalDistance - accumulatedDistance);
                accumulatedDistance += stepLength;

                // Calculate the base point along the straight line
                Vector3 straightPoint = startPos + direction * accumulatedDistance;

                // Add curviness by introducing a perpendicular offset
                Vector3 perp = Vector3.Cross(direction, Vector3.up).normalized; // Perpendicular vector
                float curveOffset = UnityEngine.Random.Range(-curviness, curviness);

                // Introduce height variation, but only ever other point
                Vector3 previousHeight = points[i - 1];
                previousHeight.x = 0;
                previousHeight.z = 0;
                float heightOffset;
                if (i % 2 == 0)
                {
                    if (previousHeight.y <= 0)
                    {
                        //we are at floor, so dont move or go up
                        heightOffset = UnityEngine.Random.value < heightChance ? HEIGHT_INCREMENT : 0f;
                    }
                    else
                    {
                        heightOffset = UnityEngine.Random.value < heightChance ? MathHelper.RandomSign() * HEIGHT_INCREMENT : 0f;
                    }
                }
                else
                {
                    heightOffset = 0;
                }
                // Final point calculation
                straightPoint = new Vector3(straightPoint.x, 0, straightPoint.z);
                perp = new Vector3(perp.x, 0, perp.z);

                Vector3 finalPoint = straightPoint + perp * curveOffset + Vector3.up * heightOffset + previousHeight;

                if (Vector3.Distance(startPos, finalPoint) + MIN_END_BUFFER > totalDistance)
                {
                    gameManager.warningController.Warning("Length variation too high, generated point was cut off");
                    break;
                }

                points.Add(finalPoint);
            }

            // Add the end point
            points.Add(endPos);

            return points;
        }

        public void GenerateMap(List<Vector3> points)
        {
            //First segement is always a platform
            List<NodeSegement> orderedSegments = new List<NodeSegement>();
            orderedSegments.Add(start);
            orderedSegments.Add(Factory.CreatePlatform(points[0], points[1]));

            //All segements inbetween are random, if elevation change, then its a platform
            bool gapLastMade = false;
            for (int i = 1; i < points.Count - 2; i++) //second to last seg is ramp, last point is exit
            {
                Vector3 startPos = points[i];
                Vector3 endPos = points[i + 1];

                Vector3 size = new Vector3(1, 1, 1); // Adjust size as needed
                orderedSegments.Add(Factory.CreateCorner(points[i], size));

                if (startPos.y != endPos.y)
                    orderedSegments.Add(Factory.CreateRamp(startPos, endPos));
                else
                {
                    if (!gapLastMade && Random.value <= GAP_CHANCE)
                    {
                        gapLastMade = true;
                        orderedSegments.Add(Factory.CreateGap(startPos, endPos));
                    }
                    else
                    {
                        gapLastMade = false;
                        orderedSegments.Add(Factory.CreatePlatform(startPos, endPos));
                    }
                }

                
            }

            //last seg always ramp
            orderedSegments.Add(Factory.CreateRamp(points[points.Count - 2], points[points.Count - 1]));
            orderedSegments.Add(end);
        }


    }
}