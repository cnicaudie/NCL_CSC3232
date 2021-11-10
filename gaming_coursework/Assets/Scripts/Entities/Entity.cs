using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] protected Health m_health;

    protected bool m_isDead = false;
    public bool IsDead
    {
        get { return m_isDead; }
    }

    [SerializeField] protected float m_actionRange = 7f;

    [SerializeField] protected bool m_isDamageable = true;
    public bool IsDamageable
    {
        get { return m_isDamageable; }
    }

    [SerializeField] protected float m_damageCooldown = 1.5f;
    protected float m_lastDamageTime;

    public delegate void OnEntityDeath();
    public event OnEntityDeath EntityDie;

    // ===================================

    protected virtual void Start()
    {
        m_health = GetComponent<Health>();
        m_lastDamageTime = m_damageCooldown;
    }

    protected virtual void Update()
    {
        m_lastDamageTime += Time.deltaTime;

        if (m_lastDamageTime >= m_damageCooldown)
        {
            m_isDamageable = true;
        }
    }

    public virtual void Damage(float damageAmount)
    {
        Debug.Log("DAMAGE ENTITY");
        
        m_health.Damage(damageAmount);

        if (m_health.IsDead())
        {
            m_isDead = true;

            Debug.Log("Entity " + gameObject.name + " died !");

            if (EntityDie != null)
            {
                EntityDie();
            }

            Destroy(gameObject, 5f);
        }

        m_lastDamageTime = 0f;
        m_isDamageable = false;
    }
}