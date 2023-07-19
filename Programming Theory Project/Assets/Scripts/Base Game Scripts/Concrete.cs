using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Example of Inheritance!!!
public class Concrete : SpecialTile
{
    public Sprite concreteCrackSprite;
    private bool isCracked = false;

    // Example of Polymorphism!!!
    public override void TakeDamage(int damage)
    {
        hitPoints -= damage;
        if (!isCracked)
        {
            Crack();
            isCracked = true;
        } 
    }

    void Crack()
    {
        sprite.sprite = concreteCrackSprite;
    }
    void MakeLighter()
    {
        // take the current color
        Color color = sprite.color;
        // get the current color alpha value
        float newAlpha = color.a * .5f;
        sprite.color = new Color(color.r, color.g, color.b, newAlpha);


    }

}
