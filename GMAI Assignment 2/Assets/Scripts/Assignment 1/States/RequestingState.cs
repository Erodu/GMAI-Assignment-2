using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestingState : PotionMakerStates
{
    bool gaveWyvernSpike = false;
    bool didNotGiveSpike = false;
    public RequestingState(PotionMakerClass potionMaker)
    {
        // m_PotionMaker comes from PotionMakerStates.
        m_PotionMaker = potionMaker;
    }

    public override void Enter()
    {
        Debug.Log("[EXPAND FOR FULL TEXT] 'Well, it's a Potion of Wyvern Poison Resistance, and I can guarantee success with brewing it. The problem is that I don't have a key ingredient: a spike from a Wyvern's Tail.'\n" +
            "She looks at your pouch, seeing a very familiar sight. '...But it seems that somehow you have one? I mean, you're an adventurer so I guess it wouldn't be uncommon for you to have it. Either way, I can make one for you, but I'd need your wyvern spike! How about it? I'll even give a discount.'");

        m_PotionMaker.btn_Give.SetActive(true);
        m_PotionMaker.btn_NoGive.SetActive(true);
    }

    public override void Execute()
    {
        if (gaveWyvernSpike == true)
        {
            m_PotionMaker.btn_Give.SetActive(false);
            m_PotionMaker.btn_NoGive.SetActive(false);
            m_PotionMaker.ChangeState(new BrewingState(m_PotionMaker));
        }
        else if (didNotGiveSpike == true)
        {
            m_PotionMaker.btn_Give.SetActive(false);
            m_PotionMaker.btn_NoGive.SetActive(false);
            m_PotionMaker.ChangeState(new AttendingState(m_PotionMaker));
        }
    }

    public override void Exit()
    {
        if (gaveWyvernSpike == true)
        {
            Debug.Log("You hand the spike over.");
        }
        else if (didNotGiveSpike == true)
        {
            Debug.Log("You chose to not hand the spike over.");
        }
    }

    public void GiveWyvernSpike()
    {
        gaveWyvernSpike = true;
    }

    public void DoNotGiveSpike()
    {
        didNotGiveSpike = true;
    }
}
