using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Panda;

public class PotionMakerClass : MonoBehaviour
{
    /*------- v STATES v -------*/
    public PotionMakerStates m_Idle { get; set; } = null;
    public PotionMakerStates m_Brewing { get; set; } = null;
    public PotionMakerStates m_Requesting { get; set; } = null;
    public PotionMakerStates m_Studying { get; set; } = null;
    public PotionMakerStates m_CheckComponents { get; set; } = null;
    public PotionMakerStates m_Transaction { get; set; } = null;
    public PotionMakerStates m_PotionInquiry { get; set; } = null;
    public PotionMakerStates m_Failed { get; set; } = null;
    public PotionMakerStates m_Cleaning { get; set; } = null;
    public PotionMakerStates m_Approached { get; set; } = null;
    public PotionMakerStates m_Attending { get; set; } = null;
    public PotionMakerStates m_Current { get; set; } = null;

    /*------- v BUTTONS v -------*/
    // All buttons here are meant to be disabled within the scene by default.
    public GameObject btn_Approach; // For Idle -> Approach
    public GameObject btn_Talk; // For Approach -> Attending
    public GameObject btn_Inquire; // For Attending -> Potion Inquiry
    public GameObject btn_Leave; // For Attending -> Idle
    public GameObject btn_Healing; // For Potion Inquiry -> Transaction
    public GameObject btn_Arcane; // For Potion Inquiry -> Studying
    public GameObject btn_ThirdPotion; // For Potion Inquiry -> Requesting
    public GameObject btn_Give; // For Requesting -> Brewing
    public GameObject btn_NoGive; // For Requesting -> Attending
    public GameObject btn_Proceed; // For Check Components -> Brewing
    public GameObject btn_Continue; // For Failed -> Brewing
    public GameObject btn_Abandon; // For Failed -> Attending
    public GameObject btn_Back; // For Check Components -> Attending
    public GameObject btn_Pay; // For Transaction -> Attending

    /*------- v OTHER VARIABLES v -------*/
    public bool inOneSession; // This variable is mainly for Transaction -> Attending, so that the potion maker asks something different if the player doesn't leave.
    public TextMeshProUGUI timerText; // This will be to give the player visual feedback during waiting functions of certain states.
    public bool isThirdPotion; // Specifically so there's no failure for the third potion during the Brewing State, since the potion maker is very familiar with it.
    public bool dirty; // This will be true if the potion maker was in the Brewing State at any point, and the player leaves.
    public PandaBehaviour pandaB;

    // These variables are for the bot to move randomly between locations in the scene.
    public Transform[] locations;
    Transform targetLocation;

    NavMeshAgent navAgent;

    float newPathTime;
    float pathChangeDelayTime = 10f; // The amount of time that is between each location change in seconds.

    bool canMoveRandomly = true;
    /* --- v IMPORTANT BOOLS FOR BUTTONS v --- */
    bool customerApproached = false; // Replacing the isBeingApproached variable in the IdleState.cs script.
    bool customerTalked = false; // Replacing the isTalkedTo variable in the ApproachedState.cs script.
    bool customerInquired = false; // We get the picture now, I think.
    bool customerLeft = false;
    bool customerChoseHealing = false;
    bool customerChoseArcane = false;
    bool customerChoseThirdPotion = false;
    bool customerPaid = false;
    public bool approachButtonAffected = true; // Will decide if btn_Approach should be affected by CounterTriggerZone.cs.

