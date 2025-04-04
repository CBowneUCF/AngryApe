using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioSpriteManager : MonoBehaviour
{
    public SpriteRenderer sprite;

    public Sprite groundedSprite;
    public Sprite airborneSprite;
    public Sprite climbingSprite;
    public Sprite deadSprite;

    public void SetFlip(bool flipped) => sprite.flipX = flipped;

    public void GoGround() => sprite.sprite = groundedSprite;
    public void GoAir() => sprite.sprite = airborneSprite;
    public void GoClimb() => sprite.sprite = climbingSprite;
    public void GoDied() => sprite.sprite = deadSprite;
}
