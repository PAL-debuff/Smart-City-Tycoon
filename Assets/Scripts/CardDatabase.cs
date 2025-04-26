using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CardDatabase", menuName = "Card System/Card Database")]
public class CardDatabase : ScriptableObject
{
    [SerializeField] private List<CardDataSO> cards = new List<CardDataSO>();

    public List<CardDataSO> Cards => cards;

    public void AddCard(CardDataSO card)
    {
        if (!cards.Contains(card))
        {
            cards.Add(card);
        }
    }

    public void RemoveCard(CardDataSO card)
    {
        cards.Remove(card);
    }

    public CardDataSO GetRandomCard()
    {
        if (cards.Count == 0) return null;
        return cards[Random.Range(0, cards.Count)];
    }

    public List<CardDataSO> GetCardsByTag(string tag)
    {
        // Implement tag-based filtering if needed
        return cards;
    }

    #if UNITY_EDITOR
    public void RefreshDatabase()
    {
        // Find all CardDataSO assets in the project
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:CardDataSO");
        cards.Clear();

        foreach (string guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            CardDataSO card = UnityEditor.AssetDatabase.LoadAssetAtPath<CardDataSO>(path);
            if (card != null)
            {
                cards.Add(card);
            }
        }
    }
    #endif
} 