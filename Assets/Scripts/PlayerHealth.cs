using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Ayarlar")]
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    [Header("UI")]
    public Image healthBarImage;
    public GameObject gameOverPanel;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();

        if (currentHealth <= 0) Die();
    }

    void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = currentHealth / maxHealth;
        }
    }

    private void Die()
    {
        isDead = true;
        Time.timeScale = 0;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}