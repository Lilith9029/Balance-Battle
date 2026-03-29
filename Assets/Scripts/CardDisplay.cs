using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] private Image cardImage;
    [SerializeField] private Button cardButton;
    [SerializeField] private CardData cardData;

    private CardAction cardAction;

    public void cardSettup(CardData cardDT)
    {
        cardData = cardDT;
        cardImage.sprite = cardData.cardSprite;
        cardButton.onClick.AddListener(() => OnCardClicked());
    }

    private void OnCardClicked()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var enemy = GameObject.FindGameObjectWithTag("Enemy");

        CardAction cardAction = FindFirstObjectByType<CardAction>();
        if (cardAction != null)
        {
            cardAction.UseCard(player, enemy, cardData);
            if (cardAction.isCardUsed == true)
            {
                Destroy(gameObject);
            }
        }
    }
}
