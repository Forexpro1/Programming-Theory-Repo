using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Example of Inheritance!!!
public class Lock : SpecialTile
{
  
    // Example of Polymorphism!!!
    public override void TakeDamage(int damage)
    {
        hitPoints -= damage;
        EnableRBSimulation();
        
    }
    void EnableRBSimulation()
    {
        rb.simulated = true;
        Vector2 randomForceVector  = new Vector2(Random.Range(-5,5), Random.Range(-3, 10));
        rb.AddForce(randomForceVector,ForceMode2D.Impulse);
        rb.angularVelocity = Random.Range(-10, 10);
        
    }
}
