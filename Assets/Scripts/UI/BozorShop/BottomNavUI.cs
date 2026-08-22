using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BozorShop
{
    public class BottomNavUI : MonoBehaviour
    {
        [Header("Nav Items")]
        [SerializeField] private List<BottomNavItemUI> navItems = new List<BottomNavItemUI>();
        [SerializeField] private BottomNavSection defaultSection = BottomNavSection.Buildings;

        [Header("Section Panels (Optional)")]
        [SerializeField] private GameObject buildingsPanel;
        [SerializeField] private GameObject resourcesPanel;
        [SerializeField] private GameObject armyPanel;
        [SerializeField] private GameObject researchPanel;
        [SerializeField] private GameObject otherPanel;

        [Header("Events")]
        [SerializeField] private UnityEvent<BottomNavSection> onSectionChanged;

        public event Action<BottomNavSection> OnSectionSelected;

        public BottomNavSection CurrentSection { get; private set; }

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            foreach (var item in navItems)
            {
                if (item != null)
                {
                    item.Initialize(SelectSection);
                }
            }

            SelectSection(defaultSection);
        }

        public void SelectSection(BottomNavSection section)
        {
            CurrentSection = section;

            foreach (var item in navItems)
            {
                if (item != null)
                {
                    item.SetActive(item.Section == section);
                }
            }

            UpdatePanels(section);

            OnSectionSelected?.Invoke(section);
            onSectionChanged?.Invoke(section);

            Debug.Log($"<color=#29A39A>[BottomNav]</color> Switched to section: <b>{section}</b>");
        }

        private void UpdatePanels(BottomNavSection section)
        {
            if (buildingsPanel != null) buildingsPanel.SetActive(section == BottomNavSection.Buildings);
            if (resourcesPanel != null) resourcesPanel.SetActive(section == BottomNavSection.Resources);
            if (armyPanel != null) armyPanel.SetActive(section == BottomNavSection.Army);
            if (researchPanel != null) researchPanel.SetActive(section == BottomNavSection.Research);
            if (otherPanel != null) otherPanel.SetActive(section == BottomNavSection.Other);
        }
    }
}
