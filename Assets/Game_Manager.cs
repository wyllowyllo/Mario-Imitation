using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game_Manager : MonoBehaviour
{

    public int Total_Point;
    public int Stage_Point;
    public int Stage_Index;
    public int health;
    public Player_Move player;
    public GameObject[] Stages;
    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject Restart_Button;


    private void Update()
    {
        UIPoint.text = (Total_Point + Stage_Point).ToString();
    }

    public void NextStage()
    {

        //Stage change
        if (Stage_Index < Stages.Length - 1)
        {
            Stages[Stage_Index].SetActive(false);
            Stage_Index++;
            Stages[Stage_Index].SetActive(true);
            PlayerReposition();

            UIStage.text = "Stage " + (Stage_Index+1).ToString();
        }
        else
        {
            //Game clear

            //Player control lock
            Time.timeScale = 0;
            //Result UI
            Debug.Log("게임 클리어!");
            //Restart Button UI
            Text BtnText = Restart_Button.GetComponentInChildren<Text>();
            BtnText.text = "Clear!";
            Restart_Button.SetActive(true);
            
        }

        //Calculate points
        Total_Point += Stage_Point;
        Stage_Point = 0;
    }

    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
            UIhealth[health].color = new Color(1, 0, 0, 0.4f);
        }
        else
        {
            //All Health UI off
            UIhealth[0].color = new Color(1, 0, 0, 0.4f);

            //player die effect
            player.OnDie();


            //retry button UI
            Time.timeScale = 0;
            Restart_Button.SetActive(true);
            
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            HealthDown();

            if (health >=1)
            {
                PlayerReposition();
            }
          
        }
           
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(-17, 2, -1);
        player.velocityZero();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

}
