using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransactionState : PotionMakerStates
{
    bool hasPaid = false;
    public TransactionState(PotionMakerClass potionMaker)
    {
        // m_PotionMaker comes from PotionMakerStates.
        m_PotionMaker = potionMaker;
    }

    public override void Enter()
    {
        Debug.Log("The potion maker presents the potion.\n" +
            "Alright! I'll just wait for the gold.");
        m_PotionMaker.btn_Pay.SetActive(true);
    }

    public override void Execute()
    {
        if (hasPaid == true)
        {
            Debug.Log("Thank you for your purchase!");
            // Set isOneSession to true, so that when going back to the Attending State the Potion Maker will respond with something else.
            m_PotionMaker.inOneSession = true;
            m_PotionMaker.btn_Pay.SetActive(false);
            m_PotionMaker.ChangeState(new AttendingState(m_PotionMaker));
        }
    }

    public override void Exit()
    {
        Debug.Log("You get the potion!");
    }

    public void PayForPotion()
    {
        hasPaid = true;
    }
}
