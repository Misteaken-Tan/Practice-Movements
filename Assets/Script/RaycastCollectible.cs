using UnityEngine;

public class RaycastCollectible : MonoBehaviour
{
    public string itemName = "Supply Box";
    public int scoreValue = 1;

    public void Collect()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.AddCoins(scoreValue);
        }

        Destroy(gameObject);
    }
}