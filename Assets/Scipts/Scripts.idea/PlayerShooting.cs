using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private int poolSize = 20;
    [SerializeField] private int maxAmmo = 20;
    
    private Queue<GameObject> bulletPool = new Queue<GameObject>();
    private float nextFireTime;
    private int currentAmmo;

    private void Awake()
    {
        InitializePool();
        currentAmmo = 0; // Start with no ammo
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
        }
    }

    private void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime && currentAmmo > 0)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        if (bulletPool.Count > 0 && currentAmmo > 0)
        {
            GameObject bullet = bulletPool.Dequeue();
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation;
            bullet.SetActive(true);
            currentAmmo--;
            
            // Update UI or show ammo count
            Debug.Log($"Ammo: {currentAmmo}/{maxAmmo}");
        }
        else
        {
            Debug.LogWarning("No ammo! Cannot shoot.");
        }
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
    }

    public void CollectAmmo(int amount)
    {
        currentAmmo = Mathf.Min(currentAmmo + amount, maxAmmo);
        Debug.Log($"Collected {amount} ammo. Current ammo: {currentAmmo}/{maxAmmo}");
    }
}
