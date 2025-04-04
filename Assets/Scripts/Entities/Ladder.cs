using EditorAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    public float topEdge;
    public float bottomEdge;
    public bool topAtPlatform = true;
    public bool bottomAtPlatform = true;

    [OnValueChanged(nameof(SetValues))]
    public int pixelCount;
    [Button]
    public void SetValues()
    {
        if (!TryGetComponent(out BoxCollider2D box)) return;

        bottomEdge = transform.position.y;
        topEdge = transform.position.y + (pixelCount * .125f);
        box.size = new Vector2(box.size.x, pixelCount * .125f);
        box.offset = new(0, box.size.y * .5f);
    }
    [Button]
    public void MovePixelUp() => transform.position = transform.position + Vector3.up * .125f;
    [Button]
    public void MovePixelDown() => transform.position = transform.position - Vector3.up * .125f;

    [Button]
    public void SetUpperArea()
    {
        if (!TryGetComponent(out BoxCollider2D box)) return;
        box.size = new Vector2(box.size.x, box.size.y+(4*.125f));
        box.offset = new(0, box.size.y * .5f);
    }
}
