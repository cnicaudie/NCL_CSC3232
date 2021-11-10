using UnityEngine;

public class Player : Entity
{
    private Animator m_animator;

    [SerializeField] private PhysicMaterial m_defaultMaterial;
    [SerializeField] private PhysicMaterial m_injuredMaterial;
    private CapsuleCollider m_collider;

    private float m_animationSpeedMultiplier = 1f;

    // ===================================

    protected override void Start()
    {
        base.Start();

        m_animator = GetComponent<Animator>();
        m_collider = GetComponent<CapsuleCollider>();
        m_animator.SetFloat("AnimationSpeed", m_animationSpeedMultiplier);
    }

    protected override void Update()
    {
        if (GameManager.IsGamePlaying())
        {
            base.Update();

            if (m_isDamageable)
            {
                m_collider.material = m_defaultMaterial;
                m_animator.SetFloat("AnimationSpeed", m_animationSpeedMultiplier);
            }
            else
            {
                m_collider.material = m_injuredMaterial;
                m_animator.SetFloat("AnimationSpeed", m_animationSpeedMultiplier / 2f);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();

            if (enemy && enemy.IsAttacking())
            {
                HitArea hitArea = collision.gameObject.GetComponent<HitArea>();

                if (hitArea != null && hitArea.bodyPart == HitArea.BodyPart.Glove && m_isDamageable)
                {
                    Debug.Log("Player got punched by enemy");

                    Damage(hitArea.GetAttackDamage());
                }
            }
        }
    }
}