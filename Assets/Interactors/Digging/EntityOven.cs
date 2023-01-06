using System;
using UnityEngine;

namespace Interactors.Digging
{
    public class EntityOven : MonoBehaviour
    {
        private OvenMesh _mesh;
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private Material _material;

        private void Awake()
        {
            _mesh = GetComponentInChildren<OvenMesh>();
            _material = _mesh.GetComponent<MeshRenderer>().materials[0];
        }

        void Start()
        {
            _mesh.gameObject.SetActive(false);
        }

        public void SetHeat(float factor)
        {
            if (!_mesh.gameObject.activeSelf) _mesh.gameObject.SetActive(true);

            var clampedFactor = Mathf.Clamp(factor, 0f, 1f);
            _material.SetColor(BaseColor, new Color(1.0f, 0, 0, clampedFactor * .5f));
        }

        public void ResetHeat()
        {
            _material.SetColor(BaseColor, new Color(1.0f, 0, 0, 0.0f));

            _mesh.gameObject.SetActive(false);
        }
    }
}