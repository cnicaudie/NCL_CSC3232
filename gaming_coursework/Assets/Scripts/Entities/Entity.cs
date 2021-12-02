using UnityEngine;

/// <summary>
/// Base class of all entities (player + enemies)
/// Handles basic live/death behaviour
/// </summary>
public class Entity : MonoBehaviour
{
    // ===================================
    // ATTRIBUTES
    // ===================================

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

    // ===================================
    // PUBLIC METHODS
    // ===================================

    /// <summary>
    /// Damage current health and check if entity is dead
    /// </summary>
    /// <param name="damageAmount"></param>
    public virtual void Damage(float damageAmount)
    {
        Debug.Log("DAMAGE ENTITY");

        m_health.Damage(damageAmount);

        if (m_health.IsDead())
        {
            SoundManager.PlaySound("die");

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

    // ===================================
    // PROTECTED METHODS
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
}