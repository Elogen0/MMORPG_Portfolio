using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Kame
{
    public class AutoDestroyEffect : MonoBehaviour
    {
        private ParticleSystem particle;

        // Start is called before the first frame update
        void Start()
        {
            particle = GetComponent<ParticleSystem>();
        }

        // Update is called once per frame
        void Update()
        {
            if (particle)
            {
                if (!particle.IsAlive())
                {
                    AddressableLoader.ReleaseInstance(gameObject);
                }
            }
        }

        public void OnEnable()
        {
            if (particle)
                particle.Play();
        }

        public void OnDisable()
        {
            if (particle)
                particle.Stop();
        }
    }
}