using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour{

    protected Rigidbody2D rb;
    protected Collider2D myCollider;
    protected int scoreValue = 0;
    protected string myType;

    public bool activated = false;

    /* Use for item movement, sounds, etc. */
    public abstract void ItemBehavior();

    /* Called when item is picked up. */
    public abstract void PickUpItem(PlayerController player);

    public virtual void Start() {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.freezeRotation = true;
        myCollider = this.GetComponent<Collider2D>();
        myCollider.isTrigger = true;
        int itemLayer = LayerMask.NameToLayer("Item");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        Physics2D.IgnoreLayerCollision(itemLayer, enemyLayer, true);
    }

    public virtual void FixedUpdate() {
        ItemBehavior();
        //rb.AddForce(new Vector3(0, -300));
    }

    public int GetScore()
    {
        return scoreValue;
    }

    public string GetType()
    {
        return myType;
    } 
}
