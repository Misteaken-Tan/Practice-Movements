using UnityEngine;

public class Lava : MonoBehaviour
{
    public int damage = 10;
    public float damageInterval = 1f;

    private float damageTimer = 0f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }

                damageTimer = 0f; // Reset interval after applying damage
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            damageTimer = 0f;
        }
    }
}