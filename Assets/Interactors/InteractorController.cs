using System;
using System.Linq;
using Interactors.Digging;
using UnityEngine;

namespace Interactors
{
    public class InteractorController : MonoBehaviour
    {
        private const int DefaultModule = -1;

        public GameObject defaultModuleContainer;
        public GameObject interactorsContainer;

        public event Action<InteractorModule> InteractorSelected;
        public event Action<InteractorModule, Block> FailedToBuild;
        public event Action UnhandledMouseUp;

        private InteractorModule _defaultModule;
        private InteractorModule[] _modules;
        private int _currentModule = -1;
        private Camera _camera;

        private readonly KeyCode[] _selectKeys =
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            // KeyCode.Alpha3,
            // KeyCode.Alpha4,
            // KeyCode.Alpha5,
            // KeyCode.Alpha6,
            // KeyCode.Alpha7,
            // KeyCode.Alpha8,
            // KeyCode.Alpha9,
        };

        private static InteractorController _instance;
        private bool _lockedToDefaultInteractor;
        private int _selectedInteractorBeforeWasLocked = -1;
        private int _previousInteractor = -1;
        private CameraController _cameraController;

        private BuildingType[] _generalBuildings;

        public static BuildingType[] GeneralBuildings() => Get()._generalBuildings;

        private GeneralBuildingInteractor[] _generalBuildingInteractors;

        public static InteractorController Get()
        {
            return _instance;
        }

        private void Awake()
        {
            _instance = this;

            _defaultModule = defaultModuleContainer.GetComponent<InteractorModule>();

            _generalBuildingInteractors = interactorsContainer.GetComponentsInChildren<GeneralBuildingInteractor>();
            _generalBuildings = _generalBuildingInteractors.Select(g => g.GetBuildingType()).ToArray();

            _modules = new[]
            {
                interactorsContainer.GetComponent<DigInteractor>(),
                interactorsContainer.GetComponent<PortInteractor>(),
                interactorsContainer.GetComponent<RefineryInteractor>(),
                interactorsContainer.GetComponent<FactoryInteractor>(),
                interactorsContainer.GetComponent<PowerPlantInteractor>(),
                interactorsContainer.GetComponent<FarmDomeInteractor>(),
                interactorsContainer.GetComponent<ResidencyInteractor>(),
                interactorsContainer.GetComponent<ScaffoldingInteractor>(),
                interactorsContainer.GetComponent<KorvKioskInteractor>(),
                _defaultModule
            }.Concat(_generalBuildingInteractors).ToArray();
        }

        private void Start()
        {
            _camera = GetComponent<Camera>();

            _cameraController = CameraController.Get();
            _cameraController.OnToggleZoom += (b) => UpdateInteractorLock();
            _cameraController.OnNavigationStarted += UpdateInteractorLock;

            var currentPlanetController = CurrentPlanetController.Get();
            currentPlanetController.CurrentPlanetChanged += (i) => UpdateInteractorLock();
            currentPlanetController.ShipSelected += (i) => UpdateInteractorLock();

            SetInteractorByInteractorType(InteractorType.Port);
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
                _selectedInteractorBeforeWasLocked = DefaultModule;
                _currentModule = DefaultModule;
            }
            else
            {
                // Uncomment to enable keyboard shortcuts:
                // Note however - these have no support for progress locks. So that needs to be fixed first.
                for (var i = 0; i < _selectKeys.Length; i++)
                {
                    if (Input.GetKeyDown(_selectKeys[i]) && _modules.Length > i)
                    {
                        _selectedInteractorBeforeWasLocked = i;
                        _currentModule = i;
                    }
                }
            }

