using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card")]
public class CardData : ScriptableObject
{
    [Header("Basic Info")]
    public string cardName;
    public Sprite cardSprite;
    [TextArea] public string cardDescription;

    [Header("Effect")]
    public int energyCost = 0;                  // Năng lượng tiêu tốn
    public int selfBalanceShift = 0;            // Lực đẩy tác động ngược lại bản thân
    public int targetBalanceShift = 0;          // Lực đẩy tấn công đối thủ

    [Header("Extra Effects")]
    public int reducePushNextHit = 0;           // Giảm lực đẩy đòn kế tiếp
    public bool invertBalance = false;          // Đảo ngược balance
    public int moveTowardsCenter = 0;           // Dịch về trung tâm X điểm
    public bool blockAllPushNextHit = false;    // Block 100% lực đẩy
    public bool resetBalance = false;           // Reset balance về 0
    public bool skipNextTurn = false;           // Mất lượt kế tiếp

    [Header("Direction")]
    public bool canChooseDirection = false;     // cho phép chọn hướng tấn công
    public Vector2 direction;                   // hướng tấn công mặc định x = 1 => right, x = -1 = left, y = 1 = up, y = -1 = down

    [Header("Card Type")]
    public CardType cardType;

    public enum CardType
    {
        Attack_Red,
        Defense_Green,
        Weather_Blue,
        Support_Yelow,
        Special_Purple,
    }
}
