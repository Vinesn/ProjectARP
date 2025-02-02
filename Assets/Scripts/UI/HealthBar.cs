using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthUI;
    public Slider healthUIForSh;
    [SerializeField] float healthLerpSpeed = 0.01f;

    EnemyController enemyController;

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
    }

    private void Start()
    {
        enemyController.currentHealth = enemyController.maxHealth;
        healthUIForSh.maxValue = enemyController.maxHealth;
        healthUIForSh.value = enemyController.maxHealth;
        healthUI.maxValue = enemyController.maxHealth;
    }

    private void Update()
    {
        if (healthUI.value != enemyController.currentHealth)
        {
            healthUI.value = enemyController.currentHealth;
        }

        if (healthUIForSh.value != healthUI.value)
        {
            healthUIForSh.value = Mathf.Lerp(healthUIForSh.value, enemyController.currentHealth, healthLerpSpeed);
        }
    }
}
