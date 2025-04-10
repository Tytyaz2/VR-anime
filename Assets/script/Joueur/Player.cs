using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public bool isInvincible = false;
    public float invincibilityDuration = 1f;
    public UnityEvent OnPlayerDeath;

    [Header("Level System")]
    public int level = 1;
    public int currentExp = 0;
    public int[] expToNextLevel;
    public int maxLevel = 10;
    public UnityEvent OnLevelUp;

    [Header("Visual Feedback")]
    public HealthBarUI healthBar;
    public ExpBarUI expBar;
    public ParticleSystem levelUpEffect;
    public AudioClip levelUpSound;

    private AudioSource audioSource;
    private float invincibilityTimer;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
        InitializeExpRequirements();
    }

    void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
            }
        }
    }

    void InitializeExpRequirements()
    {
        expToNextLevel = new int[maxLevel];
        for (int i = 0; i < maxLevel; i++)
        {
            expToNextLevel[i] = (int)(100 * Mathf.Pow(1.5f, i)); // Croissance exponentielle
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || currentHealth <= 0) return;

        currentHealth -= damage;
        healthBar?.UpdateHealth((float)currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        healthBar?.UpdateHealth((float)currentHealth / maxHealth);
    }

    public void AddExp(int amount)
    {
        if (level >= maxLevel) return;

        currentExp += amount;

        while (currentExp >= expToNextLevel[level - 1] && level < maxLevel)
        {
            LevelUp();
        }

        expBar?.UpdateExp((float)currentExp / expToNextLevel[level - 1]);
    }

    void LevelUp()
    {
        currentExp -= expToNextLevel[level - 1];
        level++;

        // Amélioration des stats
        maxHealth += 20;
        currentHealth = maxHealth;

        // Feedback
        if (levelUpEffect != null) levelUpEffect.Play();
        if (levelUpSound != null) audioSource.PlayOneShot(levelUpSound);

        OnLevelUp.Invoke();
        healthBar?.UpdateHealth(1f);
    }

    void Die()
    {
        OnPlayerDeath.Invoke();
        // Ici vous pouvez ajouter une logique de respawn ou de game over
        Debug.Log("Player Died");
    }

    public void FullHeal()
    {
        currentHealth = maxHealth;
        healthBar?.UpdateHealth(1f);
    }
}