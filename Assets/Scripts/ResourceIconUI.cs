using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ResourceIconUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private GameObject changeIndicator;
    [SerializeField] private TextMeshProUGUI changeText;

    [Header("Animation Settings")]
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeStrength = 15f;
    [SerializeField] private int shakeVibrato = 10;
    [SerializeField] private float shakeRandomness = 90f;
    [SerializeField] private float popupDuration = 0.3f;
    
    private int currentValue;
    private Color positiveColor = new Color(0.2f, 1f, 0.2f);
    private Color negativeColor = new Color(1f, 0.2f, 0.2f);

    public void SetValue(int value)
    {
        int oldValue = currentValue;
        currentValue = value;
        valueText.text = value.ToString();

        // 如果值发生变化，显示动画效果
        if (oldValue != value)
        {
            int change = value - oldValue;
            ShowChangeEffect(change);
        }
    }

    private void ShowChangeEffect(int change)
    {
        // 显示变化指示器
        if (changeIndicator != null && changeText != null)
        {
            changeIndicator.SetActive(true);
            changeText.text = (change > 0 ? "+" : "") + change.ToString();
            changeText.color = change >= 0 ? positiveColor : negativeColor;

            // 重置指示器的位置和缩放
            changeIndicator.transform.localPosition = Vector3.zero;
            changeIndicator.transform.localScale = Vector3.one;

            // 创建弹出动画序列
            Sequence sequence = DOTween.Sequence();
            
            // 图标抖动效果
            sequence.Join(iconImage.transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness));
            
            // 变化指示器动画
            sequence.Join(changeIndicator.transform.DOScale(1.5f, popupDuration).From(1f));
            sequence.Join(changeIndicator.transform.DOMoveY(changeIndicator.transform.position.y + 50f, 1f));
            sequence.Join(changeText.DOFade(0f, 0.5f).SetDelay(0.5f));

            // 动画结束后隐藏指示器
            sequence.OnComplete(() => {
                changeIndicator.SetActive(false);
                changeText.alpha = 1f;
            });
        }

        // 图标缩放动画
        iconImage.transform.DOPunchScale(Vector3.one * 0.5f, 0.3f, 5, 0.5f);
    }

    private void OnDestroy()
    {
        // 清理DOTween动画
        DOTween.Kill(iconImage.transform);
        DOTween.Kill(changeIndicator.transform);
        DOTween.Kill(changeText);
    }
} 