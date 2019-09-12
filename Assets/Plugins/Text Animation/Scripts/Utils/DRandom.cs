using UnityEngine;
using System.Collections;

namespace I2.TextAnimation
{
    public static class DRandom
    {
        static float[] mValues = null;

        const int NUM_VALUES = 500;

        public static int mCurrentSeed = 0;

        public static float Get01(int index)
        {
            if (mValues == null)
            {
                mValues = new float[NUM_VALUES];
                for (int i = 0; i < NUM_VALUES; ++i)
                    mValues[i] = UnityEngine.Random.value;
            }

            index = (index + mCurrentSeed) % NUM_VALUES;
            return mValues[index];
        }

        public static float GetUnit(int index)
        {
            if (mValues == null)
            {
                mValues = new float[NUM_VALUES];
                for (int i = 0; i < NUM_VALUES; ++i)
                    mValues[i] = UnityEngine.Random.value;
            }

            index = (index + mCurrentSeed) % NUM_VALUES;
            return mValues[index]*2-1;
        }


        public static float Get(int index, float min, float max)
        {
            return min + Get01(index) * (max - min);
        }


        public static int GetSeed()
        {
            return Random.Range(0, NUM_VALUES);
        }

    }
}