using UnityEngine;
using UnityEngine.UI;

public class EnergyController : MonoBehaviour
{
    [Header("Energy Settings")]
    [SerializeField] private int maxEnergy = 5;         // Energy tối đa tuyệt đối
    [SerializeField] private int currentMaxEnergy = 4;  // Energy khởi đầu, có thể tăng tối đa lên maxEnergy
    private int currentEnergy;

    [Header("Energy UI")]
    [SerializeField] private Image[] slots;
    [SerializeField] private Sprite emptySlot;
    [SerializeField] private Sprite fullSlot;

    public int CurrentEnergy => currentEnergy;
    public int CurrentMaxEnergy => currentMaxEnergy;

    private void Start()
    {
        ResetEnergy();
    }

    /// <summary>Giảm energy, trả về true nếu thành công</summary>
    public bool UseEnergy(int amount)
    {
        if (currentEnergy >= amount)
        {
            currentEnergy -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    /// <summary>Reset energy về full của currentMaxEnergy</summary>
    public void ResetEnergy()
    {
        currentEnergy = currentMaxEnergy;
        UpdateUI();
    }

    /// <summary>Tăng 1 energy slot (ví dụ qua item), tối đa maxEnergy</summary>
    public void GainEnergySlot()
    {
        if (currentMaxEnergy < maxEnergy)
        {
            currentMaxEnergy++;
            ResetEnergy();
        }
    }

    private void UpdateUI()
    {
        // Hiển thị đúng số slot = currentMaxEnergy, full hay empty theo currentEnergy
        for (int i = 0; i < slots.Length; i++)
        {
            bool activeSlot = (i < currentMaxEnergy);
            slots[i].gameObject.SetActive(activeSlot);

            if (!activeSlot) continue;

            slots[i].sprite = (i < currentEnergy) ? fullSlot : emptySlot;
        }
    }
}
