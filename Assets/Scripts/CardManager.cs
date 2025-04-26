using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class CardManager : MonoBehaviour
{
    [SerializeField] private CardDatabase cardDatabase;
    [SerializeField] private ResourceUIManager resourceUIManager;
    [SerializeField] private CardDragHandler cardDragHandler;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI leftChoiceText;
    [SerializeField] private TextMeshProUGUI rightChoiceText;
    [SerializeField] private Image cardImage;

    private List<CardDataSO> availableCards = new List<CardDataSO>();
    private int currentCardIndex = 0;
    private bool isInitialized = false;
    private bool isTransitioning = false;

    private void Start()
    {
        InitializeComponents();
        if (isInitialized)
        {
            InitializeGame();
        }
    }

    private void InitializeComponents()
    {
        // Check CardDragHandler
        if (cardDragHandler == null)
        {
            cardDragHandler = GetComponent<CardDragHandler>();
            if (cardDragHandler == null)
            {
                Debug.LogError("CardDragHandler not found! Please add CardDragHandler component.");
                return;
            }
        }
        
        // Check ResourceUIManager
        if (resourceUIManager == null)
        {
            resourceUIManager = FindObjectOfType<ResourceUIManager>();
            if (resourceUIManager == null)
            {
                Debug.LogError("ResourceUIManager not found in scene!");
                return;
            }
        }

        // Check CardDatabase
        if (cardDatabase == null)
        {
            Debug.LogError("CardDatabase is not assigned! Please assign a CardDatabase in the inspector.");
            return;
        }

        if (cardDatabase.Cards == null || cardDatabase.Cards.Count == 0)
        {
            Debug.LogError("Card database is empty! Please add cards to the database.");
            return;
        }

        // Check UI Components
        if (titleText == null || descriptionText == null || 
            leftChoiceText == null || rightChoiceText == null || cardImage == null)
        {
            Debug.LogError("UI components are not all assigned! Please check the inspector.");
            return;
        }

        isInitialized = true;
    }

    private void InitializeGame()
    {
        // Initialize available cards
        availableCards.Clear();
        availableCards.AddRange(cardDatabase.Cards);
        ShuffleDeck(); // 初始洗牌
        
        // Add event listeners
        cardDragHandler.OnCardDecision.RemoveListener(HandleCardDecision);
        cardDragHandler.OnCardDecision.AddListener(HandleCardDecision);
        
        // Add animation complete listener
        cardDragHandler.OnCardAnimationComplete.RemoveListener(OnCardAnimationComplete);
        cardDragHandler.OnCardAnimationComplete.AddListener(OnCardAnimationComplete);
        
        // Show first card
        currentCardIndex = 0; // 确保从第一张卡开始
        ShowCurrentCard();
        Debug.Log("Card system initialized successfully!");
    }

    public void HandleCardDecision(bool isRightChoice)
    {
        if (!isInitialized || isTransitioning)
        {
            return;
        }

        if (currentCardIndex >= availableCards.Count)
        {
            Debug.LogError("Current card index out of range!");
            return;
        }

        if (ResourceManager.Instance == null)
        {
            Debug.LogError("ResourceManager instance not found!");
            return;
        }

        isTransitioning = true;

        CardDataSO currentCard = availableCards[currentCardIndex];
        CardDataSO.ResourceImpact[] impacts = isRightChoice ? currentCard.rightChoiceImpacts : currentCard.leftChoiceImpacts;

        // Apply resource impacts
        if (impacts != null)
        {
            foreach (var impact in impacts)
            {
                ResourceManager.Instance.UpdateResource(impact.resourceType.ToString(), impact.impactValue);
            }
        }

        // Check for game over
        if (ResourceManager.Instance.CheckGameOver())
        {
            Debug.Log("Game Over!");
            isTransitioning = false;
            return;
        }

        // Move to next card
        currentCardIndex++;
        
        // 如果到达牌组末尾，重新洗牌
        if (currentCardIndex >= availableCards.Count)
        {
            Debug.Log("重新洗牌...");
            ShuffleDeck();
            currentCardIndex = 0;
        }
    }

    private void OnCardAnimationComplete()
    {
        if (!isInitialized) return;
        
        ShowCurrentCard();
        isTransitioning = false;
    }

    public void ShowCurrentCard()
    {
        if (!isInitialized)
        {
            Debug.LogError("CardManager not properly initialized!");
            return;
        }

        if (currentCardIndex >= availableCards.Count)
        {
            Debug.LogError("No cards available to show!");
            return;
        }

        CardDataSO currentCard = availableCards[currentCardIndex];
        
        // 使用 CanvasGroup 来实现淡入效果
        CanvasGroup canvasGroup = cardDragHandler.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            StartCoroutine(FadeInCard(canvasGroup, currentCard));
        }
    }

    private System.Collections.IEnumerator FadeInCard(CanvasGroup canvasGroup, CardDataSO card)
    {
        canvasGroup.alpha = 0f;

        // 先更新所有内容
        titleText.text = card.title;
        descriptionText.text = card.description;
        leftChoiceText.text = card.leftChoiceText;
        rightChoiceText.text = card.rightChoiceText;
        cardImage.sprite = card.cardImage;

        float elapsedTime = 0f;
        float fadeDuration = 0.3f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    public void AddCardToDeck(CardDataSO card)
    {
        if (!availableCards.Contains(card))
        {
            availableCards.Add(card);
        }
    }

    public void RemoveCardFromDeck(CardDataSO card)
    {
        availableCards.Remove(card);
    }

    public void ShuffleDeck()
    {
        System.Random rng = new System.Random();
        int n = availableCards.Count;
        
        // Fisher-Yates 洗牌算法
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            CardDataSO temp = availableCards[k];
            availableCards[k] = availableCards[n];
            availableCards[n] = temp;
        }
        
        Debug.Log($"牌组已洗牌，共 {availableCards.Count} 张卡片");
    }

    // 添加重新开始游戏的方法
    public void RestartGame()
    {
        currentCardIndex = 0;
        ShuffleDeck();
        ShowCurrentCard();
        Debug.Log("游戏重新开始，牌组已重新洗牌");
    }
} 