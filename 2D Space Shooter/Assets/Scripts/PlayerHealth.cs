using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;
    [SerializeField] private float currentHealthPoints = 1f;
    [SerializeField] private float respawnTime = 3f;
    [SerializeField] private Slider healthSlider = null;
    [SerializeField] private Image healthImage = null;
    [SerializeField] private Gradient healthGradient = null;
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private BoxCollider2D boxCollider = null;
    [SerializeField] private GameObject shield = null;
    [SerializeField] private string explosionRef = null;

    private void Start()
    {
        instance = this;
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();
        UpdateHealthBar();
    }

    public void ActivateShield() => shield?.SetActive(true);

    public void TakeOrHealDamage(float change)
    {
        if (shield != null && shield.activeSelf)
        {
            shield.SetActive(false);
            return;
        }

        currentHealthPoints = Mathf.Clamp(currentHealthPoints + change, 0f, 1f);
        UpdateHealthBar();
        if (currentHealthPoints <= 0f)
        {
            VFXPool.instance.Spawn(transform.position);
            SoundPool.instance.Spawn(transform.position, explosionRef);
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
            GameManager.instance.GameOver();
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
        UpdateHealthBar();
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
