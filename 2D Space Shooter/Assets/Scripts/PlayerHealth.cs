using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float currentHealthPoints = 1f;
    [SerializeField] private float respawnTime = 3f;
    [SerializeField] private Slider healthSlider = null;
    [SerializeField] private Image healthImage = null;
    [SerializeField] private Gradient healthGradient = null;
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private BoxCollider2D boxCollider = null;


    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();
        UpdateHealthBar();
    }
    public void TakeOrHealDamage(float change)
    {
        currentHealthPoints = Mathf.Clamp(currentHealthPoints + change, 0f, 1f);
        UpdateHealthBar();
        if (currentHealthPoints <= 0f)
        {
            GameManager.instance.SetRespawning(true);
            GameManager.instance.playerLives--;
            spriteRenderer.enabled = false;
            boxCollider.enabled = false;
            StartCoroutine(RespawnPlayer());
        }
    }

    private void UpdateHealthBar()
    {
        healthSlider.value = currentHealthPoints;
        healthImage.color = healthGradient.Evaluate(currentHealthPoints);
    }

    private IEnumerator RespawnPlayer()
    {
        if (GameManager.instance.playerLives < 0)
        {
            yield return new WaitForEndOfFrame();
            Debug.Log("Game Over!");
        }
        else
        {
            yield return new WaitForSeconds(respawnTime);
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        GameManager.instance.SetRespawning(false);
        currentHealthPoints = 1f;
        StartCoroutine(PlayerBlink());
    }

    private IEnumerator PlayerBlink()
    {
        for (int i = 0; i < 30; i++)
        {
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.enabled = !spriteRenderer.enabled;
        }
        spriteRenderer.enabled = true;
        boxCollider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyLaser"))
        {
            var enemyLaser = collision.GetComponent<EnemyLaser>();
            if (enemyLaser)
            {
                TakeOrHealDamage(-enemyLaser.damage);
                Destroy(enemyLaser.gameObject);
            }
        }
    }
}
