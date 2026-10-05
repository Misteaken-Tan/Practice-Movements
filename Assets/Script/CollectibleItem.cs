using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public AudioClip pickupSound;
    public float spinSpeed = 90f;

    void Update()
    {
        transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (pickupSound != null)
            {
                // Plays clip in 3D world space so it finishes playing after the object is destroyed
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }
            Destroy(gameObject);
        }
    }
}