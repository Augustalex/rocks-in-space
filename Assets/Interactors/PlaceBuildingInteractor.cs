using UnityEngine;

namespace Interactors
{
    public class PlaceBuildingInteractor : MonoBehaviour
    {
        private const int DefaultModule = 0;

        public InteractorModule defaultModule;

        public InteractorModule[] modules;
        private int _currentModule = DefaultModule;
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
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _currentModule = DefaultModule;
            }
            else
            {
                for (var i = 0; i < _selectKeys.Length; i++)
                {
                    if (Input.GetKeyDown(_selectKeys[i]) && modules.Length > i)
                    {
                        _currentModule = _currentModule == i ? DefaultModule : i;
                    }
                }
            }
        }

        public void Interact()
        {
            if (CurrentModule().Continuous() ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0))
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

                var interactorModule = CurrentModule();
                if (Physics.Raycast(ray, out hit, interactorModule.MaxActivationDistance()))
                {
                    var block = hit.collider.GetComponent<Block>();
                    if (block != null)
                    {
                        var planetResources = block.GetConnectedPlanet().GetResources();
                        if (interactorModule && interactorModule.CanBuild(block, planetResources))
                        {
                            interactorModule.Build(block, planetResources);
                            interactorModule.OnBuilt(block.transform.position);
                        }
                        else
                        {
                            interactorModule.OnFailedToBuild(block.transform.position);
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