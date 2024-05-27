using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailedState : PotionMakerStates
{
    bool choseToContinue = false;
    bool choseToAbandon = false;
    public FailedState(PotionMakerClass potionMaker)
    {
        // m_PotionMaker comes from PotionMakerStates.
        m_PotionMaker = potionMaker;
    }

    public override void Enter()
    {
        Debug.Log("'Dang... Could have sworn that I was gonna be successful, but this potion is experimental. If you want, I can retry it, or you can choose something else.'");
        m_PotionMaker.btn_Continue.SetActive(true);
        m_PotionMaker.btn_Abandon.SetActive(true);
    }

    public override void Execute()
    {
        if (choseToContinue == true)
        {
            // If the player chooses to continue, go back to Brewing State.
            m_PotionMaker.btn_Continue.SetActive(false);
            m_PotionMaker.btn_Abandon.SetActive(false);
            m_PotionMaker.ChangeState(new BrewingState(m_PotionMaker));
        }
        else if (choseToAbandon == true)
        {
            // If the player chooses to abandon, go back to Attending State.
            m_PotionMaker.btn_Continue.SetActive(false);
            m_PotionMaker.btn_Abandon.SetActive(false);
            m_PotionMaker.inOneSession = true;
            m_PotionMaker.ChangeState(new AttendingState(m_PotionMaker));
        }
    }

    public override void Exit()
    {
        if (choseToContinue == true)
        {
            Debug.Log("You chose to continue.");
        }
        if (choseToAbandon == true)
        {
            Debug.Log("You chose to abandon.");
        }
    }

    public void ContinueBrewing()
    {
        choseToContinue = true;
    }

    public void AbandonBrewing()
    {
        choseToAbandon = true;
    }
}
