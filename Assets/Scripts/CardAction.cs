using UnityEngine;

public class CardAction : MonoBehaviour
{
    [HideInInspector] public bool isCardUsed = false;

    /// <summary>
    /// User dùng 1 lá bài lên target.
    /// </summary>
    public void UseCard(GameObject user, GameObject target, CardData cardDT)
    {
        string method = "CardAction.UseCard";
        isCardUsed = false;

        var energyCtrl = user.GetComponent<EnergyController>();
        var userBal = user.GetComponent<BalanceController>();
        var tgtBal = target?.GetComponent<BalanceController>();

        Debug.Log($"[{method}] Attempting to play '{cardDT.cardName}' by {user.name} (Energy: {energyCtrl.CurrentEnergy}/{energyCtrl.CurrentMaxEnergy}) on {(target != null ? target.name : "NULL")}");

        // 1) Kiểm tra năng lượng
        if (energyCtrl == null || !energyCtrl.UseEnergy(cardDT.energyCost))
        {
            Debug.LogWarning($"[{method}] {user.name} không đủ năng lượng ({cardDT.energyCost} needed). CurrentEnergy={energyCtrl?.CurrentEnergy}");
            return;
        }

        isCardUsed = true;
        Debug.Log($"[{method}] {user.name} used '{cardDT.cardName}', -{cardDT.energyCost} energy → NewEnergy={energyCtrl.CurrentEnergy}");

        // 2) Áp dụng self shift
        if (cardDT.selfBalanceShift != 0 && userBal != null)
        {
            Debug.Log($"[{method}] {user.name} selfBalanceShift = {cardDT.selfBalanceShift}");
            userBal.AddBalance(cardDT.selfBalanceShift);
        }

        // 3) Áp dụng push lên target
        if (cardDT.targetBalanceShift != 0 && tgtBal != null)
        {
            int dir = cardDT.canChooseDirection
                      ? Mathf.RoundToInt(cardDT.direction.x)
                      : (cardDT.targetBalanceShift >= 0 ? 1 : -1);
            int pushAmt = dir * Mathf.Abs(cardDT.targetBalanceShift);

            Debug.Log($"[{method}] {user.name} will push {pushAmt} to {target.name}");
            tgtBal.ReceivePush(pushAmt);
        }

        // 4) Special Defense Effects
        switch (cardDT.cardName)
        {
            case "Planted Stance":
                userBal.nextPushReduction = 2;
                Debug.Log($"[{method}] {user.name} Planted Stance → nextPushReduction=2");
                break;

            case "Guarded Form":
                userBal.nextPushReduction = 1;
                userBal.ReduceBalanceTowardZero(1);
                Debug.Log($"[{method}] {user.name} Guarded Form → nextPushReduction=1, moved toward center by 1");
                break;

            case "Braced Body":
                userBal.blockNextPush = true;
                Debug.Log($"[{method}] {user.name} Braced Body → blockNextPush=true");
                break;

            case "Hold Firm":
                userBal.ResetBalance();
                FindFirstObjectByType<TurnManager>()?.SetPlayerSkipNextTurn();
                Debug.Log($"[{method}] {user.name} Hold Firm → resetBalance & skip next turn");
                break;

            case "Adjust":
                int oldBal = userBal.currentBalance;
                userBal.currentBalance = -oldBal;
                userBal.onBalanceChanged?.Invoke(userBal.currentBalance);
                Debug.Log($"[{method}] {user.name} Adjust → {oldBal} → {-oldBal}");
                break;

            default:
                Debug.Log($"[{method}] No special defense effect for '{cardDT.cardName}'");
                break;
        }
    }
}
