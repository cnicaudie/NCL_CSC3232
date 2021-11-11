using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private GameObject m_bullet;

    [SerializeField] private Transform m_shootPoint;
    [SerializeField] private Transform m_bulletsParent;

    private const int k_maxAmmunitions = 5;
    [SerializeField] private int m_ammunitions = k_maxAmmunitions;
    [SerializeField] private bool m_infiniteAmmos = false;

    private float m_shootRate = 0.75f;
    private float m_cooldownSpeed = 0f;

    private float m_range = 100f;

    //public delegate void UpdateBulletValue();
    //public event UpdateBulletValue OnBulletNumberChange;

    // ===================================

    private void Update()
    {
        if (GameManager.IsGamePlaying())
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

    public int GetAmmunitionCount()
    {
        return m_ammunitions;
    }

    private void AddAmmunition()
    {
        if (m_ammunitions < k_maxAmmunitions)
        {
            m_ammunitions += 1;

            /*if (OnBulletNumberChange != null)
            {
                OnBulletNumberChange();
            }*/
        }
    }

    private void InstantiateBullet(Vector3 hitPoint)
    {
        GameObject bullet = Instantiate(m_bullet, m_shootPoint.position, Quaternion.identity, m_bulletsParent);

        bullet.SetActive(true);

        bullet.GetComponent<Bullet>().hitPoint = hitPoint;
    }

    private void Shoot()
    {
        m_cooldownSpeed = 0f;
        m_ammunitions -= 1;

        Vector3 hitPoint = m_shootPoint.position + m_shootPoint.forward * m_range;

        InstantiateBullet(hitPoint);

        /*if (OnBulletNumberChange != null)
        {
            OnBulletNumberChange();
        }*/
    }
}
