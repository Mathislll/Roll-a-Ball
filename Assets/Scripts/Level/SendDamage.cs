using UnityEngine;

public class SendDamage : MonoBehaviour
{
    public bool isTrigger = false;
    public int damageAmount = 1;
    public string targetTag = "Player";

    [Header("Options")]
    public bool destroyOnImpact = false;
    public bool continuousDamage = false;

    void OnCollisionEnter(Collision collision)
    {
        if (!isTrigger)
        {
            TryDealDamage(collision.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isTrigger)
        {
            TryDealDamage(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (isTrigger && continuousDamage)
        {
            TryDealDamage(other.gameObject);
        }
    }

    void TryDealDamage(GameObject target)
    {
        if (target.CompareTag(targetTag))
        {
            PlayerController player = target.GetComponent<PlayerController>();

            if (player != null)
            {
                player.TakeDamage(damageAmount);

                if (destroyOnImpact)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}