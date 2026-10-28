using UnityEngine;

public class PlayerFPSGun : MonoBehaviour
{
    [Header("Camera & Muzzle")]
    public Camera fpsCamera;
    public Transform muzzlePoint;

    [Header("Gun Mechanics")]
    public float fireRate = 0.2f;
    public float weaponRange = 100f;
    public int damagePerShot = 1;
    private float nextFireTime;

    [Header("Collectible Interaction")]
    public float interactionDistance = 3.5f;
    public LayerMask interactionMask = ~0;

    [Header("Visual Effects (Requirements 5 & 6)")]
    public ParticleSystem muzzleFlash;
    public GameObject impactEffectPrefab;

    [Header("Audio (Requirement 7)")]
    public AudioSource audioSource;
    public AudioClip gunshotSound;
    public AudioClip collectibleSound;

    void Start()
    {
        if (fpsCamera == null)
            fpsCamera = Camera.main;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Shooting
        if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }

        // Collectible Interaction
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteractOrCollect();
        }
    }

    void Shoot()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        if (audioSource != null && gunshotSound != null)
        {
            audioSource.PlayOneShot(gunshotSound);
        }

        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, weaponRange))
        {
            Target target = hit.collider.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damagePerShot);
            }

            if (impactEffectPrefab != null)
            {
                GameObject impact = Instantiate(impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 1.5f);
            }
        }
    }

    void TryInteractOrCollect()
    {
        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactionMask))
        {
            RaycastCollectible collectible = hit.collider.GetComponent<RaycastCollectible>();
            if (collectible != null)
            {
                if (audioSource != null && collectibleSound != null)
                {
                    audioSource.PlayOneShot(collectibleSound);
                }

                collectible.Collect();
            }
        }
    }
}