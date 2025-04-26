using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card System/Card Data")]
public class CardDataSO : ScriptableObject
{
    [Header("Card Information")]
    public string title;
    [TextArea(3, 5)]
    public string description;
    public Sprite cardImage;
    
    [Header("Left Choice")]
    public string leftChoiceText;
    public ResourceImpact[] leftChoiceImpacts;

    [Header("Right Choice")]
    public string rightChoiceText;
    public ResourceImpact[] rightChoiceImpacts;

    [System.Serializable]
    public class ResourceImpact
    {
        public enum ResourceType
        {
            Economy,
            Technology,
            Environment,
            Happiness
        }

        public ResourceType resourceType;
        public int impactValue;
    }
} 