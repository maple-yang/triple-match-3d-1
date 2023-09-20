using System;
using Random = UnityEngine.Random;

namespace Game.Utils.Utils
{
    public class PredictableRandom : IDisposable
    {
        private readonly Random.State m_savedState;
        
        public PredictableRandom(int initSeed)
        {
            m_savedState = Random.state;
            Random.InitState(initSeed);
        }

        public void Dispose()
        {
            Random.state = m_savedState;
        }
    }
}
