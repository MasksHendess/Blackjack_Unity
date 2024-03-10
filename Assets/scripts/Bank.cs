using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bank : MonoBehaviour
{
    #region  setup Singelton pattern
    // only 1 instance of BuildManager in scene that is easy to acsess
    // Dont duplicate this region 
    public static Bank instance; //self reference
    private void Awake()
    {
        //check if instance already exisist
        if (instance != null)
        {
            Debug.LogError("More than one BuildManager in scene");
            return;
        }

        instance = this;
    }
    #endregion
    public int playerChips;
    public int bet;
    public Text chipsText;
    public Text betTxt;
    // Start is called before the first frame update
    void Start()
    {
        bet = 0;
    }

    // Update is called once per frame
    void Update()
    {
        chipsText.text = "Chips\n" + playerChips.ToString();
        betTxt.text = "Current Bet\n" + bet.ToString();
    }

    public void smallBet()
    {
        setBet(100);
    }
    public void mediumBet()
    {
        setBet(300);
    }
    public void bigBet()
    {
        setBet(500);
    }
    public void setBet(int amount)
    {
        if (bet + amount < playerChips + 1)
        {
            bet += amount;
          //  playerChips -= amount;
        }
    }
    public void blackJack()
    {
        bet = bet * 2;
        playerChips += bet;
        bet = 0;
    }
    public void modifyChips(bool playerWon)
    {
        if(playerWon)
        {
            playerChips += bet;
        }
        else 
        {
           playerChips -= bet;
        }

        bet = 0;
    }
}
