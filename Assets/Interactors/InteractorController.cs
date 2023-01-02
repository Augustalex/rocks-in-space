using System;
using System.Linq;
using Interactors.Digging;
using UnityEngine;

namespace Interactors
{
    public class InteractorController : MonoBehaviour
    {
        private const int DefaultModule = -1;

        private const int
            PortInteractorIndex =
                6; // You want the player to as easily as possible to the correct first action, which is to place a port.

        public GameObject defaultModuleContainer;
        public GameObject interactorsContainer;

        public event Action<InteractorModule, Block> FailedToBuild;
        public event Action UnhandledMouseUp;

        private InteractorModule _defaultModule;
        private InteractorModule[] _modules;
        private int _currentModule = PortInteractorIndex;
        private Camera _camera;

        private readonly KeyCode[] _selectKeys =
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

        private static InteractorController _instance;
        private bool _lockedToDefaultInteractor;
        private int _selectedInteractorBeforeWasLocked;

        private void Awake()
        {
            _instance = this;

            _defaultModule = defaultModuleContainer.GetComponent<InteractorModule>();
            _modules = new InteractorModule[]
            {
                interactorsContainer.GetComponent<DigInteractor>(),
                interactorsContainer.GetComponent<RefineryInteractor>(),
                interactorsContainer.GetComponent<FactoryInteractor>(),
                interactorsContainer.GetComponent<PowerPlantInteractor>(),
                interactorsContainer.GetComponent<FarmDomeInteractor>(),
                interactorsContainer.GetComponent<ResidencyInteractor>(),
                interactorsContainer.GetComponent<PortInteractor>(),
                interactorsContainer.GetComponent<ScaffoldingInteractor>(),
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
            UpdateInteractorSelection();

            // if (CurrentModule().Hoverable())
            // {
            RayCastToHover();
            // }
        }

        private void UpdateInteractorSelection()
        {
            if (_lockedToDefaultInteractor)
            {
                _currentModule = DefaultModule;
            }
            else
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
            }
        }

        public void SetInteractorByName(string interactorName)
        {
            for (var index = 0; index < _modules.Length; index++)
            {
                var interactorModule = _modules[index];
                if (interactorModule.GetInteractorName() == interactorName)
                {
                    _currentModule = index;
                    return;
                }
            }

            Debug.Log($"Tried to set interactor with name '{interactorName}' but there is no such interactor.");
        }

        private void RayCastToHover()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            var interactorModule = CurrentModule();
            if (Physics.Raycast(ray, out hit, interactorModule.MaxActivationDistance()))
            {
                if (hit.collider.CompareTag("PlanetLandmark"))
                {
                    hit.collider.GetComponent<PlanetLandmark>().Hover();
                }

                var block = hit.collider.GetComponent<Block>();
                if (block)
                {
                    var popupTarget = block.GetRoot().GetComponentInChildren<PopupTarget>();
                    if (popupTarget)
                    {
                        popupTarget.Show();
                    }
                }

                if (CurrentModule().Hoverable())
                {
                    interactorModule.Hover(hit);
                }
            }
        }

        public void Interact()
        {
            CheckForActionTarget();

            if (CurrentModule().Continuous() ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0))
            {
                RayCastToBuild();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                RayCastSecondaryAction();
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

        private void RayCastSecondaryAction()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            var interactorModule = CurrentModule();
            if (Physics.Raycast(ray, out hit, interactorModule.MaxActivationDistance()))
            {
                if (!interactorModule) return;
                var block = hit.collider.GetComponent<Block>();
                if (block != null)
                {
                    if (interactorModule)
                    {
                        interactorModule.OnSecondaryInteract(block, hit);
                    }
                }
            }
        }

        private void CheckForActionTarget()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseDown();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                HandleMouseUp();
            }
        }

        private void HandleMouseDown()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            var interactorModule = CurrentModule();
            if (Physics.Raycast(ray, out var hit, interactorModule.MaxActivationDistance()))
            {
                if (hit.collider.CompareTag("PlanetLandmark"))
                {
                    hit.collider.GetComponent<PlanetLandmark>().MouseDown();
                    return;
                }
                if (hit.collider.CompareTag("ColonyShip"))
                {
                    hit.collider.GetComponent<ColonyShip>().MouseDown();
                    return;
                }

                var parent = hit.collider.GetComponentInParent<BlockRoot>();
                if (parent)
                {
                    var actionTarget = parent.GetComponentInChildren<ActionTarget>();
                    if (actionTarget)
                    {
                        actionTarget.MouseDown();
                        return;
                    }
                }
            }
        }

        private void HandleMouseUp()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            var interactorModule = CurrentModule();
            if (Physics.Raycast(ray, out var hit, interactorModule.MaxActivationDistance()))
            {
                if (hit.collider.CompareTag("PlanetLandmark"))
                {
                    hit.collider.GetComponent<PlanetLandmark>().MouseUp();
                    return;
                }
            }

            UnhandledMouseUp?.Invoke();
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

                    if (interactorModule is DigInteractor digInteractor)
                    {
                        HandleDigInteractions(digInteractor, hit);
                    }
                    else
                    {
                        if (block != null)
                        {
                            if (interactorModule && interactorModule.CanBuild(block))
                            {
                                interactorModule.Build(block, hit);
                                interactorModule.OnBuilt(block.transform.position);
                            }
                            else
                            {
                                interactorModule.OnFailedToBuild(block.transform.position);
                                FailedToBuild?.Invoke(interactorModule, block);
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
                var block = hit.collider.GetComponent<Block>();
                if (block && !block.GetConnectedPlanet().HasPort())
                {
                    FailedToBuild?.Invoke(interactorModule, block);
                }
                else if (interactorModule.CanPerformInteraction(laserableEntity))
                {
                    if (!interactorModule.Started())
                    {
                        interactorModule.StartInteraction(laserableEntity);
                    }
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

        public void LockToDefaultInteractor()
        {
            _selectedInteractorBeforeWasLocked = _currentModule;
            _lockedToDefaultInteractor = true;
        }

        public void UnlockFromDefaultInteractor()
        {
            _currentModule = _selectedInteractorBeforeWasLocked;
            _lockedToDefaultInteractor = false;
        }
    }
}