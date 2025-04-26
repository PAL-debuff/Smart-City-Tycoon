using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private CanvasGroup mainMenuCanvas;
    [SerializeField] private Image cityIcon;

    [Header("Animation Settings")]
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private float titleBobSpeed = 1f;
    [SerializeField] private float titleBobAmount = 10f;
    [SerializeField] private float buttonHoverScale = 1.1f;
    [SerializeField] private float buttonAnimationSpeed = 10f;
    [SerializeField] private float cityRotateAmount = 5f;
    [SerializeField] private float cityRotateSpeed = 1f;

    private void Start()
    {
        // 设置按钮监听
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
            SetupButtonAnimation(startButton);
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
            SetupButtonAnimation(quitButton);
        }

        // 启动动画
        if (titleText != null)
            StartCoroutine(AnimateTitle());

        if (cityIcon != null)
            StartCoroutine(AnimateCity());

        // 淡入菜单
        StartCoroutine(FadeInMenu());
    }

    private void SetupButtonAnimation(Button button)
    {
        // 添加按钮悬停效果
        button.transition = Selectable.Transition.Animation;
        
        // 获取按钮的 RectTransform
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        
        // 添加事件触发器组件
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
        
        // 鼠标进入事件
        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((data) => {
            StartCoroutine(ScaleButton(rectTransform, buttonHoverScale));
        });
        trigger.triggers.Add(enterEntry);
        
        // 鼠标离开事件
        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => {
            StartCoroutine(ScaleButton(rectTransform, 1f));
        });
        trigger.triggers.Add(exitEntry);
    }

    private IEnumerator ScaleButton(RectTransform rectTransform, float targetScale)
    {
        Vector3 startScale = rectTransform.localScale;
        Vector3 targetScaleVector = Vector3.one * targetScale;
        float elapsedTime = 0;
        
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * buttonAnimationSpeed;
            rectTransform.localScale = Vector3.Lerp(startScale, targetScaleVector, elapsedTime);
            yield return null;
        }
    }

    private IEnumerator AnimateTitle()
    {
        Vector3 startPos = titleText.transform.position;
        float time = 0;

        while (true)
        {
            time += Time.deltaTime * titleBobSpeed;
            float newY = startPos.y + Mathf.Sin(time) * titleBobAmount;
            titleText.transform.position = new Vector3(startPos.x, newY, startPos.z);
            yield return null;
        }
    }

    private IEnumerator AnimateCity()
    {
        Quaternion startRotation = cityIcon.transform.rotation;
        float time = 0;

        while (true)
        {
            time += Time.deltaTime * cityRotateSpeed;
            float rotationZ = Mathf.Sin(time) * cityRotateAmount;
            cityIcon.transform.rotation = startRotation * Quaternion.Euler(0, 0, rotationZ);
            yield return null;
        }
    }

    private IEnumerator FadeInMenu()
    {
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.alpha = 0;
            while (mainMenuCanvas.alpha < 1)
            {
                mainMenuCanvas.alpha += Time.deltaTime * fadeSpeed;
                yield return null;
            }
        }
    }

    public void StartGame()
    {
        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        // 淡出菜单
        if (mainMenuCanvas != null)
        {
            while (mainMenuCanvas.alpha > 0)
            {
                mainMenuCanvas.alpha -= Time.deltaTime * fadeSpeed;
                yield return null;
            }
        }

        // 加载游戏场景
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
} 