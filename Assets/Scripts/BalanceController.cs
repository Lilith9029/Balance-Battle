using UnityEngine;
using UnityEngine.Events;

public class BalanceController : MonoBehaviour
{
    [Header("Balance Settings")]
    public int maxBalance = 9;
    public int currentBalance = 0;

    [Header("Push Defense")]
    public int nextPushReduction = 0;
    public bool blockNextPush = false;

    [Header("Events")]
    public UnityEvent<int> onBalanceChanged;
    public UnityEvent onDamageTaken;

    private void Start()
    {
        currentBalance = 0;
        Debug.Log($"[{gameObject.name}][BalanceController.Start] Initialized with currentBalance = {currentBalance}");
        onBalanceChanged?.Invoke(currentBalance);
    }

    /// <summary>
    /// Thay đổi thăng bằng, clamp và trigger sự kiện.
    /// </summary>
    public void AddBalance(int amount)
    {
        int old = currentBalance;
        currentBalance += amount;

        if (Mathf.Abs(currentBalance) > maxBalance)
        {
            Debug.LogWarning($"[{gameObject.name}][BalanceController.AddBalance] OUT OF BALANCE! Attempted {old} + {amount} = {currentBalance}");
            onDamageTaken?.Invoke();
            currentBalance = Mathf.Clamp(currentBalance, -maxBalance, maxBalance);
            Debug.Log($"[{gameObject.name}][BalanceController.AddBalance] Clamped to {currentBalance}");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"[{gameObject.name}][BalanceController.AddBalance] {old} + {amount} = {currentBalance}");
        }

        onBalanceChanged?.Invoke(currentBalance);
        Debug.Log($"[{gameObject.name}][BalanceController.AddBalance] onBalanceChanged invoked with {currentBalance}");

        Debug.Log($"[BalanceController] AddBalance({amount}) → New = {currentBalance}");

    }

    /// <summary>
    /// Giảm thăng bằng về 0 theo amount.
    /// </summary>
    public void ReduceBalanceTowardZero(int amount)
    {
        int old = currentBalance;
        if (currentBalance > 0)
            currentBalance = Mathf.Max(0, currentBalance - amount);
        else if (currentBalance < 0)
            currentBalance = Mathf.Min(0, currentBalance + amount);

        Debug.Log($"[{gameObject.name}][BalanceController.ReduceBalanceTowardZero] {old} -> {currentBalance} by amount {amount}");
        onBalanceChanged?.Invoke(currentBalance);
        Debug.Log($"[{gameObject.name}][BalanceController.ReduceBalanceTowardZero] onBalanceChanged invoked with {currentBalance}");
    }

    /// <summary>
    /// Reset thăng bằng về 0.
    /// </summary>
    public void ResetBalance()
    {
        Debug.Log($"[{gameObject.name}][BalanceController.ResetBalance] {currentBalance} -> 0");
        currentBalance = 0;
        onBalanceChanged?.Invoke(currentBalance);
        Debug.Log($"[{gameObject.name}][BalanceController.ResetBalance] onBalanceChanged invoked with 0");
    }

    /// <summary>
    /// Áp dụng tất cả hiệu ứng của CardData lên bản thân.
    /// </summary>
    public void ApplyCardEffect(CardData card)
    {
        Debug.Log($"[{gameObject.name}][BalanceController.ApplyCardEffect] Applying '{card.cardName}'");

        // 1) Self shift
        if (card.selfBalanceShift != 0)
        {
            Debug.Log($"[{gameObject.name}][ApplyCardEffect] selfBalanceShift = {card.selfBalanceShift}");
            AddBalance(card.selfBalanceShift);
        }

        // 2) Reduce next push
        if (card.reducePushNextHit > 0)
        {
            nextPushReduction = card.reducePushNextHit;
            Debug.Log($"[{gameObject.name}][ApplyCardEffect] nextPushReduction set to {nextPushReduction}");
        }

        // 3) Invert
        if (card.invertBalance)
        {
            int old = currentBalance;
            currentBalance = -currentBalance;
            Debug.Log($"[{gameObject.name}][ApplyCardEffect] invertBalance: {old} -> {currentBalance}");
            onBalanceChanged?.Invoke(currentBalance);
        }

        // 4) Move towards center
        if (card.moveTowardsCenter > 0)
        {
            Debug.Log($"[{gameObject.name}][ApplyCardEffect] moveTowardsCenter = {card.moveTowardsCenter}");
            ReduceBalanceTowardZero(card.moveTowardsCenter);
        }

        // 5) Block next push
        if (card.blockAllPushNextHit)
        {
            blockNextPush = true;
            Debug.Log($"[{gameObject.name}][ApplyCardEffect] blockAllPushNextHit = true");
        }

        // 6) Reset
        if (card.resetBalance)
        {
            Debug.Log($"[{gameObject.name}][ApplyCardEffect] resetBalance = true");
            ResetBalance();
        }

        // 7) Skip next turn
        if (card.skipNextTurn)
        {
            Debug.Log($"[{gameObject.name}][ApplyCardEffect] skipNextTurn = true");
            var tm = FindFirstObjectByType<TurnManager>();
            if (tm != null)
            {
                tm.SetPlayerSkipNextTurn();
                Debug.Log($"[{gameObject.name}][ApplyCardEffect] Called TurnManager.SkipPlayerNextTurn()");
            }
        }
    }

    /// <summary>
    /// Nhận đòn đẩy, áp dụng block/reduction rồi AddBalance.
    /// </summary>
    public void ReceivePush(int pushAmount)
    {
        Debug.Log($"[BalanceController] {gameObject.name} ReceivePush: {pushAmount}");

        if (blockNextPush)
        {
            Debug.Log($"[{gameObject.name}][ReceivePush] blockNextPush consumed, no damage");
            blockNextPush = false;
            return;
        }

        if (nextPushReduction > 0)
        {
            Debug.Log($"[{gameObject.name}][ReceivePush] nextPushReduction = {nextPushReduction}");
            pushAmount -= nextPushReduction;
            nextPushReduction = 0;
            Debug.Log($"[{gameObject.name}][ReceivePush] pushAmount after reduction = {pushAmount}");
        }

        if (pushAmount != 0)
        {
            AddBalance(pushAmount);
        }
        else
        {
            Debug.Log($"[{gameObject.name}][ReceivePush] pushAmount is zero, no AddBalance called");
        }
    }
}
