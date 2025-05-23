using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerEvent : MonoBehaviour
{
    public UltEvents.UltEvent Event;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out MarioController _)) Event?.Invoke();
    }
}
