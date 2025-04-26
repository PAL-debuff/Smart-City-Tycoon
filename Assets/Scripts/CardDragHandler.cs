using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [System.Serializable]
    public class CardDecisionEvent : UnityEvent<bool> { }

    [Header("Drag Settings")]
    [SerializeField] private float swipeThreshold = 100f;
    [SerializeField] private float dragSpeed = 10f;
    [SerializeField] private float maxRotationAngle = 30f;
    [SerializeField] private float returnSpeed = 15f;
    [SerializeField] private float cardResetDelay = 0.5f;

    [Header("Events")]
    public CardDecisionEvent OnCardDecision;
    public UnityEvent OnCardAnimationComplete;

    private RectTransform rectTransform;
    private Vector2 startPosition;
    private bool isDragging = false;
    private Vector2 dragStartPosition;
    private bool isAnimating = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isAnimating) return;
        isDragging = true;
        dragStartPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || isAnimating) return;

        // Calculate drag position
        Vector2 currentPosition = eventData.position;
        Vector2 dragDelta = currentPosition - dragStartPosition;
        
        // Update card position
        Vector2 newPosition = startPosition + new Vector2(dragDelta.x, 0);
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, newPosition, Time.deltaTime * dragSpeed);

        // Calculate and apply rotation based on drag distance
        float dragDistance = newPosition.x - startPosition.x;
        float rotation = Mathf.Clamp(dragDistance * 0.1f, -maxRotationAngle, maxRotationAngle);
        rectTransform.rotation = Quaternion.Euler(0, 0, rotation);

        // Visual feedback based on drag direction
        UpdateVisualFeedback(dragDistance);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging || isAnimating) return;
        isDragging = false;
        float dragDistance = rectTransform.anchoredPosition.x - startPosition.x;

        if (Mathf.Abs(dragDistance) > swipeThreshold)
        {
            // Trigger decision event
            bool isRightSwipe = dragDistance > 0;
            OnCardDecision?.Invoke(isRightSwipe);
            
            // Animate card off screen
            StartCoroutine(AnimateCardOffScreen(isRightSwipe));
        }
        else
        {
            // Return card to center
            StartCoroutine(ReturnCardToCenter());
        }
    }

    private void UpdateVisualFeedback(float dragDistance)
    {
        // You can add visual feedback here, like changing card color or showing indicators
        // based on the drag distance and direction
    }

    private System.Collections.IEnumerator AnimateCardOffScreen(bool isRightSwipe)
    {
        isAnimating = true;
        Vector2 targetPosition = startPosition + new Vector2(isRightSwipe ? 1000 : -1000, 0);
        Quaternion targetRotation = Quaternion.Euler(0, 0, isRightSwipe ? maxRotationAngle : -maxRotationAngle);

        // 先移动到目标位置
        while (Vector2.Distance(rectTransform.anchoredPosition, targetPosition) > 0.1f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, Time.deltaTime * returnSpeed);
            rectTransform.rotation = Quaternion.Lerp(rectTransform.rotation, targetRotation, Time.deltaTime * returnSpeed);
            yield return null;
        }

        // 等待一段时间
        yield return new WaitForSeconds(cardResetDelay);

        // 立即将卡片移到起始位置但设置为透明
        rectTransform.anchoredPosition = startPosition;
        rectTransform.rotation = Quaternion.identity;

        // 通知可以更新卡片内容
        OnCardAnimationComplete?.Invoke();
        
        isAnimating = false;
    }

    private System.Collections.IEnumerator ReturnCardToCenter()
    {
        isAnimating = true;
        while (Vector2.Distance(rectTransform.anchoredPosition, startPosition) > 0.1f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, startPosition, Time.deltaTime * returnSpeed);
            rectTransform.rotation = Quaternion.Lerp(rectTransform.rotation, Quaternion.identity, Time.deltaTime * returnSpeed);
            yield return null;
        }

        rectTransform.anchoredPosition = startPosition;
        rectTransform.rotation = Quaternion.identity;
        isAnimating = false;
    }
} 