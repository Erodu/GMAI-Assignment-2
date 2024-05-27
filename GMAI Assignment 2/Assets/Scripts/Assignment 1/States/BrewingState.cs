using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrewingState : PotionMakerStates
{
    bool brewFailed = false;
    bool brewSuccessful = false;
    public BrewingState(PotionMakerClass potionMaker)
    {
        // m_PotionMaker comes from PotionMakerStates.
        m_PotionMaker = potionMaker;
    }

    public override void Enter()
    {
        Debug.Log("'Alright then! Wait here while I do my magic!' The potion maker begins her careful process.");
        m_PotionMaker.timerText.gameObject.SetActive(true);
        m_PotionMaker.StartCoroutine(BrewingTimer(10f));
        m_PotionMaker.dirty = true;
    }

    public override void Execute()
    {
        if (brewSuccessful == true)
        {
            m_PotionMaker.ChangeState(new TransactionState(m_PotionMaker));
        }
        else if (brewFailed == true)
        {
            m_PotionMaker.ChangeState(new FailedState(m_PotionMaker));
        }
    }

    public override void Exit()
    {
        if (brewSuccessful == true)
        {
            Debug.Log("The brewing process is successful!");
        }
        else if (brewFailed == true)
        {
            Debug.Log("The brewing process was unsuccessful.");
        }
    }

    private IEnumerator BrewingTimer(float duration)
    {
        float timer = duration;

        // Continue while there is still time.
        while (timer > 0)
        {
            timer -= Time.deltaTime;

            // To give the player feedback, we put out the time left. "0.0" rounds the result to one decimal's place.
            m_PotionMaker.timerText.text = "Time left for brewing: " + timer.ToString("0.0");
            yield return null;
        }
        // After the timer is done, we roll for success.
        if (timer <= 0)
        {
            m_PotionMaker.timerText.gameObject.SetActive(false); // Disable the timer text.
            if (m_PotionMaker.isThirdPotion == true)
            {
                brewSuccessful = true; // Automatically set it to true if the player is making the third potion.
                // Make sure that the isThirdPotion bool does not remain true for the rest of this bot's runtime.
                m_PotionMaker.isThirdPotion = false;
            }
            else
            {
                DecideSuccess();
            }
        }
    }

    private void DecideSuccess()
    {
        // Random check to see if the brewing process is successful or not. Getting anything more than 2 is a success, but otherwise triggers a failstate.
        int brewCheck = Random.Range(0, 11);

        if (brewCheck > 2f)
        {
            brewSuccessful = true;
        }
        else
        {
            brewFailed = true;
        }
    }
}
