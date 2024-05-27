using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudyingState : PotionMakerStates
{
    public StudyingState(PotionMakerClass potionMaker)
    {
        // m_PotionMaker comes from PotionMakerStates.
        m_PotionMaker = potionMaker;
    }

    public override void Enter()
    {
        Debug.Log("'Okay! Let me go research it first.' The potion maker walks off to her study station, where she looks over her notes. 'I'll need just a few seconds.'");
        m_PotionMaker.timerText.gameObject.SetActive(true);
        m_PotionMaker.StartCoroutine(StudyTimer(5f));
    }

    public override void Execute()
    {
        // Nothing in the Execute() method, since this is just a waiting state.
    }

    public override void Exit()
    {
        Debug.Log("The potion maker concludes her study.");
    }

    // The Studying State will involve a set timer in its functions, before doing everything else.
    private IEnumerator StudyTimer(float duration)
    {
        float timer = duration;

        // Continue while there is still time.
        while (timer > 0)
        {
            timer -= Time.deltaTime;

            // To give the player feedback, we put out the time left. "0.0" rounds the result to one decimal's place.
            m_PotionMaker.timerText.text = "Time left for study: " + timer.ToString("0.0");
            yield return null;
        }
        // After the timer is done, change state to Check Component State.
        if (timer <= 0)
        {
            m_PotionMaker.ChangeState(new CheckComponentsState(m_PotionMaker));
        }
    }
}
