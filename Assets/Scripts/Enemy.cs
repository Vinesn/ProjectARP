using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100;
    [SerializeField] public float currentHealth;
    SpriteRenderer spriteController;

    private void Awake()
    {
        spriteController = GetComponentInChildren<SpriteRenderer>();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        StartCoroutine(faintAndDie());
    }

    IEnumerator faintAndDie()
    {
        spriteController.flipY = true;

        yield return new WaitForSeconds(3f);

        Destroy(gameObject);
    }
}
