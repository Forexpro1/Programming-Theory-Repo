using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Example of Inheritance!!!
public class Lock : SpecialTile
{
    bool isDead = false;

    protected override void Update()
    {
        if (hitPoints <= 0)
        {
            if (!isDead)
            {
                isDead = true;
                if (goalManager != null)
                {
                    goalManager.CompareGoal(this.gameObject.tag);
                    goalManager.UpdateGoals();
                }
                Destroy(this.gameObject, 5f);
            } 
        }
    }

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
        rb.angularVelocity = Random.Range(-20 , 20);
        
    }
}
