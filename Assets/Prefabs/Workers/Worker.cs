using UnityEngine;

public class Worker : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 2f;

    [Header("Collection")]
    [SerializeField] private float collectInterval = 2f;
    [SerializeField] private float foodCollectInterval = 5f;
    [SerializeField] private int resourcePerCollect = 1;

    private Vector3 target;
    private bool moving;
    private bool working;

    private WoodSource targetWood;
    private StoneSource targetStone;
    private FoodSource targetFood;

    private float collectTimer;

    private void Start()
    {
        target = transform.position;
    }

    private void Update()
    {
        HandleMovement();
        HandleWork();
    }

    private void HandleMovement()
    {
        if (!moving)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target) < 0.2f)
        {
            moving = false;

            if (targetWood != null ||
                targetStone != null ||
                targetFood != null)
            {
                working = true;

                // Birinchi resursni olishdan oldin kutish
                if (targetFood != null)
                    collectTimer = foodCollectInterval;
                else
                    collectTimer = collectInterval;
            }
        }
    }

    private void HandleWork()
    {
        if (!working)
            return;

        // Timer faqat vaqt o'tishi bilan kamayadi
        collectTimer -= Time.deltaTime;

        // Hali vaqt tugamagan bo'lsa hech narsa qilmaymiz
        if (collectTimer > 0f)
            return;

        // FOOD
        if (targetFood != null)
        {
            CollectFood();
            collectTimer = foodCollectInterval;
        }
        // WOOD
        else if (targetWood != null)
        {
            CollectWood();
            collectTimer = collectInterval;
        }
        // STONE
        else if (targetStone != null)
        {
            CollectStone();
            collectTimer = collectInterval;
        }
    }

    public void GoTo(Vector3 position)
    {
        StopWorking();

        target = new Vector3(
            position.x,
            transform.position.y,
            position.z
        );

        moving = true;
    }

    public void GoToWood(WoodSource wood)
    {
        if (wood == null || !wood.HasWood())
            return;

        StopWorking();

        targetWood = wood;

        target = new Vector3(
            wood.transform.position.x,
            transform.position.y,
            wood.transform.position.z
        );

        moving = true;

        Debug.Log("Worker going to wood");
    }

    public void GoToStone(StoneSource stone)
    {
        if (stone == null || !stone.HasStone())
            return;

        StopWorking();

        targetStone = stone;

        target = new Vector3(
            stone.transform.position.x,
            transform.position.y,
            stone.transform.position.z
        );

        moving = true;

        Debug.Log("Worker going to stone");
    }

    public void GoToFood(FoodSource food)
    {
        if (food == null || !food.HasFood())
            return;

        StopWorking();

        targetFood = food;

        target = new Vector3(
            food.transform.position.x,
            transform.position.y,
            food.transform.position.z
        );

        moving = true;

        Debug.Log("Worker going to farm");
    }

    private void CollectWood()
    {
        if (targetWood == null)
            return;

        int amount = targetWood.CollectWood(resourcePerCollect);

        if (amount <= 0)
        {
            StopWorking();
            return;
        }

        ResourceManager.Instance.AddWood(amount);

        Debug.Log("Wood +" + amount);
    }

    private void CollectStone()
    {
        if (targetStone == null)
            return;

        int amount = targetStone.CollectStone(resourcePerCollect);

        if (amount <= 0)
        {
            StopWorking();
            return;
        }

        ResourceManager.Instance.AddStone(amount);

        Debug.Log("Stone +" + amount);
    }

    private void CollectFood()
    {
        if (targetFood == null)
            return;

        int amount = targetFood.CollectFood(resourcePerCollect);

        if (amount <= 0)
        {
            StopWorking();
            return;
        }

        ResourceManager.Instance.AddFood(amount);

        Debug.Log("Food +" + amount);
    }

    private void StopWorking()
    {
        working = false;
        moving = false;

        targetWood = null;
        targetStone = null;
        targetFood = null;

        collectTimer = 0f;
    }
}