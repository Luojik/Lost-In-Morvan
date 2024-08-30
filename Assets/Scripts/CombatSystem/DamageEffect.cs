using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageEffect : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public float arcHeight = 2f;
    public float duration = 1f;

    public void ShowDamage(int damage, Vector3 worldPosition, float xOffset, float yOffset)
    {
        worldPosition.x += xOffset;
        worldPosition.y += yOffset;

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        Canvas canvas = FindObjectOfType<Canvas>();

        GameObject damageTextObj = Instantiate(damageTextPrefab, canvas.transform);

        RectTransform rectTransform = damageTextObj.GetComponent<RectTransform>();
        rectTransform.position = screenPosition;

        Text damageText = damageTextObj.GetComponent<Text>();
        damageText.text = damage.ToString();

        StartCoroutine(AnimateDamageText(damageTextObj, screenPosition));
    }

    private IEnumerator AnimateDamageText(GameObject damageTextObj, Vector3 startPosition)
    {
        Vector3 endPosition = startPosition + new Vector3(0, 1, 0);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float progress = elapsedTime / duration;
            float height = Mathf.Sin(Mathf.PI * progress) * arcHeight;
            damageTextObj.transform.position = Vector3.Lerp(startPosition, endPosition, progress) + new Vector3(0, height, 0);

            Text textComponent = damageTextObj.GetComponent<Text>();
            Color color = textComponent.color;
            color.a = 1 - progress;
            textComponent.color = color;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(damageTextObj);
    }
}

