using UnityEngine;
using UnityEngine.InputSystem;

public class WorkerController : MonoBehaviour
{
    private static Worker selectedWorker;

    [SerializeField] private GameObject selectionRing;

    private Worker worker;

    private void Awake()
    {
        worker = GetComponent<Worker>();

        if (selectionRing != null)
            selectionRing.SetActive(false);
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        // LEFT CLICK - Worker tanlash
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            SelectWorker();
        }

        // RIGHT CLICK - Buyruq berish
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CommandWorker();
        }

        // Ringni yangilash
        if (selectionRing != null)
        {
            selectionRing.SetActive(selectedWorker == worker);
        }
    }

    private void SelectWorker()
    {
        Ray ray = Camera.main.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Worker clickedWorker =
                hit.collider.GetComponent<Worker>();

            if (clickedWorker != null)
            {
                selectedWorker = clickedWorker;
                Debug.Log("Worker selected");
            }
        }
    }

    private void CommandWorker()
    {
        if (selectedWorker != worker)
            return;

        Ray ray = Camera.main.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            WoodSource woodSource =
                hit.collider.GetComponent<WoodSource>();

            if (woodSource != null)
            {
                worker.GoToWood(woodSource);
                return;
            }

            StoneSource stoneSource =
                hit.collider.GetComponent<StoneSource>();

            if (stoneSource != null)
            {
                worker.GoToStone(stoneSource);
                return;
            }

            FoodSource foodSource =
                hit.collider.GetComponent<FoodSource>();

            if (foodSource != null)
            {
                worker.GoToFood(foodSource);
                return;
            }
            worker.GoTo(hit.point);
        }
    }
}