using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CityBuilderCore
{
    /// <summary>
    /// selects walkers and buildings under the mouse on click
    /// </summary>
    /// <remarks><see href="https://citybuilder.softleitner.com/manual">https://citybuilder.softleitner.com/manual</see></remarks>
    [HelpURL("https://citybuilderapi.softleitner.com/class_city_builder_core_1_1_selection_tool.html")]
    public class SelectionTool : BaseTool
    {
        [Tooltip("fired when a building is clicked, use to show building dialogs and such")]
        public BuildingEvent BuildingSelected;
        [Tooltip("fired when a walker is clicked, use to show walker dialogs and such")]
        public WalkerEvent WalkerSelected;
        [Tooltip("fired when a click occured but not building or walker was found")]
        public Vector2IntEvent PointSelected;
        [Tooltip("color used to highlight what the pointer hovers over(set alpha to 0 to deactivate)")]
        public Color HighlightColor = Color.clear;
        [Tooltip("added to buildings when the mouse hovers over them")]
        public BuildingAddon BuildingAddon;
        [Tooltip("added to walkers when the mouse hovers over them")]
        public WalkerAddon WalkerAddon;

        public override bool ShowGrid => false;
        public override bool IsTouchPanAllowed => true;
        public bool IsHighlighting => HighlightColor.a > 0f;
        public bool IsHovering => IsHighlighting || BuildingAddon || WalkerAddon;

        private float _mouseDown;
        private IMouseInput _mouseInput;
        private IHighlightManager _highlighting;
        private IBuilding _currentAddonBuilding;
        private Walker _currentAddonWalker;

        private void Start()
        {
            _mouseInput = Dependencies.Get<IMouseInput>();
            if (IsHighlighting)
                _highlighting = Dependencies.Get<IHighlightManager>();
        }

        protected override void updateTool()
        {
            base.updateTool();

            if (IsHighlighting)
                _highlighting.Clear();

            if (EventSystem.current.IsPointerOverGameObject())
            {
                setWalkerAddon(null);
                setBuildingAddon(null);
                return;
            }

            if (!_mouseInput.TryGetMouseGridPosition(out Vector2Int mousePoint))
                return;

            if (Input.GetMouseButtonDown(0))
                _mouseDown = Time.unscaledTime;

            var clicked = Input.GetMouseButtonUp(0) && (Time.unscaledTime - _mouseDown) < 0.2f;
            if (clicked)
                onApplied();

            if (!IsHovering && !clicked)
                return;

            var walker = getWalker();
            if (walker)
            {
                setWalkerAddon(walker);
                setBuildingAddon(null);

                if (IsHighlighting)
                    _highlighting.Highlight(walker.GridPoint, HighlightColor);
                if (clicked)
                    WalkerSelected?.Invoke(walker);
                return;
            }

            var building = getBuilding(mousePoint);
            if (building != null)
            {
                setWalkerAddon(null);
                setBuildingAddon(building);

                if (IsHighlighting)
                    _highlighting.Highlight(building.GetPoints(), HighlightColor);
                if (clicked)
                    BuildingSelected?.Invoke(building.BuildingReference);
                return;
            }

            setWalkerAddon(null);
            setBuildingAddon(null);

            if (IsHighlighting)
                _highlighting.Highlight(mousePoint, HighlightColor);

            if (clicked)
                PointSelected?.Invoke(mousePoint);
        }

        private void setWalkerAddon(Walker walker)
        {
            if (!WalkerAddon)
                return;

            if (_currentAddonWalker == walker)
                return;

            if (_currentAddonWalker != null)
                _currentAddonWalker.RemoveAddon(WalkerAddon.Key);
            _currentAddonWalker = walker;
            if (_currentAddonWalker != null)
                _currentAddonWalker.AddAddon(WalkerAddon);
        }
        private void setBuildingAddon(IBuilding building)
        {
            if (!BuildingAddon)
                return;

            if (_currentAddonBuilding == building)
                return;

            if (_currentAddonBuilding != null)
                _currentAddonBuilding.RemoveAddon(BuildingAddon.Key);
            _currentAddonBuilding = building;
            if (_currentAddonBuilding != null)
                _currentAddonBuilding.AddAddon(BuildingAddon);
        }

        private Walker getWalker()
        {
            var ray = _mouseInput.GetRay();
            if (ray.IsInvalid())
                return null;

            var walkerObject = Physics.RaycastAll(ray).Select(h => h.transform.gameObject).FirstOrDefault(g => g.CompareTag("Walker"));
            if (!walkerObject)
                return null;

            var walker = walkerObject.GetComponent<Walker>();
            if (!walker)
                walker = walkerObject.GetComponentInParent<Walker>();

            return walker;
        }

        private IBuilding getBuilding(Vector2Int point)
        {
            return Dependencies.Get<IBuildingManager>().GetBuilding(point).FirstOrDefault();
        }
    }
}