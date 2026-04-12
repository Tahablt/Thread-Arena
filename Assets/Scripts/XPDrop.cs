using UnityEngine;

public class XPDrop : MonoBehaviour
{
    public float xpAmount = 20f;

    private void OnTriggerEnter(Collider other)
    {
        // Temas eden objeyi konsola yazdýrýyoruz (hata ayýklamak için)
        Debug.Log("Çarptýðým þeyin adý: " + other.name);

        if (other.CompareTag("Player"))
        {
            PlayerXP playerXP = other.GetComponent<PlayerXP>();

            if (playerXP != null)
            {
                playerXP.AddXP(xpAmount);
                Debug.Log("XP toplandý: " + xpAmount);

                // Küreyi yok et
                Destroy(gameObject);
            }
        }
    }
}