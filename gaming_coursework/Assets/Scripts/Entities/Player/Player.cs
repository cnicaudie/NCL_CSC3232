using UnityEngine;

public class Player : Entity
{
    // ===================================

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            HitArea hitArea = collision.gameObject.GetComponent<HitArea>();

            if (hitArea != null && hitArea.bodyPart == HitArea.BodyPart.Glove && m_isDamageable)
            {
                Debug.Log("Player got punched by enemy");

                Damage(hitArea.GetAttackDamage());

                // TODO : Change physic material temporary
            }
        }
    }
}