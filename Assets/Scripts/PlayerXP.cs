using UnityEngine;
using UnityEngine.UI;

public class PlayerXP : MonoBehaviour
{
    [Header("XP Ayarlarý")]
    public int currentLevel = 1;
    public float currentXP = 0f;
    public float xpToNextLevel = 100f;

    [Header("UI")]
    public Image xpBarImage;

    private void Start()
    {
        UpdateXPBar();
    }

    public void AddXP(float amount)
    {
        currentXP += amount;

        while (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }

        UpdateXPBar();
    }

    private void LevelUp()
    {
        currentXP -= xpToNextLevel;
        currentLevel++;
        xpToNextLevel = Mathf.Round(xpToNextLevel * 1.2f);

        Debug.Log("LEVEL UP! Yeni Seviye: " + currentLevel);
    }

    void UpdateXPBar()
    {
        if (xpBarImage != null)
        {
            xpBarImage.fillAmount = currentXP / xpToNextLevel;
        }
    }
}