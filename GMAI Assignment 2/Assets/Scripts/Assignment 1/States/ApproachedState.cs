using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApproachedState : PotionMakerStates
{
    // This boolean will switch to true when the player talks to the potion maker.
    bool isTalkedTo = false;
    public ApproachedState(PotionMakerClass potionMaker)
    {
        // m_PotionMaker comes from PotionMakerStates.
        m_PotionMaker = potionMaker;
    }

    public override void Enter()
    {
        Debug.Log("As the potion maker sees a customer approach, she perks up, and begins to move over to the counter.");
        m_PotionMaker.btn_Talk.SetActive(true);
    }

    public override void Execute()
    {
        if (isTalkedTo == true)
        {
            // Logic for transitioning into the next state.
            m_PotionMaker.btn_Talk.SetActive(false);
            m_PotionMaker.ChangeState(new AttendingState(m_PotionMaker));
        }
        else
        {
            Debug.Log("The potion maker waits for the customer to engage.");
        }
    }

    public override void Exit()
    {
        Debug.Log("The potion maker is now being talked to.");
    }

    public void TalkToPotionMaker()
    {
        isTalkedTo = true;
    }
}
