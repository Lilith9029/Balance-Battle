using UnityEngine;

public class BalanceArrowController : MonoBehaviour
{
    [Header("References")]
    public RectTransform arrow;             // mũi tên trắng
    public RectTransform barBackground;     // thanh màu nền
    public BalanceController balanceController; // sẽ tự gán nếu chưa có

    private int maxBalance;

    private void Start()
    {
        // Nếu không gán thủ công trong Inspector, tự tìm GameObject tagged "Player"
        if (balanceController == null)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null)
                balanceController = player.GetComponent<BalanceController>();
        }

        if (balanceController == null)
        {
            Debug.LogError("[BalanceArrowController] Không tìm thấy BalanceController của Player!");
            return;
        }

        maxBalance = balanceController.maxBalance;

        // Đăng ký sự kiện mỗi khi balance thay đổi
        balanceController.onBalanceChanged.AddListener(UpdateArrowPosition);

        // Cập nhật lần đầu
        UpdateArrowPosition(balanceController.currentBalance);
    }

    private void OnDestroy()
    {
        if (balanceController != null)
            balanceController.onBalanceChanged.RemoveListener(UpdateArrowPosition);
    }

    private void UpdateArrowPosition(int currentBalance)
    {

        Debug.Log($"[BalanceArrowController] Update UI → Balance = {currentBalance}");

        Debug.Log($"[BalanceArrowController] Cập nhật UI Balance = {currentBalance}");  // Debug xem UI có nhận event không

        // Clamp lại nếu có lỗi
        currentBalance = Mathf.Clamp(currentBalance, -maxBalance, maxBalance);

        // Tính tỉ lệ 0 → 1
        float normalized = (currentBalance + maxBalance) / (2f * maxBalance);
        Debug.Log($"[BalanceArrowController] normalized = {normalized}");

        // Tính khoảng dịch
        float barWidth = barBackground.rect.width;
        float newX = (normalized - 0.5f) * barWidth;

        // Debug vị trí mới
        Debug.Log($"[BalanceArrowController] newX = {newX}");

        // Update vị trí
        arrow.anchoredPosition = new Vector2(newX, arrow.anchoredPosition.y);
    }
}
