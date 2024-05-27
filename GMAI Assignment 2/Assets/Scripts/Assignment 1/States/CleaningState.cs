using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleaningState : PotionMakerStates
{
    public CleaningState(PotionMakerClass potionMaker)
    {
        // m_PotionMaker comes from PotionMakerStates.
        m_PotionMaker = potionMaker;
    }

    public override void Enter()
    {
        Debug.Log("As you leave, the potion maker looks at her dirty workstation, deciding to clean it up.");
        m_PotionMaker.timerText.gameObject.SetActive(true);
        m_PotionMaker.StartCoroutine(CleaningTimer(10f));
    }

    public override void Execute()
    {
        // Since this is a waiting state, just uh yeah I guess.
    }

    public override void Exit()
    {
        Debug.Log("With that, the workplace looks good as new!");
    }

    private IEnumerator CleaningTimer(float duration)
    {
        float timer = duration;

        // Continue while there is still time.
        while (timer > 0)
        {
            timer -= Time.deltaTime;

            // To give the player feedback, we put out the time left. "0.0" rounds the result to one decimal's place.
            m_PotionMaker.timerText.text = "Time left for cleaning: " + timer.ToString("0.0");
            yield return null;
        }
        
        if (timer <= 0)
        {
            m_PotionMaker.timerText.gameObject.SetActive(false); // Disable the timer text.
            m_PotionMaker.dirty = false;
            m_PotionMaker.ChangeState(new IdleState(m_PotionMaker));
        }
    }
}
