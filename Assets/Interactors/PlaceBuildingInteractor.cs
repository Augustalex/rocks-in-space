using System;
using UnityEngine;

namespace Interactors
{
    public class PlaceBuildingInteractor : MonoBehaviour
    {
        public GameObject[] templates;
        private int _currentTemplate = -1;
        private AudioController _audioController;
        private Camera _camera;

        private KeyCode[] _selectKeys = new[]
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9,
        };
        
        private void Start()
        {
            _camera = GetComponent<Camera>();
            _audioController = AudioController.Get();
        }

        private void Update()
        {
            for (var i = 0; i < _selectKeys.Length; i++)
            {
                if (Input.GetKeyDown(_selectKeys[i]) && templates.Length > i)
                {
                    _currentTemplate = _currentTemplate == i ? -1 : i;
                }
            }
        }

        public bool Loaded()
        {
            return _currentTemplate != -1;
        }

        public void Interact()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = _camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 60))
                {
                    var block = hit.collider.GetComponent<Block>();
                    if (block != null)
                    {
                        _audioController.Play(_audioController.destroyBlock, _audioController.destroyBlockVolume,
                            block.transform.position);

                        var seed = CurrentTemplate();
                        if (seed)
                        {
                            block.Seed(seed);
                        }
                    }
                }
            }
        }

        private GameObject CurrentTemplate()
        {
            if (_currentTemplate < 0 || _currentTemplate >= templates.Length) return null;

            return templates[_currentTemplate];
        }
    }
}