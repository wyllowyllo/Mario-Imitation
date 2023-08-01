using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
    // Start is called before the first frame update

    Rigidbody2D rigid;
    SpriteRenderer spriterenderer;
    Animator anime;
    AudioSource audio;
    public Game_Manager gamemanager;
    public float Max_speed;
    public float Jump_power;
    private Color oriColor;
    bool Double_Jump = false;

    //All Sound sources
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriterenderer = GetComponent<SpriteRenderer>();
        anime = GetComponent<Animator>();
        oriColor = spriterenderer.color;
        audio = GetComponent<AudioSource>();
    }


    private void Update() //�ܹ����� Ű �Է� �� update���� ó���ϴ� ���� ����
    {
        //stop speed
        if (Input.GetButtonUp("Horizontal"))
            rigid.velocity = new Vector2(rigid.velocity.normalized.x*0.5f, rigid.velocity.y);

        if(Input.GetButton("Horizontal"))
            spriterenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            anime.SetBool("IsWalking", false);
        else
            anime.SetBool("IsWalking", true);

        //Jump
        if (Input.GetButtonDown("Jump") && !anime.GetBool("IsJumping"))
        {

            rigid.AddForce(Vector2.up * Jump_power, ForceMode2D.Impulse);
            anime.SetBool("IsJumping", true);
            Double_Jump = false;
            PlaySound("JUMP");
        }
        else if (Input.GetButtonDown("Jump") && !Double_Jump&&anime.GetBool("IsJumping"))
        {
            rigid.AddForce(Vector2.up * (Jump_power*0.7f), ForceMode2D.Impulse);
            anime.SetBool("IsJumping", true);
            Double_Jump = true;
            PlaySound("JUMP");
        }

        

    }

    private void FixedUpdate() // ������ update �� fixedupdate ���
    {
        //move speed
        float h_move = Input.GetAxisRaw("Horizontal");
       


        rigid.AddForce(Vector2.right * h_move, ForceMode2D.Impulse);

        if (rigid.velocity.x > Max_speed)
            rigid.velocity = new Vector2(Max_speed, rigid.velocity.y);

        else if (rigid.velocity.x < Max_speed * (-1))
            rigid.velocity = new Vector2(Max_speed * (-1), rigid.velocity.y);




        if (rigid.velocity.y < 0)
        {
            //Landing Platform
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0)); //sceneȭ�鿡�� ray �׸���(����ȭ�鿡���� ������ ����)

            //���� ���� ���� ��� ���� ����
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            //���� ���� �´´ٸ�, rayhit�� �ʱ�ȭ�Ǿ� collider�� ����ִ�
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                    anime.SetBool("IsJumping", false);
            }
        }

    }

     void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
          
            //Attack logic
            if(rigid.velocity.y<0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
                gamemanager.Stage_Point += 100;
               
            }
            else
            {
                Debug.Log("���� �浹�߽��ϴ�!!");
                OnDamaged(collision.transform.position);
                //anime.SetBool("IsJumpoing", false);
               
                Invoke("OffDamaged", 1);
            }


        }
    }

     void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {

            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");


            if(isBronze)
                gamemanager.Stage_Point += 50;
            else if(isSilver)
                gamemanager.Stage_Point += 100;
            else if(isGold)
                gamemanager.Stage_Point += 300;

            collision.gameObject.SetActive(false);
            PlaySound("ITEM");
        }

        else if (collision.gameObject.tag == "Finish")
        {
            //Next Stage
            gamemanager.NextStage();
            PlaySound("FINISH");
        }
    }

    void OnDamaged(Vector2 targetPos)
    {
        //health discount
        gamemanager.HealthDown();

        //layer changed - playerDamaged
        gameObject.layer = 8;


        //player color change
        spriterenderer.color = new Color(1, 0, 0, 0.3f);

        //knockback
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1)*7, ForceMode2D.Impulse);

        anime.SetTrigger("OnDamaged");
        PlaySound("DAMAGED");
    }

    void OffDamaged()
    {
        //return to original state
        gameObject.layer = 3;
        spriterenderer.color = oriColor;
    }

    void OnAttack(Transform enemy)
    {
        //Point


        //Reaction Force
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        // Enemy Die
        Enemy_Move enemyMove = enemy.GetComponent<Enemy_Move>();
        enemyMove.OnDamaged();
        PlaySound("ATTACK");
    }

    public void OnDie()
    {
        //Sprite Alpha
        spriterenderer.color = new Color(1, 1, 1, 0.4f);
        //Sprite Filp Y
        spriterenderer.flipY = true;
        //Collider Disable
        
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        PlaySound("DIE");
    }

    public void velocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audio.clip = audioJump;
                audio.Play();
                break;
            case "ATTACK":
                audio.clip = audioAttack;
                audio.Play();
                break;
            case "DAMAGED":
                audio.clip = audioDamaged;
                audio.Play();
                break;
            case "ITEM":
                audio.clip = audioItem;
                audio.Play();
                break;
            case "DIE":
                audio.clip = audioDie;
                audio.Play();
                break;
            case "FINISH":
                audio.clip = audioFinish;
                audio.Play();
                break;
        }
    }
}
