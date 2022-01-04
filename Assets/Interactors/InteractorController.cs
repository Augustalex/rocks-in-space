using Interactors.Digging;
using Unity.VisualScripting;
using UnityEngine;

namespace Interactors
{
    public class InteractorController : MonoBehaviour
    {
        private const int DefaultModule = -1;

        public GameObject defaultModuleContainer;
        public GameObject interactorsContainer;

        private InteractorModule _defaultModule;
        private InteractorModule[] _modules;
        private int _currentModule = DefaultModule;
        private Camera _camera;

        private readonly KeyCode[] _selectKeys =
        {
            KeyCode.Alpha1,
            // KeyCode.Alpha2,
            // KeyCode.Alpha3,
            // KeyCode.Alpha4,
            // KeyCode.Alpha5,
            // KeyCode.Alpha6,
            // KeyCode.Alpha7,
            // KeyCode.Alpha8,
            // KeyCode.Alpha9,
        };

        private static InteractorController _instance;

        private void Awake()
        {
            _instance = this;

            _defaultModule = defaultModuleContainer.GetComponent<InteractorModule>();
            _modules = new InteractorModule[]
            {
                interactorsContainer.GetComponent<DigInteractor>()
            };
        }

        public static InteractorController Get()
        {
            return _instance;
        }

        private void Start()
        {
            _camera = GetComponent<Camera>();
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
                    if (Input.GetKeyDown(_selectKeys[i]) && _modules.Length > i)
                    {
                        _currentModule = i;
                    }
                }
            }

            if (CurrentModule().Hoverable())
            {
                RayCastToHover();
            }
        }

        private void RayCastToHover()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            var interactorModule = CurrentModule();
            if (Physics.Raycast(ray, out hit, interactorModule.MaxActivationDistance()))
            {
                interactorModule.Hover(hit);
            }
        }

        public void Interact()
        {
            if (CurrentModule().Continuous() ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0))
            {
                RayCastToBuild();
            }

            if (Input.GetMouseButtonUp(0))
            {
                var interactorModule = CurrentModule();
                if (interactorModule is DigInteractor digInteractor)
                {
                    digInteractor.StopInteraction();
                }
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
                    if (interactorModule is DigInteractor digInteractor)
                    {
                        HandleDigInteractions(digInteractor, hit);
                    }
                    else
                    {
                        var block = hit.collider.GetComponent<Block>();
                        if (block != null)
                        {
                            var planetResources = block.GetConnectedPlanet().GetResources();
                            if (interactorModule && interactorModule.CanBuild(block))
                            {
                                interactorModule.Build(block, planetResources);
                                interactorModule.OnBuilt(block.transform.position);
                            }
                            else
                            {
                                interactorModule.OnFailedToBuild(block.transform.position);
                            }
                        }
                    }

                    return;
                }
            }
        }

        private void HandleDigInteractions(DigInteractor interactorModule, RaycastHit hit)
        {
            var laserableEntity = hit.collider.GetComponent<ILaserInteractable>();
            if (laserableEntity != null)
            {
                if (interactorModule)
                {
                    if (interactorModule.CanPerformInteraction(laserableEntity))
                    {
                        if (!interactorModule.Started())
                        {
                            interactorModule.StartInteraction(laserableEntity);
                        }
                    }
                }
                else
                {
                    // interactorModule.OnFailedToBuild(block.transform.position);
                }
            }
        }

        public InteractorModule CurrentModule()
        {
            if (_currentModule == DefaultModule)
            {
                return _defaultModule;
            }
            else if (_currentModule >= 0 && _currentModule < _modules.Length)
            {
                return _modules[_currentModule];
            }
            else
            {
                return null;
            }
        }
    }
}