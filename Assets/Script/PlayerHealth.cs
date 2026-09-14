using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthBar;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth; // Sets the slider limit to 100 instead of 1
            healthBar.value = currentHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        Debug.Log("Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Player is dead!");
            if (GameManager.instance != null)
            {
                GameManager.instance.GameOver();
            }
        }
    }

    public void InstantKill()
    {
        TakeDamage(maxHealth);
    }
}