using UnityEngine;
using System;

[System.Serializable]
public class CardData
{
    public string title;
    public string description;
    public Sprite cardImage;
    
    [Header("Left Choice")]
    public string leftChoiceText;
    [Serializable]
    public class ResourceImpact
    {
        public string resourceName;
        public int impactValue;
    }
    public ResourceImpact[] leftChoiceImpacts;

    [Header("Right Choice")]
    public string rightChoiceText;
    public ResourceImpact[] rightChoiceImpacts;
} 