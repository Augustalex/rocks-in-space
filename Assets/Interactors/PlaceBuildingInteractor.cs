using System;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

namespace Interactors
{
    public class PlaceBuildingInteractor : MonoBehaviour
    {
        public InteractorModule[] modules;
        private int _currentModule = -1;
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

        private static PlaceBuildingInteractor _instance;

        private void Awake()
        {
            _instance = this;
        }

        public static PlaceBuildingInteractor Get()
        {
            return _instance;
        }
        
        private void Start()
        {
            _camera = GetComponent<Camera>();
            _audioController = AudioController.Get();
        }

        private void Update()
        {
            for (var i = 0; i < _selectKeys.Length; i++)
            {
                if (Input.GetKeyDown(_selectKeys[i]) && modules.Length > i)
                {
                    _currentModule = _currentModule == i ? -1 : i;
                }
            }
        }

        public bool Loaded()
        {
            return _currentModule != -1;
        }

        public void Interact()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RayCastToBuild();
            }
        }

        private void RayCastToBuild()
        {
            for (int i = 0; i < 100; i++)
            {
                var ray = _camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 60))
                {
                    var block = hit.collider.GetComponent<Block>();
                    if (block != null)
                    {
                        var planetResources = block.GetConnectedPlanet().GetResources();
                        var interactorModule = CurrentModule();
                        if (interactorModule && interactorModule.CanBuild(block, planetResources))
                        {
                            _audioController.Play(_audioController.destroyBlock, _audioController.destroyBlockVolume,
                                block.transform.position);

                            interactorModule.Build(block, planetResources);
                        }
                        else
                        {
                            _audioController.Play(_audioController.cannotBuild, _audioController.cannotBuildVolume,
                                block.transform.position);
                        }
                    }
                    
                    return;
                }   
            }
        }

        public InteractorModule CurrentModule()
        {
            if (_currentModule < 0 || _currentModule >= modules.Length) return null;

            return modules[_currentModule];
        }
    }
}