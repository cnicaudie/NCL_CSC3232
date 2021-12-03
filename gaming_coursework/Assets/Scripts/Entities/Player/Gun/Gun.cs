using UnityEngine;

/// <summary>
/// Handles Gun behaviour
/// </summary>
public class Gun : MonoBehaviour
{
    // ===================================
    // ATTRIBUTES
    // ===================================

    [SerializeField] private GameObject m_bullet;

    [SerializeField] private Transform m_shootPoint;
    
    private const int k_maxAmmunitions = 5;
    [SerializeField] private int m_ammunitions = k_maxAmmunitions;
    [SerializeField] private bool m_infiniteAmmos = false;

    private float m_shootRate = 0.75f;
    private float m_cooldownSpeed = 0f;

    private float m_range = 100f;

    // ===================================

    // ===================================
    // PRIVATE METHODS
    // ===================================

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

    private void InstantiateBullet(Vector3 hitPoint)
    {
        GameObject bullet = Instantiate(m_bullet, m_shootPoint.position, Quaternion.identity, m_bullet.transform.parent);

        bullet.SetActive(true);

        bullet.GetComponent<Bullet>().hitPoint = hitPoint;
    }

    private void Shoot()
    {
        SoundManager.Instance.PlaySound("shoot");

        m_cooldownSpeed = 0f;
        m_ammunitions -= 1;

        Vector3 hitPoint = m_shootPoint.position + m_shootPoint.forward * m_range;

        InstantiateBullet(hitPoint);
    }
}
