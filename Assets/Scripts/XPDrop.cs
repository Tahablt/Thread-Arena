using UnityEngine;

public class XPDrop : MonoBehaviour
{
    public float xpAmount = 20f;
    public float pickupDistance = 1.5f; // Yari capi biraz daha buyutelim
    
    private bool isCollected = false;
    private Transform playerTransform;
    private PlayerXP playerXP;

    private void Start()
    {
        // Unity'nin Trigger ve Collider celiskilerini sonsuza dek atliyoruz.
        // Karakteri direkt buluyoruz.
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            playerXP = playerObj.GetComponentInChildren<PlayerXP>();
        }
    }

    private void OnEnable()
    {
        isCollected = false;
    }

    private void Update()
    {
        if (isCollected || playerTransform == null || playerXP == null) return;

        // X ve Z duzlemindeki gercek mesafeye bak (Y eksenindeki yukseklik farki hataya sebep olmasin!)
        Vector3 diff = transform.position - playerTransform.position;
        diff.y = 0; // Yuksekligi sifirla, silindir seklinde bir mesafe ölcumu!

        float distanceSqr = diff.sqrMagnitude;

        if (distanceSqr <= pickupDistance * pickupDistance)
        {
            isCollected = true;
            playerXP.AddXP(xpAmount);
            Destroy(gameObject);
        }
    }
}
