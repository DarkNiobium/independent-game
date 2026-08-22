using UnityEngine;
using UnityEngine.UI;

namespace BozorShop
{
    public class BottomNavUI : MonoBehaviour
    {
        [SerializeField] private Button resurslarBtn;
        [SerializeField] private Button binolarBtn;
        [SerializeField] private Button armiyaBtn;
        [SerializeField] private Button tadqiqotBtn;
        [SerializeField] private Button boshqaBtn;

        private void Start()
        {
            if (resurslarBtn != null) resurslarBtn.onClick.AddListener(() => Debug.Log("[Nav] Resurslar clicked"));
            if (binolarBtn != null) binolarBtn.onClick.AddListener(() => Debug.Log("[Nav] Binolar clicked"));
            if (armiyaBtn != null) armiyaBtn.onClick.AddListener(() => Debug.Log("[Nav] Armiya clicked"));
            if (tadqiqotBtn != null) tadqiqotBtn.onClick.AddListener(() => Debug.Log("[Nav] Tadqiqot clicked"));
            if (boshqaBtn != null) boshqaBtn.onClick.AddListener(() => Debug.Log("[Nav] Boshqa clicked"));
        }
    }
}
