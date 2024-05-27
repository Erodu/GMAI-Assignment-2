using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionInquiryState : PotionMakerStates
{
    bool choseHealing = false;
    bool choseArcane = false;
    bool choseThird = false;
    public PotionInquiryState(PotionMakerClass potionMaker)
    {
        // m_PotionMaker comes from PotionMakerStates.
        m_PotionMaker = potionMaker;
    }

    public override void Enter()
    {
        Debug.Log("[EXPAND FOR FULL TEXT] 'I have about three potions for today, but only two types are readily available.'\n" +
            "'First off, there's our healing potions, which are prepared already and have a flat fee of 10 gold pieces each!'\n" +
            "'Then we have a rather new one! A Potion of Arcane Excellence. But since it's new, I'd have to check if I actually have the components, and refresh myself on how to make it.'\n" +
            "'The third one... Well, I know for sure I'm missing something for that one.'");

        m_PotionMaker.btn_Healing.SetActive(true);
        m_PotionMaker.btn_Arcane.SetActive(true);
        m_PotionMaker.btn_ThirdPotion.SetActive(true);
    }

    public override void Execute()
    {
        if (choseHealing == true)
        {
            m_PotionMaker.btn_Healing.SetActive(false);
            m_PotionMaker.btn_Arcane.SetActive(false);
            m_PotionMaker.btn_ThirdPotion.SetActive(false);
            m_PotionMaker.ChangeState(new TransactionState(m_PotionMaker));
        }
        else if (choseArcane == true)
        {
            m_PotionMaker.btn_Healing.SetActive(false);
            m_PotionMaker.btn_Arcane.SetActive(false);
            m_PotionMaker.btn_ThirdPotion.SetActive(false);
            m_PotionMaker.ChangeState(new StudyingState(m_PotionMaker));
        }
        else if (choseThird == true)
        {
            m_PotionMaker.btn_Healing.SetActive(false);
            m_PotionMaker.btn_Arcane.SetActive(false);
            m_PotionMaker.btn_ThirdPotion.SetActive(false);
            m_PotionMaker.ChangeState(new RequestingState(m_PotionMaker));
        }
    }

    public override void Exit()
    {
        if (choseHealing == true)
        {
            Debug.Log("You chose the healing potion.");
        }
        else if (choseArcane == true)
        {
            Debug.Log("You chose the Potion of Arcane Excellence.");
        }
        else if (choseThird == true)
        {
            Debug.Log("You chose to inquire further about the third potion.");
        }
    }

    public void ChooseHealingPotion()
    {
        choseHealing = true;
    }

    public void ChooseArcanePotion()
    {
        choseArcane = true;
    }

    public void ChooseThirdPotion()
    {
        choseThird = true;
    }
}
