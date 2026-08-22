using UnityEngine;

public class TownCenter : MonoBehaviour
{
    public static bool Exists { get; private set; }

    private void Awake()
    {
        if (Exists)
        {
            Destroy(gameObject);
            return;
        }

        Exists = true;
    }

    private void OnDestroy()
    {
        Exists = false;
    }
}