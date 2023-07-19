using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTile : MonoBehaviour
{
    public int hitPoints;
    protected SpriteRenderer sprite;
    protected GoalManager goalManager;
    protected Rigidbody2D rb;

    private void Start()
    {
        goalManager = FindObjectOfType<GoalManager>();
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (hitPoints <= 0)
        {
            if (goalManager != null)
            {
                goalManager.CompareGoal(this.gameObject.tag);
                goalManager.UpdateGoals();
            }
            Destroy(this.gameObject,5);
        }
    }
    public virtual void TakeDamage(int damage)
    {
        hitPoints -= damage;
       
    }
    
}
