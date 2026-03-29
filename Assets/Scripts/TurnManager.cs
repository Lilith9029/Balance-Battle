using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy;

    private bool isPlayerTurn = true;
    private bool playerSkipNextTurn = false;

    private EnergyController playerEnergy;
    private HandController playerHand;

    private void Start()
    {
        playerEnergy = player.GetComponent<EnergyController>();
        playerHand = player.GetComponent<HandController>();
    }

    public void EndTurn()
    {
        Debug.Log($"[TurnManager] Kết thúc lượt của {(isPlayerTurn ? "Player" : "Enemy")}");

        isPlayerTurn = !isPlayerTurn;

        if (isPlayerTurn)
        {
            if (playerSkipNextTurn)
            {
                Debug.Log("[TurnManager] Player mất lượt!");
                playerSkipNextTurn = false;
                isPlayerTurn = false;  // Enemy chơi tiếp
                return;
            }

            StartPlayerTurn();
        }
        else
        {
            StartEnemyTurn();
        }
    }

    private void StartPlayerTurn()
    {
        Debug.Log("[TurnManager] Bắt đầu lượt của Player");

        if (playerEnergy != null)
            playerEnergy.ResetEnergy();   // 🟡 Reset lại energy

        if (playerHand != null)
        {
            playerHand.ClearHand();       // 🔵 Bỏ hết thẻ cũ
            playerHand.DrawStartingHand(); // 🟢 Rút bộ thẻ mới
        }
    }

    private void StartEnemyTurn()
    {
        Debug.Log("[TurnManager] Bắt đầu lượt của Enemy");
        // Enemy logic...
    }

    public void SetPlayerSkipNextTurn()
    {
        playerSkipNextTurn = true;
    }
}
