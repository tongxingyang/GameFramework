using UnityEngine;

namespace GameFramework.Utility
{
    public class DelayDestory : MonoBehaviour
    {
        public float LifeTime;
        private float startTime;

        void Start()
        {
            startTime = Time.time;
        }

        void Update()
        {
            if (Time.time - startTime > LifeTime)
            {
                Destroy(gameObject);
            }
        }
    }
}