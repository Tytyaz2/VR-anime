using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("Références Obligatoires")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image healthFill;

    [Header("Paramètres")]
    [SerializeField] private float yOffset = 1.5f;
    private float initialWidth;

    private Transform target;

    void Awake()
    {
        // Vérifications critiques
        if (canvas == null) canvas = GetComponent<Canvas>();
        if (healthFill == null) healthFill = GetComponentInChildren<Image>();

        initialWidth = healthFill.rectTransform.sizeDelta.x;

        // Configuration essentielle
        canvas.worldCamera = Camera.main;
        canvas.sortingOrder = 1000; // Pour s'afficher au-dessus
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Positionnement précis
        transform.position = target.position + Vector3.up * yOffset;
        transform.rotation = Camera.main.transform.rotation;
    }

    public void Initialize(Transform enemyTransform)
    {
        target = enemyTransform;
        gameObject.SetActive(true); // Activation explicite
    }

    public void UpdateHealth(float healthPercent)
    {
        healthFill.rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            initialWidth * Mathf.Clamp01(healthPercent)
        );
    }
}