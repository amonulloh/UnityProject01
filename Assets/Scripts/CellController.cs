using UnityEngine;
using UnityEngine.UI;

public class CellController : MonoBehaviour
{
    public bool isAlive = false;
    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
        UpdateColor();
    }

    public void Toggle()
    {
        isAlive = !isAlive;
        UpdateColor();
    }

    public void SetAlive(bool state)
    {
        isAlive = state;
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (image == null) return;
        image.color = isAlive ? Color.green : Color.gray;
    }
}
