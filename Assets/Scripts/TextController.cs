using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider2D))] // does something?
public class InteractableText : MonoBehaviour
{
    [Header("References")]
    // the textmeshpro to animate
    public TMP_Text textComponent;

    [Header("Animation Settings")]
    // how fast text fade in out
    public float animationDuration = 0.25f;
    // how far text starts down
    public float yOffset = 0.5f;

    private Vector3 visibleLocalPosition;
    private Vector3 hiddenLocalPosition;
    private Coroutine currentAnimation;

    void Start()
    {
        if (textComponent == null) // in the event im retarded
            textComponent = GetComponentInChildren<TMP_Text>();
        
        visibleLocalPosition = textComponent.transform.localPosition;
        hiddenLocalPosition = visibleLocalPosition - new Vector3(0, yOffset, 0);
        
        // snap to hidden
        textComponent.transform.localPosition = hiddenLocalPosition;
        SetTextAlpha(0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            if (currentAnimation != null) StopCoroutine(currentAnimation);
            currentAnimation = StartCoroutine(AnimateText(visibleLocalPosition, 1f));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Animates back to hiddenLocalPosition and 0f (invisible)
            if (currentAnimation != null) StopCoroutine(currentAnimation);
            currentAnimation = StartCoroutine(AnimateText(hiddenLocalPosition, 0f));
        }
    }

    private IEnumerator AnimateText(Vector3 targetPosition, float targetAlpha)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = textComponent.transform.localPosition;
        float startAlpha = textComponent.color.a;
        
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            
            // Calculate a 0 to 1 percentage of how far along the animation we are
            float t = elapsedTime / animationDuration;

            // Apply an "Ease Out" mathematical curve to make it snappy like Celeste!
            // Instead of moving at a linear boring speed, it pops fast and slows down at the end.
            float easeOut = 1f - Mathf.Pow(1f - t, 3f);

            // Move the text and fade it
            textComponent.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, easeOut);
            SetTextAlpha(Mathf.Lerp(startAlpha, targetAlpha, easeOut));

            yield return null; // Wait until the next frame
        }

        // Ensure it exactly reaches the target at the end
        textComponent.transform.localPosition = targetPosition;
        SetTextAlpha(targetAlpha);
    }

    private void SetTextAlpha(float alpha)
    {
        Color color = textComponent.color;
        color.a = alpha;
        textComponent.color = color;
    }
}