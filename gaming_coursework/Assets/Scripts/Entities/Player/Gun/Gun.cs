using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Gun behaviour (uses the Freelist design pattern)
/// </summary>
public class Gun : MonoBehaviour
{
    // ===================================
    // ATTRIBUTES
    // ===================================

    [Header("Gun Settings")]
    [SerializeField] private Transform m_shootPoint;
    private float m_shootRate = 0.75f;
    private float m_cooldownSpeed = 0f;
    private float m_range = 100f;

    [Header("Ammunitions Settings")]
    [SerializeField] private GameObject m_bulletTemplate;
    [SerializeField] private List<GameObject> m_bulletsFreelist;

    private const int k_maxAmmunitions = 5;
    [SerializeField] private int m_ammunitions = k_maxAmmunitions;
    [SerializeField] private bool m_infiniteAmmos = false;

    // ===================================

    // ===================================
    // PRIVATE METHODS
    // ===================================

    private void Start()
    {
        m_bulletsFreelist = new List<GameObject>();
    }

    private void Update()
    {
        if (GameManager.Instance.IsGamePlaying())
        {
            m_cooldownSpeed += Time.deltaTime;
    
            if (Input.GetButton("Fire1"))
            {
                if ((m_infiniteAmmos || m_ammunitions > 0) && m_cooldownSpeed > m_shootRate)
                {
                    Shoot();
                }
            }
        }
    }

    private void Shoot()
    {
        SoundManager.Instance.PlaySound("shoot");

        m_cooldownSpeed = 0f;
        m_ammunitions -= 1;

        Vector3 hitPoint = m_shootPoint.position + m_shootPoint.forward * m_range;

        InstantiateBullet(hitPoint);
    }

    /// <summary>
    /// Instantiate a new bullet using the freelist design pattern to avoid
    /// multiple unecessary creation/destruction of the bullets
    /// </summary>
    private void InstantiateBullet(Vector3 hitPoint)
    {
        bool isNewBullet = false;

        GameObject bulletGameObject;

        if (m_bulletsFreelist.Count == 0) // Create new GameObject
        {
            isNewBullet = true;
            bulletGameObject = Instantiate(m_bulletTemplate, m_shootPoint.position, Quaternion.identity, m_bulletTemplate.transform.parent);
        }
        else // Use already created GameObject
        {
            isNewBullet = false;

            // Get last item of the freelist and remove it
            int bulletIndex = m_bulletsFreelist.Count - 1;
            bulletGameObject = m_bulletsFreelist[bulletIndex];
            m_bulletsFreelist.Remove(bulletGameObject);

            // Reset its position and rotation
            bulletGameObject.transform.position = m_shootPoint.position;
            bulletGameObject.transform.rotation = Quaternion.identity;
        }

        bulletGameObject.SetActive(true);

        // Set up the bullet
        Bullet bullet = bulletGameObject.GetComponent<Bullet>();
        bullet.Reset(hitPoint, isNewBullet);
        bullet.DestroyBullet += AddBulletToFreelist;
    }

    /// <summary>
    /// Add the bullet object to the freelist
    /// </summary>
    private void AddBulletToFreelist(GameObject bulletGameObject)
    {
        bulletGameObject.SetActive(false);
        m_bulletsFreelist.Add(bulletGameObject);
    }
}
