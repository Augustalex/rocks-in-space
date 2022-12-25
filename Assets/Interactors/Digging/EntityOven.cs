using System;
using UnityEngine;

namespace Interactors.Digging
{
    public class EntityOven : MonoBehaviour
    {
        private OvenMesh _mesh;

        private void Awake()
        {
            _mesh = GetComponentInChildren<OvenMesh>();
        }

        void Start()
        {
            _mesh.gameObject.SetActive(false);
        }
        
        public void SetHeat(float factor)
        {
            if(!_mesh.gameObject.activeSelf) _mesh.gameObject.SetActive(true);
            
            var clampedFactor = Mathf.Clamp(factor, 0f, 1f);
            var material = _mesh.GetComponent<MeshRenderer>().materials[0];
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", new Color(1.0f, 0, 0, 1.0f) * clampedFactor);
        }

        public void ResetHeat()
        {
            var material = _mesh.GetComponent<MeshRenderer>().materials[0];
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", new Color(1.0f,0, 0,1.0f) * 0f);
            material.DisableKeyword("_EMISSION");
            
            _mesh.gameObject.SetActive(false);
        }
    }
}