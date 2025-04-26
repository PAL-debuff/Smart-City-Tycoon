using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Resource UI")]
    [SerializeField] private Text economyText;
    [SerializeField] private Text technologyText;
    [SerializeField] private Text environmentText;
    [SerializeField] private Text happinessText;
    [SerializeField] private Slider[] resourceSliders;

    [Header("Card UI")]
    [SerializeField] private Text cardTitleText;
    [SerializeField] private Text cardDescriptionText;
    [SerializeField] private Image cardImage;
    [SerializeField] private Text leftChoiceText;
    [SerializeField] private Text rightChoiceText;
    [SerializeField] private GameObject choiceIndicatorLeft;
    [SerializeField] private GameObject choiceIndicatorRight;

    private ResourceManager resourceManager;

    private void Start()
    {
        resourceManager = ResourceManager.Instance;
        if (resourceManager != null)
        {
            resourceManager.OnResourcesUpdated.AddListener(UpdateResourceUI);
        }

        // Hide choice indicators initially
        if (choiceIndicatorLeft) choiceIndicatorLeft.SetActive(false);
        if (choiceIndicatorRight) choiceIndicatorRight.SetActive(false);
    }

    public void UpdateResourceUI(ResourceManager.Resource[] resources)
    {
        foreach (var resource in resources)
        {
            switch (resource.name)
            {
                case "Economy":
                    UpdateResourceDisplay(economyText, 0, resource);
                    break;
                case "Technology":
                    UpdateResourceDisplay(technologyText, 1, resource);
                    break;
                case "Environment":
                    UpdateResourceDisplay(environmentText, 2, resource);
                    break;
                case "Happiness":
                    UpdateResourceDisplay(happinessText, 3, resource);
                    break;
            }
        }
    }

    private void UpdateResourceDisplay(Text textComponent, int sliderIndex, ResourceManager.Resource resource)
    {
        if (textComponent != null)
        {
            textComponent.text = $"{resource.name}: {resource.value}";
        }

        if (resourceSliders != null && sliderIndex < resourceSliders.Length)
        {
            Slider slider = resourceSliders[sliderIndex];
            if (slider != null)
            {
                slider.minValue = resource.minValue;
                slider.maxValue = resource.maxValue;
                slider.value = resource.value;
            }
        }
    }

    public void ShowChoiceIndicator(bool isLeft, bool show)
    {
        if (isLeft && choiceIndicatorLeft)
        {
            choiceIndicatorLeft.SetActive(show);
        }
        else if (!isLeft && choiceIndicatorRight)
        {
            choiceIndicatorRight.SetActive(show);
        }
    }

    public void UpdateCardUI(CardDataSO card)
    {
        if (card == null) return;

        if (cardTitleText) cardTitleText.text = card.title;
        if (cardDescriptionText) cardDescriptionText.text = card.description;
        if (cardImage) cardImage.sprite = card.cardImage;
        if (leftChoiceText) leftChoiceText.text = card.leftChoiceText;
        if (rightChoiceText) rightChoiceText.text = card.rightChoiceText;
    }
} 