using UnityEngine;

namespace BozorShop
{
    public enum BuildingCategory
    {
        All,             // Barchasi
        Production,      // Ishlab chiqarish
        Agriculture,     // Qishloq xo'jaligi
        Trade,           // Savdo
        Decorations      // Bezaklar
    }

    [CreateAssetMenu(fileName = "NewBuilding", menuName = "Bozor/Building Data")]
    public class BuildingDataSO : ScriptableObject
    {
        [Header("General Info")]
        public string id;
        public string displayName;
        public BuildingCategory category = BuildingCategory.Production;
        public Sprite buildingPreview;

        [Header("Production Stats")]
        public Sprite resourceIcon;
        public int productionRate = 15;
        public string rateUnit = "/ soat";

        [Header("Capacity Stats")]
        public Sprite capacityIcon;
        public int capacity = 60;

        [Header("Cost")]
        public int priceGold = 500000;
        public GameObject prefabToBuild;
    }
}
