using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Move : MonoBehaviour
{

    Rigidbody2D rigid;
    Animator anime;
    SpriteRenderer spriterenderer;
    BoxCollider2D collider;

    public int nextMove;
    public float nextThinkTime;
    //public float WalkSpeed=1.5f;
    // Start is called before the first frame update
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anime = GetComponent<Animator>();
        spriterenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();

        Invoke("Think", 5);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //platform check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove*0.2f,rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.down,1,LayerMask.GetMask("Platform"));

        if (rayHit.collider == null)
        {
            Debug.Log("Warning!! it's cliff front there!!");
            Turn();
        }
    }

    void Think()
    {
        //Set next active
        nextMove = Random.Range(-1, 2);

        //Flip sprite
        if (nextMove != 0)
            spriterenderer.flipX = nextMove == 1;

        //Sprite animation
        anime.SetInteger("WalkSpeed", nextMove);

 
        //Recursive
        nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);
    }

    void Turn()
    {
        nextMove = nextMove *= -1;
        //Flip sprite
        if (nextMove != 0)
            spriterenderer.flipX = nextMove == 1;
        CancelInvoke();
        Invoke("Think", 2);
    }

    public void OnDamaged()
    {
        //Sprite Alpha
        spriterenderer.color = new Color(1, 1, 1, 0.4f);
        //Sprite Filp Y
        spriterenderer.flipY = true;
        //Collider Disable
        collider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up*5, ForceMode2D.Impulse);
        //Destroy
        Invoke("DeActive", 5);

    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}
