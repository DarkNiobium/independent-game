using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingSystem : MonoBehaviour
{
    [Header("Selected Building")]
    [SerializeField] private GameObject buildingPrefab;

    [Header("Cost")]
    [SerializeField] private int woodCost = 20;
    [SerializeField] private int stoneCost = 10;

    [Header("Grid")]
    [SerializeField] private float gridSize = 2f;

    private GameObject preview;
    private Camera mainCamera;

    private bool canBuild;

    private void Start()
    {
        mainCamera = Camera.main;
        preview = null;
    }

    private void Update()
    {
        if (preview == null)
            return;

        UpdatePreview();

        // LEFT CLICK - qurish
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Build();
        }

        // RIGHT CLICK - bekor qilish
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CancelBuild();
        }

        // ESC - bekor qilish
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CancelBuild();
        }
    }

    // UI BUTTON shu funksiyani chaqiradi
    public void SelectBuilding(GameObject building)
    {
        if (building == null)
        {
            Debug.LogError("Building is NULL!");
            return;
        }

        // TownCenter faqat 1 ta bo'lishi mumkin
        if (building.GetComponent<TownCenter>() != null &&
            TownCenter.Exists)
        {
            Debug.Log("TownCenter allaqachon qurilgan!");
            return;
        }

        buildingPrefab = building;

        // Oldingi preview bo'lsa o'chiramiz
        CancelBuild(false);

        CreatePreview();

        Debug.Log("Selected building: " + building.name);
    }

    private void CreatePreview()
    {
        if (buildingPrefab == null)
            return;

        preview = Instantiate(buildingPrefab);

        preview.name = "Building Preview";

        // Preview ichidagi barcha scriptlarni o'chiramiz
        MonoBehaviour[] scripts =
            preview.GetComponentsInChildren<MonoBehaviour>();

        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = false;
        }

        // Preview colliderlarini o'chiramiz
        Collider[] colliders =
            preview.GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }

        SetPreviewMaterial();
    }

    private void UpdatePreview()
    {
        if (mainCamera == null || Mouse.current == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        Vector3 position = SnapToGrid(hit.point);

        preview.transform.position = position;

        // Joy bo'shmi?
        canBuild = IsPositionFree(position);

        // Agar TownCenter tanlangan bo'lsa va oldin mavjud bo'lsa
        if (buildingPrefab.GetComponent<TownCenter>() != null &&
            TownCenter.Exists)
        {
            canBuild = false;
        }

        UpdatePreviewColor();
    }

    private Vector3 SnapToGrid(Vector3 position)
    {
        float x =
            Mathf.Round(position.x / gridSize) * gridSize;

        float z =
            Mathf.Round(position.z / gridSize) * gridSize;

        return new Vector3(x, 0.5f, z);
    }

    private bool IsPositionFree(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapBox(
            position,
            new Vector3(1f, 1f, 1f)
        );

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Ground"))
                continue;

            return false;
        }

        return true;
    }

    private void Build()
    {
        if (preview == null || buildingPrefab == null)
            return;

        // Joy band bo'lsa
        if (!canBuild)
        {
            Debug.Log("Bu joyda qurib bo'lmaydi!");
            return;
        }

        // TownCenter yana tekshiriladi
        if (buildingPrefab.GetComponent<TownCenter>() != null &&
            TownCenter.Exists)
        {
            Debug.Log("Faqat 1 ta TownCenter qurish mumkin!");
            CancelBuild();
            return;
        }

        if (ResourceManager.Instance == null)
        {
            Debug.LogError("ResourceManager topilmadi!");
            return;
        }

        // Resurs tekshirish
        if (ResourceManager.Instance.wood < woodCost)
        {
            Debug.Log("Wood yetarli emas!");
            return;
        }

        if (ResourceManager.Instance.stone < stoneCost)
        {
            Debug.Log("Stone yetarli emas!");
            return;
        }

        // Resurslarni kamaytirish
        ResourceManager.Instance.wood -= woodCost;
        ResourceManager.Instance.stone -= stoneCost;

        // Haqiqiy bino quriladi
        Instantiate(
            buildingPrefab,
            preview.transform.position,
            Quaternion.identity
        );

        Debug.Log("Bino qurildi!");

        // Preview o'chadi
        Destroy(preview);
        preview = null;
    }

    private void UpdatePreviewColor()
    {
        if (preview == null)
            return;

        Renderer[] renderers =
            preview.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            if (canBuild)
            {
                renderer.material.color =
                    new Color(0.3f, 1f, 0.3f);
            }
            else
            {
                renderer.material.color =
                    new Color(1f, 0.2f, 0.2f);
            }
        }
    }

    private void SetPreviewMaterial()
    {
        if (preview == null)
            return;

        Renderer[] renderers =
            preview.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material material =
                new Material(renderer.material);

            material.color =
                new Color(0.3f, 1f, 0.3f, 0.5f);

            renderer.material = material;
        }
    }

    private void CancelBuild(bool showMessage = true)
    {
        if (preview != null)
        {
            Destroy(preview);
            preview = null;
        }

        if (showMessage)
        {
            Debug.Log("Building cancelled");
        }
    }
}