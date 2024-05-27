using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckComponentsState : PotionMakerStates
{
    bool choseToProceed = false;
    bool choseBack = false;
    bool checkCompleted = false;
    public CheckComponentsState(PotionMakerClass potionMaker)
    {
        // m_PotionMaker comes from PotionMakerStates.
        m_PotionMaker = potionMaker;
    }

    public override void Enter()
    {
        Debug.Log("'Okay, now give me a bit as I check the component stock!' The potion maker hurries over to the shelves.");
        m_PotionMaker.StartCoroutine(CheckComponentTimer(5f));
    }

    public override void Execute()
    {
        if (checkCompleted == true)
        {
            Debug.Log("Okay, I have all the components needed. Just one final confirmation before I start the brewing process, is this potion what you want?");
        }
        if (choseToProceed == true)
        {
            m_PotionMaker.btn_Proceed.SetActive(false);
            m_PotionMaker.btn_Back.SetActive(false);
            m_PotionMaker.ChangeState(new BrewingState(m_PotionMaker));
        }
        else if (choseBack == true)
        {
            m_PotionMaker.inOneSession = true;
            m_PotionMaker.btn_Proceed.SetActive(false);
            m_PotionMaker.btn_Back.SetActive(false);
            m_PotionMaker.ChangeState(new AttendingState(m_PotionMaker));
        }
    }

    public override void Exit()
    {
        if (choseToProceed == true)
        {
            Debug.Log("You chose to proceed.");
        }
        else if (choseBack == true)
        {
            Debug.Log("You chose to go back.");
        }
    }

    private IEnumerator CheckComponentTimer(float duration)
    {
        float timer = duration;

        // Continue while there is still time.
        while (timer > 0)
        {
            timer -= Time.deltaTime;

            // To give the player feedback, we put out the time left. "0.0" rounds the result to one decimal's place.
            m_PotionMaker.timerText.text = "Time left for component check: " + timer.ToString("0.0");
            yield return null;
        }
        // After the timer is done, change state to Check Component State.
        if (timer <= 0)
        {
            checkCompleted = true;
            m_PotionMaker.timerText.gameObject.SetActive(false); // Disable the timer text.
            m_PotionMaker.btn_Proceed.SetActive(true);
            m_PotionMaker.btn_Back.SetActive(true);
        }
    }

    public void ChooseProceedOption()
    {
        choseToProceed = true;
    }

    public void ChooseBackOption()
    {
        choseBack = true;
    }
}
