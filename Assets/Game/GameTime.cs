using UnityEngine;

namespace Game
{
    public class GameTime : MonoBehaviour
    {
        private static GameTime _instance;
        private double _time;
        private double _timeOffset = 0.0;

        // Stores the current application time duration to use as an offset the next time the game is loaded
        private double _nextTimeOffset = 0.0;

        public static GameTime Get()
        {
            return _instance;
        }

        private void Awake()
        {
            _instance = this;

            _time = 0.0;

            // In the future "_nextTimeOffset" will be loaded from a save file before the game starts
            _timeOffset += _nextTimeOffset;
            _nextTimeOffset = 0.0;
        }

        public static float Time()
        {
            var gameTime = Get();
            gameTime._nextTimeOffset = UnityEngine.Time.time;
            gameTime._time = UnityEngine.Time.time + gameTime._timeOffset;

            return (float)gameTime._time;
        }
    }
}