using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float currentHealthPoints = 1f;
    [SerializeField] private Slider healthSlider = null;
    [SerializeField] private Image healthImage = null;
    [SerializeField] private Gradient healthGradient = null;

    public void TakeOrHealDamage(float change)
    {
        currentHealthPoints = Mathf.Clamp(currentHealthPoints + change, 0f, 1f);
        UpdateHealthBar();
    }

    private void Start()
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthSlider.value = currentHealthPoints;
        healthImage.color = healthGradient.Evaluate(currentHealthPoints);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyLaser"))
        {
            var enemyLaser = collision.GetComponent<EnemyLaser>();
            if (enemyLaser)
            {
                currentHealthPoints -= enemyLaser.damage;
                UpdateHealthBar();
                Destroy(enemyLaser.gameObject);
            }
        }
    }
}
