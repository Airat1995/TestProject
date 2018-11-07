using System;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Find perimeter between thee crossing points
    /// </summary>
    class TrianglePeremeter
    {
        public double Perimeter { get; private set; }

        public Vector2 FirstPoint { get; set; }

        public Vector2 SecondPoint { get; set; }

        public Vector2 ThirdPoint { get; set; }

        public TrianglePeremeter(Vector2 firstPoint, Vector2 secondPoint, Vector2 thirdPoint)
        {
            FirstPoint = firstPoint;
            SecondPoint = secondPoint;
            ThirdPoint = thirdPoint;
            CalculateSize();
        }

        private void CalculateSize()
        {
            double centersFirstAndSecondDistance = Math.Sqrt(Math.Pow(Math.Abs(FirstPoint.x - SecondPoint.x), 2) +
                                                             Math.Pow(Math.Abs(FirstPoint.y - SecondPoint.y), 2));
            double centersFirstAndThirdDistance = Math.Sqrt(Math.Pow(Math.Abs(FirstPoint.x - ThirdPoint.x), 2) +
                                                            Math.Pow(Math.Abs(FirstPoint.y - ThirdPoint.y), 2));
            double centersSecondAndThirdDistance = Math.Sqrt(Math.Pow(Math.Abs(SecondPoint.x - ThirdPoint.x), 2) +
                                                             Math.Pow(Math.Abs(SecondPoint.y - ThirdPoint.y), 2));
            Perimeter = centersSecondAndThirdDistance + centersFirstAndSecondDistance + centersFirstAndThirdDistance;
        }

        /// <summary>
        /// Calculate average position between the vectors
        /// </summary>
        /// <returns></returns>
        public Vector2 AvgBetweenVectors()
        {
            float xPos = (FirstPoint.x + SecondPoint.x + ThirdPoint.x) / 3;
            float yPos = (FirstPoint.y + SecondPoint.y + ThirdPoint.y) / 3;

            return new Vector2(xPos, yPos);
        }

    }
}
