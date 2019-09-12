using UnityEngine;

namespace GameFramework.Utility
{
    public class DelayDeactive : MonoBehaviour
    {
        public float LifeTime;
        private float startTime;

        void OnEnable()
        {
            startTime = Time.time;
        }

        void Update()
        {
            if (Time.time - startTime > LifeTime)
            {
                gameObject.SetActive(false);
            }
        }
    }
}