    // Start is called before the first frame update
    void Start()
    {
        //First, initialize into the Idle State (a.k.a the opening state).
        m_Idle = new IdleState(this);
        m_Current = m_Idle;
        m_Current.Enter();
        inOneSession = false;
        isThirdPotion = false;
        dirty = false;

        navAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        m_Current.Execute();
    }
    #region Idle Tasks and Related Code
    [Task]
    public void EnterIdleState()
    {
        if (customerApproached == false)
        {
            Debug.Log("The Kobold's Beaker is ready for business!");
            // Enable the button for Approach.
            //btn_Approach.SetActive(true);
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void ChooseRandomLocation()
    {
        if (Time.time - newPathTime >= pathChangeDelayTime && canMoveRandomly == true) // This is to have a delay between each time the bot wanders to a random location.
        {
            if (!navAgent.pathPending && !navAgent.hasPath) // If the potion maker has reached, thanks to ChatGPT for suggesting these parameters.
            {
                if (locations.Length > 0)
                {
                    targetLocation = locations[Random.Range(0, locations.Length)]; // Randomize the location the bot goes to.

                    navAgent.SetDestination(targetLocation.position);
                    //Debug.Log("Moving...");
                    newPathTime = Time.time;
                }
                else
                {
                    Task.current.Fail();
                }
            }
        }

        Task.current.Succeed();
    }

    #endregion

    #region Approached Tasks and Related Code

    [Task]
    public void ApproachDebug()
    {
        Debug.Log("Approached Tree now!");
        Task.current.Succeed();
    }

    [Task]
    public void CheckApproach()
    {
        if (customerApproached == true)
        {
            approachButtonAffected = false;
            customerApproached = false;
            btn_Approach.SetActive(false);
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void MoveToCounter()
    {
        if (locations.Length > 0 && navAgent != null)
        {
            canMoveRandomly = false;
            navAgent.SetDestination(locations[0].position); // Element 0 is the Attending Location.
            btn_Talk.SetActive(true);
            Task.current.Succeed();
        }
        else
        {
            Debug.Log("No location to move to.");
            Task.current.Fail();
        }
    }

    #endregion

    #region Attending Tasks and Related Code

    [Task]
    public void CheckAttending()
    {
        if (customerTalked == true)
        {
            btn_Talk.gameObject.SetActive(false);
            customerTalked = false;
            if (inOneSession == true)
            {
                Debug.Log("'Anything else I can do for you?' The potion maker politely asks.");
                btn_Inquire.SetActive(true);
                btn_Leave.SetActive(true);
            }
            else
            {
                Debug.Log("'Hello! Welcome to The Kobold's Beaker, how may I help you?' The potion maker chirps.");
                btn_Inquire.SetActive(true);
                btn_Leave.SetActive(true);
            }
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void CheckCustomerInquired()
    {
        //Debug.Log("This task is running");
        //if (customerInquired == true)
        //{
        //    //Debug.Log("Yes, inquiry is true");
        //    customerInquired = false;
        //    Task.current.Succeed();
        //}
        //else
        //{
        //    Task.current.Fail();
        //}
        StartCoroutine(InquiryRepeat());
    }

    private IEnumerator InquiryRepeat() // Brute forcing the CheckCustomerInquired task to repeat, since the repeat node is not working for some reason.
    {
        while (customerInquired == false)
        {
            if (customerInquired == true)
            {
                break;
            }
            yield return null;
        }
        Task.current.Succeed();
    }

    [Task]
    public void CheckCustomerLeft()
    {
        if (customerLeft == true)
        {
            customerLeft = false;
            btn_Inquire.SetActive(false);
            btn_Leave.SetActive(false);
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    #endregion

    #region Potion Inquiry Tree and Related Code

    [Task]
    public void InitializeInquiry() // Start the necessary things for this tree
    {
        customerInquired = false;
        btn_Inquire.SetActive(false);
        btn_Leave.SetActive(false);
        Debug.Log("[EXPAND FOR FULL TEXT] 'I have about three potions for today, but only two types are readily available.'\n" +
            "'First off, there's our healing potions, which are prepared already and have a flat fee of 10 gold pieces each!'\n" +
            "'Then we have a rather new one! A Potion of Arcane Excellence. But since it's new, I'd have to check if I actually have the components, and refresh myself on how to make it.'\n" +
            "'The third one... Well, I know for sure I'm missing something for that one.'");

        btn_Healing.SetActive(true);
        btn_Arcane.SetActive(true);
        btn_ThirdPotion.SetActive(true);
        Task.current.Succeed();
    }

    [Task]
    public void CheckHealingOption()
    {
        //Debug.Log("Running");
        //if (customerChoseHealing)
        //{
        //    customerChoseHealing = false;
        //    Task.current.Succeed();
        //}
        //else
        //{
        //    Task.current.Fail();
        //}
        StartCoroutine(HealingRepeat());
    }

    private IEnumerator HealingRepeat() // Continuing the trend of brute-forcing.
    {
        while (customerChoseHealing == false)
        {
            if (customerChoseHealing == true)
            {
                break;
            }
            yield return null;
        }
        Task.current.Succeed();
    }

    [Task]
    public void CheckArcaneOption()
    {
        //if (customerChoseArcane)
        //{
        //    customerChoseArcane = false;
        //    Task.current.Succeed();
        //}
        //else
        //{
        //    Task.current.Fail();
        //}
        StartCoroutine(ArcaneRepeat());
    }

    private IEnumerator ArcaneRepeat() // Continuing the trend of brute-forcing.
    {
        while (customerChoseArcane == false)
        {
            if (customerChoseArcane == true)
            {
                break;
            }
            yield return null;
        }
        Task.current.Succeed();
    }

    [Task]
    public void CheckThirdOption()
    {
        //if (customerChoseThirdPotion)
        //{
        //    customerChoseThirdPotion = false;
        //    Task.current.Succeed();
        //}
        //else
        //{
        //    Task.current.Fail();
        //}
        StartCoroutine(ThirdOptionRepeat());
    }

    private IEnumerator ThirdOptionRepeat() // Continuing the trend of brute-forcing.
    {
        while (customerChoseThirdPotion == false)
        {
            if (customerChoseThirdPotion == true)
            {
                break;
            }
            yield return null;
        }
        Task.current.Succeed();
    }

    #endregion

    #region Transaction Tree and Related Code

    [Task]
    public void InitializeTransaction()
    {
        if (customerChoseHealing)
        {
            Debug.Log("The potion maker presents the potion.\n" +
                "Alright! I'll just wait for the gold.");
            customerChoseHealing = false;
            btn_Healing.SetActive(false);
            btn_Arcane.SetActive(false);
            btn_ThirdPotion.SetActive(false);
            btn_Pay.SetActive(true);
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void CheckPaid()
    {
        StartCoroutine(TransactionRepeat());
    }

    private IEnumerator TransactionRepeat() // Continuing the trend of brute-forcing.
    {
        while (customerPaid == false)
        {
            if (customerPaid == true)
            {
                break;
            }
            yield return null;
        }
        Task.current.Succeed();
    }

    #endregion

    #region New Button Functions

    public void DoApproach()
    {
        //Debug.Log("Clickity?");
        customerApproached = true;
    }

    public void DoTalk()
    {
        customerTalked = true;
    }

    public void DoInquire()
    {
        Debug.Log("Clicked");
        customerInquired = true;
        Debug.Log(customerInquired);
    }

    public void DoLeave()
    {
        customerLeft = true;
    }

    public void DoHealing()
    {
        customerChoseHealing = true;
    }

    public void DoArcane()
    {
        customerChoseArcane = true;
    }

    public void DoThirdPotion()
    {
        customerChoseThirdPotion = true;
    }

    public void DoPay()
    {
        customerPaid = true;
    }

    #endregion

    #region OnClick Functions from Assignment 1

    // The following OnClick functions (like ApproachOnClick, TalkOnClick and InquireOnClick) are all here
    // so that we can call them from our buttons' OnClick functions from Unity.
    // We can't normally just call the functions that are called within these OnClicks since they are stored inside
    // the state scripts themselves, and PotionMakerStates and its subclasses are not attached to any GameObject.

    public void GiveOnClick()
    {
        if (m_Current != null)
        {
            if (m_Current.GetType() == typeof(RequestingState))
            {
                ((RequestingState)m_Current).GiveWyvernSpike();
            }
        }
    }

    public void NoGiveOnClick()
    {
        if (m_Current != null)
        {
            if (m_Current.GetType() == typeof(RequestingState))
            {
                ((RequestingState)m_Current).DoNotGiveSpike();
            }
        }
    }

    public void ContinueOnClick()
    {
        if (m_Current != null)
        {
            if (m_Current.GetType() == typeof(FailedState))
            {
                ((FailedState)m_Current).ContinueBrewing();
            }
        }
    }

    public void AbandonOnClick()
    {
        if (m_Current != null)
        {
            if (m_Current.GetType() == typeof(FailedState))
            {
                ((FailedState)m_Current).AbandonBrewing();
            }
        }
    }

    public void ProceedOnClick()
    {
        if (m_Current != null)
        {
            if (m_Current.GetType() == typeof(CheckComponentsState))
            {
                ((CheckComponentsState)m_Current).ChooseProceedOption();
            }
        }
    }

    public void BackOnClick()
    {
        if (m_Current != null)
        {
            if (m_Current.GetType() == typeof(CheckComponentsState))
            {
                ((CheckComponentsState)m_Current).ChooseBackOption();
            }
        }
    }

    #endregion

    public void ChangeState(PotionMakerStates nextState)
    {
        // If we do have a state under m_Current, run the Exit().
        if (m_Current != null)
        {
            m_Current.Exit();
        }

        m_Current = nextState;
        m_Current.Enter();
    }


}
