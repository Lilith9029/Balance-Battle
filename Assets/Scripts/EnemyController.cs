using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum EnemyAILevel
    {
        Basic,
        Strategic,
        Aggressive,
        BalancedMaster
    }

    public EnemyAILevel aiLevel = EnemyAILevel.Basic;
    public CardData[] enemyDeck;
    public float actionDelay = 1.2f;
    public GameObject player;

    private CardAction cardAction;

    private void Awake()
    {
        cardAction = GetComponent<CardAction>();
    }

    public void PlayTurn()
    {
        StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator EnemyTurnRoutine()
    {
        yield return new WaitForSeconds(actionDelay);

        CardData chosenCard = ChooseCard();

        if (chosenCard != null)
        {
            Debug.Log($"[EnemyAI] ({aiLevel}) chọn thẻ: {chosenCard.cardName}");
            cardAction.UseCard(gameObject, player, chosenCard);
        }

        yield return new WaitForSeconds(0.5f);
        FindFirstObjectByType<TurnManager>().EndTurn();
    }

    private CardData ChooseCard()
    {
        switch (aiLevel)
        {
            case EnemyAILevel.Basic:
                return enemyDeck[Random.Range(0, enemyDeck.Length)];

            case EnemyAILevel.Strategic:
                return ChooseStrategicCard();

            case EnemyAILevel.Aggressive:
                return ChooseCardWithHighestPush();

            case EnemyAILevel.BalancedMaster:
                return ChooseBestBalanceOption();

            default:
                return enemyDeck[0];
        }
    }

    private CardData ChooseStrategicCard()
    {
        // Ưu tiên thẻ có hướng lệch ngược nếu player đang nghiêng
        int playerBalance = player.GetComponent<BalanceController>().currentBalance;

        foreach (var card in enemyDeck)
        {
            if (playerBalance > 2 && card.targetBalanceShift > 0) return card;
            if (playerBalance < -2 && card.targetBalanceShift < 0) return card;
        }

        return enemyDeck[Random.Range(0, enemyDeck.Length)];
    }

    private CardData ChooseCardWithHighestPush()
    {
        CardData best = null;
        int maxPush = int.MinValue;
        foreach (var card in enemyDeck)
        {
            int push = Mathf.Abs(card.targetBalanceShift);
            if (push > maxPush)
            {
                best = card;
                maxPush = push;
            }
        }
        return best;
    }

    private CardData ChooseBestBalanceOption()
    {
        // Kết hợp logic phòng thủ + tấn công nếu đang mất cân bằng
        var self = GetComponent<BalanceController>();
        if (Mathf.Abs(self.currentBalance) > 3)
        {
            // Ưu tiên thẻ cân bằng hoặc phòng thủ
            foreach (var card in enemyDeck)
                if (card.cardType == CardData.CardType.Defense_Green) return card;
        }

        return ChooseCardWithHighestPush();
    }
}