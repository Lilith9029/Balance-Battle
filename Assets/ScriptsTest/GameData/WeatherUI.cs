using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
public class WeatherUI : MonoBehaviour
{
    public GameObject weatherIconPrefab; // prefab có chứa 1 Image
    public Transform iconContainer;

    public void SetWeatherIcons(List<Sprite> icons)
    {
        // Xoá hết icon cũ
        foreach (Transform child in iconContainer)
            Destroy(child.gameObject);

        // Thêm icon mới nếu có
        if (icons.Count == 0)
        {
            iconContainer.gameObject.SetActive(false);
            return;
        }

        iconContainer.gameObject.SetActive(true);
        foreach (var sprite in icons)
        {
            GameObject icon = Instantiate(weatherIconPrefab, iconContainer);
            icon.GetComponent<Image>().sprite = sprite;
        }
    }
}
