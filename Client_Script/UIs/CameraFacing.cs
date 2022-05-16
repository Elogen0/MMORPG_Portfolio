using System.Collections;
using System.Collections.Generic;
using Kame.Define;
using UnityEngine;

namespace Kame
{
    public class CameraFacing : MonoBehaviour
    {
        TransformAnchor referenceCamera;
        public bool reverseFace = false;

        public enum Axis
        {
            up, down, left, right, forward, back
        };

        public Axis axis = Axis.up;

        public Vector3 GetAxis(Axis refAxis)
        {
            switch (refAxis)
            {
                case Axis.down:
                    return Vector3.down;
                case Axis.forward:
                    return Vector3.forward;
                case Axis.back:
                    return Vector3.back;
                case Axis.left:
                    return Vector3.left;
                case Axis.right:
                    return Vector3.right;
            }

            return Vector3.up;
        }

        public void Awake()
        {
            if (!referenceCamera)
            {
                referenceCamera = TransformAnchor.Get<TransformAnchor>(ResourcePath.CameraTransformAnchor);
            }
        }

        private void LateUpdate()
        {
            if (!referenceCamera.isSet)
                return;
            Vector3 targetPos = transform.position + referenceCamera.Value.rotation * (reverseFace ? Vector3.forward : Vector3.back);
            Vector3 targetOrientaion = referenceCamera.Value.rotation * GetAxis(axis);

            transform.LookAt(targetPos, targetOrientaion);
        }
    }
}
