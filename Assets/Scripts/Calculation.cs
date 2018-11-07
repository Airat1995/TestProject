using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Calculation : MonoBehaviour
    {
        /// <summary>
        /// Text asset where we read input data
        /// </summary>
        [SerializeField]
        private TextAsset _inputFile;

        [SerializeField]
        [Tooltip("Speed to calculate position of the timer (kmps)")]
        private float _speed;

        [SerializeField]
        [Range(0, 100)]
        [Tooltip("Possible error, used to calculate intersection in %")]
        private float _error;

        /// <summary>
        /// list of the radii where transmitter could be
        /// </summary>
        private List<Circle> _firstReceiversCircles = new List<Circle>();
        private List<Circle> _secondReceiversCircles = new List<Circle>();
        private List<Circle> _thirdReceiversCircles = new List<Circle>();

        /// <summary>
        /// Receivers positions
        /// </summary>
        private List<Vector2> _receiversPositions = new List<Vector2>();

        /// <summary>
        /// Final position of the transmitter
        /// </summary>
        private List<Vector2> _finalTransmitterPositions = new List<Vector2>();

        /// <summary>
        /// Path to output file
        /// </summary>
        private const string PATH_TO_FILE = "Assets/Data/output.txt";

        // Use this for initialization
        void Start()
        {
            //convert from kmps to mps
            _speed = _speed * 1000;
            //convert error to %
            _error /= 100.0f;
            var lines = _inputFile.text.Split('\n');
            _receiversPositions = ParsePostionFromString(lines[0]);
            var times = lines.ToList();
            times.RemoveAt(0);
            times.RemoveAll(string.IsNullOrEmpty);
            lines = times.ToArray();
            var parsedTimes = ParseTimes(lines);

            foreach (var time in parsedTimes)
            {
                _firstReceiversCircles.Add(new Circle(_receiversPositions[0], _speed, _error, time.FirstTime));
                _secondReceiversCircles.Add(new Circle(_receiversPositions[1], _speed, _error, time.SecondTime));
                _thirdReceiversCircles.Add(new Circle(_receiversPositions[2], _speed, _error, time.ThirdTime));
            }

            //Iterate thought each measurement
            for (int i = 0; i < _firstReceiversCircles.Count; i++)
            {
                var firstPoints = _firstReceiversCircles[i].IsIntersectWithCircle(_secondReceiversCircles[i]);
                var secondPoints = _firstReceiversCircles[i].IsIntersectWithCircle(_thirdReceiversCircles[i]);
                var thirdPoints = _secondReceiversCircles[i].IsIntersectWithCircle(_thirdReceiversCircles[i]);

                //Only one working way to cut off -infinity points
                firstPoints = firstPoints.Where(point => point.x.ToString() != Mathf.NegativeInfinity.ToString()).ToArray();
                secondPoints = secondPoints.Where(point => point.x.ToString() != Mathf.NegativeInfinity.ToString())
                    .ToArray();
                thirdPoints = thirdPoints.Where(point => point.x.ToString() != Mathf.NegativeInfinity.ToString()).ToArray();

                //Calculate all available triangles between crossing points
                var distances = new List<TrianglePeremeter>();
                foreach (var firstPoint in firstPoints)
                {
                    foreach (var secondPoint in secondPoints)
                    {
                        foreach (var thirdPoint in thirdPoints)
                        {
                            var currentTriangle = new TrianglePeremeter(firstPoint, secondPoint, thirdPoint);
                            distances.Add(currentTriangle);
                        }
                    }
                }

                //Find minimal triangle
                var minDistance = distances[0];
                foreach (var trianglePeremiter in distances)
                {
                    if (trianglePeremiter.Perimeter < minDistance.Perimeter)
                        minDistance = trianglePeremiter;
                }

                _finalTransmitterPositions.Add(minDistance.FirstPoint);
            }

            WriteInfoToFile();
        }

        /// <summary>
        /// Write output file
        /// </summary>
        private void WriteInfoToFile()
        {
            var writer = new StreamWriter(PATH_TO_FILE, false);
            foreach (var finalTransmitterPosition in _finalTransmitterPositions)
            {
                writer.WriteLine($"{finalTransmitterPosition.x},{finalTransmitterPosition.y}");
            }

            writer.Close();
        }

        void OnDrawGizmos()
        {
            if (_finalTransmitterPositions.Count == 0)
                return;
            //Draw yellow line to represent path of the satellite
            Gizmos.color = Color.yellow;
            Vector2 initialPoint = _finalTransmitterPositions[0];
            foreach (var transmitterPosition in _finalTransmitterPositions)
            {
                Gizmos.DrawLine(initialPoint, transmitterPosition);
                initialPoint = transmitterPosition;
            }

            // Draw a blue sphere at the receivers position
            foreach (var receiversPosition in _receiversPositions)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(receiversPosition, .5f);
            }
        }

        /// <summary>
        /// Parse position
        /// </summary>
        /// <param name="stringInfo">input string data</param>
        /// <returns>list of position (Vector2)</returns>
        private static List<Vector2> ParsePostionFromString(string stringInfo)
        {
            var recieversPosition = new List<Vector2>();
            var position = stringInfo.Split(',');
            for (int i = 0; i < position.Length; i += 2)
            {
                float xPos, yPos;
                if (!float.TryParse(position[i], out xPos))
                    Debug.LogError($"Unable to parse X position {position[i]}");
                if (!float.TryParse(position[i + 1], out yPos))
                    Debug.LogError($"Unable to parse Y position {position[i + 1]}");

                recieversPosition.Add(new Vector2(xPos, yPos));
            }

            return recieversPosition;
        }

        /// <summary>
        /// Parse times 
        /// </summary>
        /// <param name="timeStrings">array of lines to parse 3 timers</param>
        /// <returns></returns>
        private static RecieversTime[] ParseTimes(string[] timeStrings)
        {
            RecieversTime[] times = new RecieversTime[timeStrings.Length];
            for (var index = 0; index < timeStrings.Length; index++)
            {
                var timeString = timeStrings[index];
                var rawTimeData = timeString.Split(',');
                float firstTime, secondTime, thirdTime;

                if (!float.TryParse(rawTimeData[0], out firstTime))
                    Debug.LogError($"Unable to parse X position {rawTimeData[0]}");
                if (!float.TryParse(rawTimeData[1], out secondTime))
                    Debug.LogError($"Unable to parse Y position {rawTimeData[1]}");
                if (!float.TryParse(rawTimeData[2], out thirdTime))
                    Debug.LogError($"Unable to parse Y position {rawTimeData[2]}");
                times[index] = new RecieversTime(firstTime, secondTime, thirdTime);
            }

            return times;
        }
    }
}
