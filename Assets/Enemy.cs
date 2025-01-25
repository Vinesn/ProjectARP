using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100;
    float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("I got hit! HP:" + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Im ded :C");
        Destroy(gameObject);
    }
}
