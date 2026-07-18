using UnityEngine;
using UnityEngine.EventSystems;

public enum BallColor
{
    White,
    Red,
    Yellow,
    Green,
    Brown,
    Blue,
    Pink,
    Black
}

public class Ball : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private int point;

    [SerializeField]
    private BallColor color;


    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(point);
        GameManger.instance.PlayerScore += point;
        Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ApplyColor();
    }

    // ทำงานทุกครั้งที่แก้ค่าใน Inspector ทำให้เห็นสีทันทีโดยไม่ต้องกด Play
    void OnValidate()
    {
        ApplyColor();
    }

    void ApplyColor()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend == null) return;

        // ใช้ MaterialPropertyBlock เพื่อเปลี่ยนสีรายลูกโดยไม่สร้าง material ใหม่
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        rend.GetPropertyBlock(block);
        block.SetColor("_BaseColor", GetColor(color));
        rend.SetPropertyBlock(block);
    }

    Color GetColor(BallColor ballColor)
    {
        switch (ballColor)
        {
            case BallColor.White: return new Color(1f, 1f, 1f);
            case BallColor.Red: return new Color(0.85f, 0.05f, 0.05f);
            case BallColor.Yellow: return new Color(1f, 0.8f, 0.05f);
            case BallColor.Green: return new Color(0.05f, 0.4f, 0.1f);
            case BallColor.Brown: return new Color(0.42f, 0.24f, 0.1f);
            case BallColor.Blue: return new Color(0.1f, 0.25f, 0.85f);
            case BallColor.Pink: return new Color(1f, 0.5f, 0.65f);
            case BallColor.Black: return new Color(0.03f, 0.03f, 0.03f);
            default: return Color.white;
        }
    }


}
