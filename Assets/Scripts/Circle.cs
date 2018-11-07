using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Class used to save receivers info and check intersection with 2 circles
    /// </summary>
    class Circle
    {
        public Vector2 Center { get; }

        /// <summary>
        /// Min available radius (radius - error)
        /// </summary>
        public float MinRadius { get; }

        /// <summary>
        /// Max available radius (radius + error)
        /// </summary>
        public float MaxRadius { get; }

        public Circle(Vector2 center, float speed, float error, float time)
        {
            Center = center;
            MinRadius = speed * time * (1 - error);
            MaxRadius = speed * time * (1 + error);
        }

        /// <summary>
        /// Check is the circle intersect with <see cref="anotherCircle"/> check all opportunities (min min, min max, max min, max max)
        /// </summary>
        /// <param name="anotherCircle">circle with which intersection is checked</param>
        /// <returns>return negativeInfinity vector if circles isn't intersect</returns>
        public Vector2[] IsIntersectWithCircle(Circle anotherCircle)
        {
            List<Vector2> points = new List<Vector2>();
            points.AddRange(CrossingPoints(anotherCircle.Center, anotherCircle.MinRadius, MinRadius));
            points.AddRange(CrossingPoints(anotherCircle.Center, anotherCircle.MinRadius, MaxRadius));
            points.AddRange(CrossingPoints(anotherCircle.Center, anotherCircle.MaxRadius, MinRadius));
            points.AddRange(CrossingPoints(anotherCircle.Center, anotherCircle.MaxRadius, MaxRadius));
            return points.ToArray();
        }

        /// <summary>
        /// Calculate crossing points between two circles
        /// </summary>
        /// <param name="center">the intersected circle center</param>
        /// <param name="firstRadius">the intersected circle radius</param>
        /// <param name="secondRadius">the current circle radius</param>
        /// <returns>-infinity if points aren't crossing</returns>
        private Vector2[] CrossingPoints(Vector2 center, float firstRadius, float secondRadius)
        {
            Vector2 allPointCrossed = new Vector2();

            double centersDistance = Math.Sqrt(Math.Pow(Math.Abs(Center.x - center.x), 2) +
                                               Math.Pow(Math.Abs(Center.y - center.y), 2));
            if (centersDistance > secondRadius + firstRadius)
                return new[] { Vector2.negativeInfinity }; //circles not crossing

            double firstDistance =
                (secondRadius * secondRadius - firstRadius * firstRadius +
                 centersDistance * centersDistance) / (2 * centersDistance);
            double height = Math.Sqrt(Math.Pow(secondRadius, 2) - Math.Pow(firstDistance, 2));


            allPointCrossed.x = (float)(Center.x + firstDistance * (center.x - Center.x) / centersDistance);
            allPointCrossed.y = (float)(Center.y + firstDistance * (center.y - Center.y) / centersDistance);

            Vector2 firstCrossPoint = Vector2.negativeInfinity, secondCrossPoint = Vector2.negativeInfinity;
            firstCrossPoint.x = (float)(allPointCrossed.x + height * (center.y - Center.y) / centersDistance);
            firstCrossPoint.y = (float)(allPointCrossed.y - height * (center.x - Center.x) / centersDistance);

            if (Math.Abs(centersDistance - secondRadius) < 0.01f) return new[] { firstCrossPoint }; //circles contact

            secondCrossPoint.x = (float)(allPointCrossed.x - height * (center.y - Center.y) / centersDistance);
            secondCrossPoint.y = (float)(allPointCrossed.y + height * (center.x - Center.x) / centersDistance);

            return new[] { firstCrossPoint, secondCrossPoint };
        }
    }

}
