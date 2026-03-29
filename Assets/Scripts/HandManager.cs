using UnityEngine;
using System.Collections.Generic;

public class HandController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cardArea;
    [SerializeField] private GameObject cardPrefab;

    [Header("Cards")]
    [SerializeField] private List<CardData> startingCards;
    private List<CardData> deckCards = new List<CardData>();
    private List<GameObject> currentHand = new List<GameObject>();  // 🟡 lưu các thẻ đang có trên tay

    private void Start()
    {
        // Load tất cả CardData từ Resources
        deckCards.AddRange(Resources.LoadAll<CardData>("Cards_Red"));
        deckCards.AddRange(Resources.LoadAll<CardData>("Cards_Green"));

        DrawStartingHand();
    }

    public void DrawStartingHand()
    {
        ClearHand();

        for (int i = 0; i < startingCards.Count; i++)
        {
            DrawRandomCard();
        }
    }

    private void DrawRandomCard()
    {
        if (deckCards.Count == 0)
        {
            Debug.LogWarning("[HandController] Deck hết thẻ!");
            return;
        }

        CardData randomCard = deckCards[Random.Range(0, deckCards.Count)];
        GameObject newCard = Instantiate(cardPrefab, cardArea);
        newCard.GetComponent<CardDisplay>().cardSettup(randomCard);
        currentHand.Add(newCard);
    }

    public void ClearHand()
    {
        foreach (var card in currentHand)
        {
            Destroy(card);
        }
        currentHand.Clear();
        Debug.Log("[HandController] Đã clear hand.");
    }
}
