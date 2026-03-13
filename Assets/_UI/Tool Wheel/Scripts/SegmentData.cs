using UnityEngine;
using UnityEngine.UI;

public class SegmentData : MonoBehaviour
{
    [field: SerializeField] public Image Image {get; private set;}
    [field: SerializeField] public ToolType ToolType  {get; private set; }

    public void SetColor(Color color)
    {
        Image.color = color;
    }
}
