using UnityEngine;

/// <summary>
/// Handles player behaviour
/// </summary>
public class Player : Entity
{
    // ===================================
    // ATTRIBUTES
    // ===================================

    private Animator m_animator;

    // Dynamic change of physic material properties
    [SerializeField] private PhysicMaterial m_defaultMaterial;
    [SerializeField] private PhysicMaterial m_injuredMaterial;
    private CapsuleCollider m_collider;

    // Dynamic change of mass
    private float m_defaultMass = 50f;
    private float m_injuredMass = 150f;
    private Rigidbody m_rigidbody;

    private float m_animationSpeedMultiplier = 1f;

    // ===================================

    // ===================================
    // PROTECTED METHODS
    // ===================================

    protected override void Start()
    {
        base.Start();

        m_animator = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_collider = GetComponent<CapsuleCollider>();
        m_animator.SetFloat("AnimationSpeed", m_animationSpeedMultiplier);
    }

    protected override void Update()
    {
        if (GameManager.IsGamePlaying())
        {
            base.Update();

            // If the player is damaged, its physic material properties change
            // for a short amount of time (injured malus)
            if (m_isDamageable)
            {
                m_collider.material = m_defaultMaterial;
                m_rigidbody.mass = m_defaultMass;
                m_animator.SetFloat("AnimationSpeed", m_animationSpeedMultiplier);
            }
            else
            {
                m_collider.material = m_injuredMaterial;
                m_rigidbody.mass = m_injuredMass;
                m_animator.SetFloat("AnimationSpeed", m_animationSpeedMultiplier / 2f);
            }
        }
    }

    // ===================================
    // PRIVATE METHODS
    // ===================================

    /// <summary>
    /// Collision response and feedback
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && m_isDamageable)
        {
            Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();

            // Enemy attack response
            if (enemy && enemy.IsAttacking())
            {
                HitArea hitArea = collision.gameObject.GetComponent<HitArea>();

                if (hitArea && hitArea.bodyPart == HitArea.BodyPart.Glove)
                {
                    Debug.Log("Player got punched by enemy");

                    Damage(hitArea.GetAttackDamage());
                }
            }
        }
    }
}