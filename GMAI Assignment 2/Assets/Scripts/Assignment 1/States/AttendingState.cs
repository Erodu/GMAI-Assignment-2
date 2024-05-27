using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttendingState : PotionMakerStates
{
    bool inquiredAboutPotions = false;
    public AttendingState(PotionMakerClass potionMaker)
    {
        // m_PotionMaker comes from PotionMakerStates.
        m_PotionMaker = potionMaker;
    }

    public override void Enter()
    {
        // Respond with the first Debug.Log here if the player has already gone through this and is coming to this state from the Transaction state.
        if (m_PotionMaker.inOneSession == true)
        {
            Debug.Log("'Anything else I can do for you?' The potion maker politely asks.");
            m_PotionMaker.btn_Inquire.SetActive(true);
            m_PotionMaker.btn_Leave.SetActive(true);
        }
        else
        {
            Debug.Log("'Hello! Welcome to The Kobold's Beaker, how may I help you?' The potion maker chirps.");
            m_PotionMaker.btn_Inquire.SetActive(true);
            m_PotionMaker.btn_Leave.SetActive(true);
        }
    }

    public override void Execute()
    {
        // The player has the choice now to inquire about potions, or to simply leave.
        if (inquiredAboutPotions == true)
        {
            m_PotionMaker.btn_Inquire.SetActive(false);
            m_PotionMaker.btn_Leave.SetActive(false);
            m_PotionMaker.ChangeState(new PotionInquiryState(m_PotionMaker));
        }
    }

    public override void Exit()
    {
        // Prints out a different line depending on if the player chose to inquire further, or leave.
        if (inquiredAboutPotions == true)
        {
            Debug.Log("You inquire more about what kind of potions there are available.");
        }
        else
        {
            Debug.Log("The potion maker bids you farewell.");
        }
    }

    public void Inquired()
    {
        inquiredAboutPotions = true;
    }

    public void LeaveShop()
    {
        m_PotionMaker.btn_Inquire.SetActive(false);
        m_PotionMaker.btn_Leave.SetActive(false);
        m_PotionMaker.inOneSession = false;
        if (m_PotionMaker.dirty == true)
        {
            m_PotionMaker.ChangeState(new CleaningState(m_PotionMaker));
        }
        else
        {
            m_PotionMaker.ChangeState(new IdleState(m_PotionMaker));
        }
    }
}
