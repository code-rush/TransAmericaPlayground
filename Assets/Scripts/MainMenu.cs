using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public static ArrayList Playertype = new ArrayList();
    public static Dictionary<int, string> BotConfig;

    public static string humanPlayersName = "";

    public InputField player1;
    public Dropdown player2;
    public Dropdown player3;
    public Dropdown player4;
    public Dropdown player5;
    public Dropdown player6;


    public void playGame()
    {
        BotConfig = new Dictionary<int, string>();

        //if (player1.options[player1.value].text != null)
        //{
        //    if (player1.options[player1.value].text == "PLAYER")

        //        Playertype.Add("human");

        //    else if (player1.options[player1.value].text == "BOT")

        //        Playertype.Add("bot");
        //}
        Playertype = new ArrayList();
        Playertype.Insert(0, "human");

        if (player1.text != "")
        {
            humanPlayersName = player1.text;
        }

        if (player2.options[player2.value].text != null)
        {
            if (player2.options[player2.value].text == "Bot-Easy")
            {
                Playertype.Add("bot");
                BotConfig[1] = "easy";
            } else if (player2.options[player2.value].text == "Bot-Medium")
            {
                Playertype.Add("bot");
                BotConfig[1] = "medium";
            }

        }

        if (player3.options[player3.value].text != null)
        {
            if (player3.options[player3.value].text == "Bot-Easy")
            {
                Playertype.Add("bot");
                BotConfig[2] = "easy";
            }
            else if (player3.options[player3.value].text == "Bot-Medium")
            {
                Playertype.Add("bot");
                BotConfig[2] = "medium";
            }
        }

        if (player4.options[player4.value].text != null)
        {
            if (player4.options[player4.value].text == "Bot-Easy")
            {
                Playertype.Add("bot");
                BotConfig[3] = "easy";
            }
            else if (player4.options[player4.value].text == "Bot-Medium")
            {
                Playertype.Add("bot");
                BotConfig[3] = "medium";
            }
        }

        if (player5.options[player5.value].text != null)
        {
            if (player5.options[player5.value].text == "Bot-Easy")
            {
                Playertype.Add("bot");
                BotConfig[4] = "easy";
            }
            else if (player5.options[player5.value].text == "Bot-Medium")
            {
                Playertype.Add("bot");
                BotConfig[4] = "medium";
            }
        }

        if (player6.options[player6.value].text != null)
        {
            if (player6.options[player6.value].text == "Bot-Easy")
            {
                Playertype.Add("bot");
                BotConfig[5] = "easy";
            }
            else if (player6.options[player6.value].text == "Bot-Medium")
            {
                Playertype.Add("bot");
                BotConfig[5] = "medium";
            }
        }

        //Debug.Log(player1.options[player1.value].text);
        //Debug.Log(player2.options[player2.value].text);
        //Debug.Log(player3.options[player3.value].text);
        //Debug.Log(player4.options[player4.value].text);
        //Debug.Log(player5.options[player5.value].text);
        //Debug.Log(player6.options[player6.value].text);
        //Debug.Log(player1.text);

        //Debug.Log(Playertype[0]);
        //Debug.Log(Playertype[1]);
        //Debug.Log(BotConfig[1]);


        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
