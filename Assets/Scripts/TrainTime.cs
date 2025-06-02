using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainTime : MonoBehaviour
{
    private int minIncrement = 1;
    PlayerInteraction interaction;

    private static int trainStop;
    // Start is called before the first frame update
    void Start()
    {
        interaction = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteraction>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeSinceLevelLoad > minIncrement * 60)
        {
            if (!interaction.IsSitting())
            {
                // GAME OVER
            }
            else
                minIncrement++;
        }
    }

    public static string GetTrainTime()
    {
        int mins = (int)Time.timeSinceLevelLoad % 60;
        int seconds = (int)Time.timeSinceLevelLoad - mins * 60;

        if (mins > 0)
            return mins + " minutes and " + seconds + " seconds";
        else
            return seconds + " seconds";
    }

    public static void Exit()
    {
        if (trainStop == 3)
        {
            // WIN GAME
        }
        else
        {
            // GAME OVER
        }
    }
}
