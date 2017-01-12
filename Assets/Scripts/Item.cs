using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour{

    //This should be a 'Block' object though.
    GameObject myBlock;
    protected Rigidbody2D rb;
    protected Collider2D myCollider;

    public bool activated = false;

    /* Use for item movement, sounds, etc. */
    public abstract void ItemBehavior();

    /* Called when item is picked up. */
    public abstract void PickUpItem(PlayerController player);

    public virtual void Start() {
        //Set myBlock automatically.
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.freezeRotation = true;
        myCollider = this.GetComponent<Collider2D>();
        myCollider.isTrigger = true;
    }

    public virtual void FixedUpdate() {
        ItemBehavior();
        //rb.AddForce(new Vector3(0, -300));
    }
}
