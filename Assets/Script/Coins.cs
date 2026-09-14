using UnityEngine;

public class Coins : MonoBehaviour
{
    public int coinValue = 1;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GameManager.instance.AddCoins(coinValue);
            Destroy(gameObject);
        }
    }
}
