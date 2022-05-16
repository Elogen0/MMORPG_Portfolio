using System;
using System.Collections;
using System.Collections.Generic;
using Kame.Core;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Kame
{
    public class PlaceTargetPointer : MonoBehaviour
    {
        #region Variables
        public float surfaceOffset = 0.01f;
        [SerializeField] private GameObject ArrowPrefab;
        [SerializeField] private GameObject CirclePrefab;

        private Transform _target = null;

        private GameObject _arrow;
        private GameObject _circle;
        private ParticleSystem arrowParticle;
        private ParticleSystem circleParticle;
        #endregion Variables

        private void Awake()
        {
            _arrow = Instantiate(ArrowPrefab, this.transform);
            _circle = Instantiate(CirclePrefab, this.transform);
            _arrow.transform.localScale *= 3f;
            _circle.transform.localScale *= 3f;
            arrowParticle = _arrow.GetComponent<ParticleSystem>();
            circleParticle = _circle.GetComponent<ParticleSystem>();
            
            _arrow.SetActive(false);
            _circle.SetActive(true);
        }

        // Update is called once per frame
        void Update()
        {
            if (_target)
            {
                transform.position = _target.position + Vector3.up * surfaceOffset;
            }
            else
            {
                _circle.SetActive(false);
                circleParticle.Stop();
            }
        }

        public void SetTarget(Transform target)
        {
            if (target)
            {
                _circle.SetActive(true);
                circleParticle.Play();
            }
            _target = target;
        }

        public void SetPosition(RaycastHit hit)
        {
            _target = null;
            _arrow.SetActive(true);
            arrowParticle.Play();
            transform.position = hit.point + hit.normal * surfaceOffset;
        }
    }

}