            if (_currentModule != _previousInteractor) InteractorSelected?.Invoke(CurrentModule());
            _previousInteractor = _currentModule;
        }

        public void SetInteractorByName(string interactorName)
        {
            for (var index = 0; index < _modules.Length; index++)
            {
                var interactorModule = _modules[index];
                if (interactorModule.GetInteractorName() == interactorName)
                {
                    _selectedInteractorBeforeWasLocked = index;
                    _currentModule = index;
                    return;
                }
            }

            Debug.Log($"Tried to set interactor with name '{interactorName}' but there is no such interactor.");
        }

        public void SetInteractorByInteractorType(InteractorType interactorType)
        {
            for (var index = 0; index < _modules.Length; index++)
            {
                var interactorModule = _modules[index];
                if (interactorModule.GetInteractorType() == interactorType)
                {
                    _selectedInteractorBeforeWasLocked = index;
                    _currentModule = index;
                    return;
                }
            }

            Debug.Log($"Tried to set interactor with type '{interactorType}' but there is no such interactor.");
        }

        public void SetInteractorByBuildingType(BuildingType buildingType)
        {
            if (!_generalBuildings.Contains(buildingType))
                throw new Exception("Cannot select buildings by type that are not in the list of General Buildings.");

            var buildingInteractor = _generalBuildingInteractors.First(b => b.GetBuildingType() == buildingType);
            var index = Array.IndexOf(_modules, buildingInteractor);
            _selectedInteractorBeforeWasLocked = index;
            _currentModule = index;
        }

        public InteractorModule GetGenericInteractorByBuildingType(BuildingType buildingType)
        {
            if (!_generalBuildings.Contains(buildingType))
            {
                return GetInteractor(FromBuildingType(buildingType));
            }

            try
            {
                return _generalBuildingInteractors.First(b => b.GetBuildingType() == buildingType);
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"Could not find building: {buildingType}, in the list of general building interactors.");
            }
        }

        public GeneralBuildingInteractor GetSpecificallyGeneralBuildingInteractor(BuildingType buildingType)
        {
            if (!_generalBuildings.Contains(buildingType))
            {
                throw new Exception("Cannot get buildings by type that are not in the list of General Buildings.");
            }

            try
            {
                return _generalBuildingInteractors.First(b => b.GetBuildingType() == buildingType);
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"Could not find building: {buildingType}, in the list of general building interactors.");
            }
        }

        public static InteractorType FromBuildingType(BuildingType buildingType)
        {
            // General buildings should NOT be in here
            return buildingType switch
            {
                BuildingType.Port => InteractorType.Port,
                BuildingType.Refinery => InteractorType.Refinery,
                BuildingType.Factory => InteractorType.Factory,
                BuildingType.PowerPlant => InteractorType.PowerPlant,
                BuildingType.FarmDome => InteractorType.FarmDome,
                BuildingType.ResidentModule => InteractorType.ResidentModule,
                BuildingType.Platform => InteractorType.Platform,
                BuildingType.KorvKiosk => InteractorType.KorvKiosk,
                _ => throw new ArgumentOutOfRangeException(nameof(buildingType), buildingType, null)
            };
        }

        public bool CurrentInteractorIsGeneralBuilding()
        {
            return _generalBuildingInteractors.Contains(CurrentModule());
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

                // Comment out to enable Planet Info popup
                // if (_cameraController
                //     .IsZoomedOut()) // Currently not using planet popup when zoomed in. Since it has been replaced by the planet info panel.
                // {
                //     var block = hit.collider.GetComponent<Block>();
                //     if (block)
                //     {
                //         var popupTarget = block.GetRoot().GetComponentInChildren<PopupTarget>();
                //         if (popupTarget)
                //         {
                //             if (ErrorDisplay.Get().IsVisible()) ErrorDisplay.Get().FadeOut();
                //             popupTarget.Show();
                //         }
                //     }
                // }

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
                    AudioController.Get().Cancel();
                    digInteractor.StopInteraction();
                }
            }
        }

        private void RayCastSecondaryAction()
        {
            return;

            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            var interactorModule = CurrentModule();
            if (Physics.Raycast(ray, out hit, interactorModule.MaxActivationDistance()))
            {
                if (interactorModule == null) return;
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

            if (Input.GetMouseButtonDown(1))
            {
                HandleRightMouseDown();
            }
            else if (Input.GetMouseButtonUp(1))
            {
                HandleRightMouseUp();
            }
        }

        private void HandleMouseDown()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            var interactorModule = CurrentModule();
            if (Physics.Raycast(ray, out var hit, 10000f))
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

                if (hit.collider.CompareTag("TradeLine"))
                {
                    hit.collider.GetComponentInParent<RouteLine>().EditRoute();
                    return;
                }

                if (hit.distance <= interactorModule.MaxActivationDistance())
                {
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
        }

        private void HandleMouseUp()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, 10000f))
            {
                if (hit.collider.CompareTag("PlanetLandmark"))
                {
                    hit.collider.GetComponent<PlanetLandmark>().MouseUp();
                    return;
                }
            }

            UnhandledMouseUp?.Invoke();
        }

        private void HandleRightMouseDown()
        {
            // Do nothing
        }

        private void HandleRightMouseUp()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, 10000f))
            {
                if (hit.collider.CompareTag("TradeLine"))
                {
                    // todo: remove?
                    // hit.collider.GetComponentInParent<RouteLine>().RemoveLine();
                    return;
                }
            }
        }

        private void RayCastToBuild()
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
            }
        }

        private void HandleDigInteractions(DigInteractor interactorModule, RaycastHit hit)
        {
            var laserableEntity = hit.collider.GetComponent<ILaserInteractable>();
            if (laserableEntity != null)
            {
                var block = hit.collider.GetComponent<Block>();
                if (!block.Exists()) return;

                var connectedPlanet = block.GetConnectedPlanet();
                if (!connectedPlanet.HasPort())
                {
                    FailedToBuild?.Invoke(interactorModule, block);
                }
                else if (interactorModule.IsCooledDown())
                {
                    if (interactorModule.CanPerformInteraction(laserableEntity))
                    {
                        if (!interactorModule.Started())
                        {
                            interactorModule.StartInteraction(laserableEntity);
                        }
                    }
                    else
                    {
                        FailedToBuild?.Invoke(interactorModule, block);
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

        private void UpdateInteractorLock()
        {
            if (CameraController.Get().IsZoomedOut())
            {
                _lockedToDefaultInteractor = true;
            }
            else
            {
                _currentModule = _selectedInteractorBeforeWasLocked;
                _lockedToDefaultInteractor = false;
            }
        }

        public InteractorModule GetInteractor(InteractorType interactorType)
        {
            return _modules.First(m => m.GetInteractorType() == interactorType);
        }
    }
}