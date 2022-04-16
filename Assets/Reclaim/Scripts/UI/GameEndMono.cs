using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndMono : MonoBehaviour
{
    public GameObject FadeScreen
    {
        get
        {
            return GameObject.FindObjectOfType<FadeInMono>(true).gameObject;
        }
    }
    public GameObject EndingScrollText
    {
        get
        {
            return GameObject.FindObjectOfType<ScrollTextMono>(true).gameObject;
        }
    }
    
    string WinningText
    {
        get
        {
            var players = Services.PlayerManagerService.GetAllPlayers();
            string playerNameList = "";
            if (players.Count == 1)
                playerNameList = players[0].Name;
            else if(players.Count == 2)
                playerNameList = $"{players[0].Name} and {players[1].Name}";
            else
            {
                for(int i = 0; i < players.Count; i++)
                {
                    if (i == players.Count - 1)
                        playerNameList += "and ";

                    playerNameList += players[i].Name + ", ";
                }
                playerNameList = playerNameList.TrimEnd(' ', ',');
            }

            bool singleHero = players.Count == 1; 
            string hero = singleHero ? "hero" : "heroes";
            string has = singleHero ? "has" : "have";

            string sorryForYourLoss = singleHero ? "Sorry for you loss, by the way.  I hope that's not too soon. " : "";

            return $"Gods above!  Our mighty {hero} {has} done it! {sorryForYourLoss}I can't say I'm that surprised, you did very well. For the most part. " +
                    $"Sorry, what I mean is congradulations!  I always knew that {playerNameList} could do it.  Wow.  Great character varity there... " +
                    $"\n\nWell done!  The halls of Goltarren are freed from the " +
                    $"evil that once desecrated them.  We owe everything to you.  You've given the gods back something most sacred, their favorite brunch spot. " +
                    $"\n\n" +
                    $"What?  The gods, as they are, deserve a nice brunch spot. They're very busy, you know. The prayers of the weak and helpless don't ignore themselves. " +
                    $"It takes quite a lot of effort to smite people for no good reason, I'll have you know.";
        }
    }

    string LossText
    {
        get
        {
            return "And so, as it had been many times before, the doors of Goltarren have sealed themselves for another millenia. " +
                    "Evil shall continue to go unhindered in these once hallowed halls.\n\n" +

                    "I really thought you had it there for a minute.  I mean, not that last minute, obviously. Alas, it's how these things go, I suppose. " +
                    "You gave it your best shot, I'm sure.  That's really all we can hope for. \n\n " +
                    
                    "I'm very proud of you, honest.  I'd probably be more proud if you'd " +
                    "actually managed to free Goltarren, the great and mystical chambers of the gods, from evil.  But, that's alright.  Don't worry too much. " +
                    "You did a great job... really.";
        }
    }

    public void EnableEndState(bool victory)
    {
        FadeScreen.SetActive(true);
        EndingScrollText.GetComponent<TextMeshProUGUI>().text = victory ? WinningText : LossText;
        EndingScrollText.SetActive(true);
    }

    public void CleanGameObjects()
    {
        foreach(var name in Singleton.Singletons)
            GameObject.Destroy(GameObject.Find(name));

        Singleton.Singletons.Clear();
        Room.CleanStaticData();
    }
}
