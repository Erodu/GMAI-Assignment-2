using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Panda;

public class PotionMakerClass : MonoBehaviour
{
    #region States
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

    #endregion

    #region Buttons

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

    #endregion

    #region Other Variables

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

    bool brewSuccessful = false;
    bool brewFailed = false;

    #endregion
    /* --- v IMPORTANT BOOLS FOR BUTTONS v --- */
    bool customerApproached = false; // Replacing the isBeingApproached variable in the IdleState.cs script.
    bool customerTalked = false; // Replacing the isTalkedTo variable in the ApproachedState.cs script.
    bool customerInquired = false; // We get the picture now, I think.
    bool customerLeft = false;
    bool customerChoseHealing = false;
    bool customerChoseArcane = false;
    bool customerChoseThirdPotion = false;
    bool customerPaid = false;
    bool customerProceed = false;
    bool customerBack = false;
    bool studyComplete = false;
    bool checkComplete = false;
    bool cleaningComplete = false;
    bool customerContinue = false;
    bool customerAbandon = false;
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
        //m_Current.Execute();
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
    public void CheckApproach()
    {
        //Debug.Log("Checking for approach");
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
        //Debug.Log("Checking for attending");
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
        //if (customerLeft == true)
        //{
        //    customerLeft = false;
        //    btn_Inquire.SetActive(false);
        //    btn_Leave.SetActive(false);
        //    Task.current.Succeed();
        //}
        //else
        //{
        //    Task.current.Fail();
        //}
        StartCoroutine(CheckLeftRepeat());
    }

    private IEnumerator CheckLeftRepeat() 
    {
        while (customerLeft == false)
        {
            if (customerLeft == true)
            {
                break;
            }
            yield return null;
        }
        Task.current.Succeed();
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

    // The Transaction Tree, unfortunately, does not work in the exact way that it does in the Finite State Machine.
    // Due to unfamiliarity with PandaBT, I was unable to find a way to have it go back to the Attending Tree without a myriad of error logs.
    // I eventually decided to simplify it and make it so that the Transaction tree would simply go back to the beginning, the Root.

    [Task]
    public void InitializeTransaction()
    {
        if (customerChoseHealing || cleaningComplete)
        {
            Debug.Log("The potion maker presents the potion.\n" +
                "Alright! I'll just wait for the gold.");
            cleaningComplete = false;
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

    [Task]
    public void SayFarewell() // This will be used for other trees as well.
    {
        if (customerPaid)
        {
            Debug.Log("Thank you for your business!");
            customerPaid = false;
            btn_Pay.SetActive(false);
            canMoveRandomly = true;
            approachButtonAffected = true;
            navAgent.ResetPath();

            Task.current.Succeed();
        }
        else if (customerLeft)
        {
            Debug.Log("Oh, okay. Have a nice day.");
            customerLeft = false;
            btn_Inquire.SetActive(false);
            btn_Leave.SetActive(false);
            canMoveRandomly = true;
            approachButtonAffected = true;
            navAgent.ResetPath();

            Task.current.Succeed();
        }
        else if (customerBack)
        {
            Debug.Log("Fair enough, let me know if you need anything else.");
            customerBack = false;
            btn_Proceed.SetActive(false);
            btn_Back.SetActive(false);
            canMoveRandomly = true;
            approachButtonAffected = true;
            navAgent.ResetPath();

            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void RestartAgent()
    {
        pandaB.Reset();
        Task.current.Succeed();
    }

    #endregion

    #region Studying Tree and Related Code

    [Task]
    public void InitializeStudy()
    {
        if (customerChoseArcane)
        {
            customerChoseArcane = false;
            btn_Healing.SetActive(false);
            btn_Arcane.SetActive(false);
            btn_ThirdPotion.SetActive(false);
            timerText.gameObject.SetActive(true);
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void MoveAndStudy()
    {
        if (locations.Length > 0 && navAgent != null)
        {
            canMoveRandomly = false;
            navAgent.SetDestination(locations[1].position); // Element 1 is the Study Location.
            StartCoroutine(WaitUntilArriveAtStudy());
            Task.current.Succeed();
        }
        else
        {
            Debug.Log("No location to move to.");
            Task.current.Fail();
        }
    }

    [Task]
    public void CheckStudy()
    {
        //StartCoroutine(StudyRepeat());
        if (studyComplete)
        {
            studyComplete = false;
            Task.current.Succeed();
            return;
        }
        StartCoroutine(StudyRepeat());
    }

    [Task]
    private IEnumerator StudyRepeat() // Continuing the trend of brute-forcing.
    {
        while (!studyComplete)
        {
            yield return null;
        }
    }

    private IEnumerator WaitUntilArriveAtStudy()
    {
        while (navAgent.pathPending || navAgent.remainingDistance > navAgent.stoppingDistance)
        {
            yield return null;
        }

        StartCoroutine(StudyTimer(5f));
    }

    private IEnumerator StudyTimer(float duration)
    {
        float timer = duration;

        // Continue while there is still time.
        while (timer > 0)
        {
            timer -= Time.deltaTime;

            // To give the player feedback, we put out the time left. "0.0" rounds the result to one decimal's place.
            timerText.text = "Time left for study: " + timer.ToString("0.0");
            yield return null;
        }
        // After the timer is done, change state to Check Component State.
        if (timer <= 0)
        {
            studyComplete = true;
        }
    }

    #endregion

    #region Check Component Tree and Related Code

    [Task]
    public void MoveAndCheck()
    {
        if (locations.Length > 0 && navAgent != null)
        {
            canMoveRandomly = false;
            navAgent.SetDestination(locations[2].position); // Element 2 is the Component Location.
            StartCoroutine(WaitUntilArriveAtComponent());
            Task.current.Succeed();
        }
        else
        {
            Debug.Log("No location to move to.");
            Task.current.Fail();
        }
    }

    [Task]
    public void KeepChecking()
    {
        if (checkComplete)
        {
            checkComplete = false;
            Task.current.Succeed();
            return;
        }
        StartCoroutine(CheckRepeat());
    }

    private IEnumerator CheckRepeat() // Continuing the trend of brute-forcing.
    {
        //while (checkComplete == false)
        //{
        //    if (checkComplete == true)
        //    {
        //        break;
        //    }
        //    yield return null;
        //}
        //Task.current.Succeed();
        while (!checkComplete)
        {
            yield return null;
        }
    }

    private IEnumerator WaitUntilArriveAtComponent()
    {
        while (navAgent.pathPending || navAgent.remainingDistance > navAgent.stoppingDistance)
        {
            yield return null;
        }

        StartCoroutine(CheckComponentTimer(5f));
    }

    private IEnumerator CheckComponentTimer(float duration)
    {
        float timer = duration;

        // Continue while there is still time.
        while (timer > 0)
        {
            timer -= Time.deltaTime;

            // To give the player feedback, we put out the time left. "0.0" rounds the result to one decimal's place.
            timerText.text = "Time left for component check: " + timer.ToString("0.0");
            yield return null;
        }
        // After the timer is done, change state to Check Component State.
        if (timer <= 0)
        {
            checkComplete = true;
        }
    }

    [Task]
    public void ConfirmWithCustomer()
    {
        Debug.Log("Okay, I have all the components needed. Just one final confirmation before I start the brewing process, is this potion what you want?");
        btn_Proceed.SetActive(true);
        btn_Back.SetActive(true);
        Task.current.Succeed();
    }

    [Task]
    public void CheckProceed()
    {
        if (customerProceed)
        {
            customerProceed = false;
            Task.current.Succeed();
            return;
        }
        StartCoroutine(ProceedRepeat());
    }
    private IEnumerator ProceedRepeat() // Continuing the trend of brute-forcing.
    {
        //while (customerProceed == false)
        //{
        //    if (customerProceed == true)
        //    {
        //        break;
        //    }
        //    yield return null;
        //}
        //Task.current.Succeed();
        while (!customerProceed)
        {
            yield return null;
        }
    }

    [Task]
    public void CheckBack()
    {
        if (customerBack)
        {
            timerText.gameObject.SetActive(false);
            customerBack = false;
            Task.current.Succeed();
            return;
        }
        StartCoroutine(BackRepeat());
    }

    private IEnumerator BackRepeat() // Continuing the trend of brute-forcing.
    {
        //while (customerBack == false)
        //{
        //    if (customerBack == true)
        //    {
        //        break;
        //    }
        //    yield return null;
        //}
        //timerText.gameObject.SetActive(false);
        //Task.current.Succeed();
        while (!customerBack)
        {
            yield return null;
        }
    }

    #endregion

    #region Brewing Task and Related Code

    [Task]
    public void InitializeBrewing()
    {
        if (locations.Length > 0 && navAgent != null)
        {
            Debug.Log("'Alright then! Wait here while I do my magic!' The potion maker begins her careful process.");
            btn_Proceed.SetActive(false);
            btn_Back.SetActive(false);
            canMoveRandomly = false;
            navAgent.SetDestination(locations[3].position); // Element 3 is the Brewing Location.
            StartCoroutine(WaitUntilMoveToBrewing());
            Task.current.Succeed();
        }
        else
        {
            Debug.Log("No location to move to.");
            Task.current.Fail();
        }
    }

    private IEnumerator WaitUntilMoveToBrewing()
    {
        while (navAgent.pathPending || navAgent.remainingDistance > navAgent.stoppingDistance)
        {
            yield return null;
        }

        StartCoroutine(BrewingTimer(5f));
    }

    private IEnumerator BrewingTimer(float duration)
    {
        float timer = duration;

        // Continue while there is still time.
        while (timer > 0)
        {
            timer -= Time.deltaTime;

            // To give the player feedback, we put out the time left. "0.0" rounds the result to one decimal's place.
            timerText.text = "Time left for brewing: " + timer.ToString("0.0");
            yield return null;
        }
        // After the timer is done, we roll for success.
        if (timer <= 0)
        {
            if (isThirdPotion == true)
            {
                brewSuccessful = true; // Automatically set it to true if the player is making the third potion.
                // Make sure that the isThirdPotion bool does not remain true for the rest of this bot's runtime.
                isThirdPotion = false;
                Task.current.Succeed();
            }
            else
            {
                DecideSuccess();
            }
        }
    }

    private void DecideSuccess()
    {
        // Random check to see if the brewing process is successful or not. Getting anything more than 2 is a success, but otherwise triggers a failstate.
        int brewCheck = Random.Range(0, 11);

        if (brewCheck > 8f)
        {
            brewSuccessful = true;
        }
        else
        {
            brewFailed = true;
        }
    }

    [Task]
    public void CheckSuccess()
    {
        if (brewSuccessful)
        {
            brewSuccessful = false;
            Task.current.Succeed();
            return;
        }
        StartCoroutine(SuccessRepeat());
    }

    private IEnumerator SuccessRepeat() // Continuing the trend of brute-forcing.
    {
        //while (brewSuccessful == false)
        //{
        //    if (brewSuccessful == true)
        //    {
        //        break;
        //    }
        //    yield return null;
        //}
        //Task.current.Succeed();
        while (!brewSuccessful)
        {
            yield return null;
        }
    }

    [Task]
    public void CheckFail()
    {
        if (brewFailed)
        {
            Task.current.Succeed();
            return;
        }
        StartCoroutine(FailRepeat());
    }

    private IEnumerator FailRepeat() // Continuing the trend of brute-forcing.
    {
        //while (brewFailed == false)
        //{
        //    if (brewFailed == true)
        //    {
        //        break;
        //    }
        //    yield return null;
        //}
        //Task.current.Succeed();
        while (!brewFailed)
        {
            yield return null;
        }
    }

    #endregion

    #region Cleaning Tree and Related Code
    // In this PandaBT version, the potion maker goes into cleaning directly after brewing.

    [Task]
    public void InitializeCleaning()
    {
        Debug.Log("Just need a bit of time for cleaning...");
        StartCoroutine(CleaningTimer(10f));
        Task.current.Succeed();
    }

    private IEnumerator CleaningTimer(float duration)
    {
        float timer = duration;

        // Continue while there is still time.
        while (timer > 0)
        {
            timer -= Time.deltaTime;

            // To give the player feedback, we put out the time left. "0.0" rounds the result to one decimal's place.
            timerText.text = "Time left for cleaning: " + timer.ToString("0.0");
            yield return null;
        }

        if (timer <= 0)
        {
            timerText.gameObject.SetActive(false); // Disable the timer text.
            cleaningComplete = true;
        }
    }

    [Task]
    public void CheckCleaning()
    {
        if (cleaningComplete)
        {
            Task.current.Succeed();
            return;
        }
        StartCoroutine(CleaningRepeat());
    }

    private IEnumerator CleaningRepeat() // Continuing the trend of brute-forcing.
    {
        //while (cleaningComplete == false)
        //{
        //    if (cleaningComplete == true)
        //    {
        //        break;
        //    }
        //    yield return null;
        //}
        //cleaningComplete = false;
        //Task.current.Succeed();
        while (!cleaningComplete)
        {
            yield return null;
        }
    }

    [Task]
    public void MoveToCounterAfterBrewing()
    {
        if (locations.Length > 0 && navAgent != null)
        {
            canMoveRandomly = false;
            navAgent.SetDestination(locations[0].position); // Element 0 is the Attending Location.
            Task.current.Succeed();
        }
        else
        {
            Debug.Log("No location to move to.");
            Task.current.Fail();
        }
    }

    #endregion

    #region Failed Tree and Related Code

    [Task]
    public void InitializeFailed()
    {
        if (brewFailed)
        {
            Debug.Log("'Dang... Could have sworn that I was gonna be successful, but this potion is experimental. If you want, I can retry it, or you can choose something else.'");
            btn_Continue.SetActive(true);
            btn_Abandon.SetActive(true);
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void CheckContinue()
    {
        if (customerContinue)
        {
            Debug.Log("Okay, let's try again!");
            Task.current.Succeed();
            return;
        }
        StartCoroutine(ContinueRepeat());
    }

    private IEnumerator ContinueRepeat() // Continuing the trend of brute-forcing.
    {
        while (!customerContinue)
        {
            yield return null;
        }
    }

    [Task]
    public void CheckAbandon()
    {
        if (customerAbandon)
        {
            Debug.Log("Okay, sorry about that. Do let me know if I can help with anything else.");
            customerAbandon = false;
            Task.current.Succeed();
            return;
        }
        StartCoroutine(AbandonRepeat());
    }

    private IEnumerator AbandonRepeat() // Continuing the trend of brute-forcing.
    {
        while (!customerAbandon)
        {
            yield return null;
        }
    }

    #endregion

    #region Retry Brewing and Related Code

    [Task]
    public void InitializeRetryBrewing()
    {
        if (customerContinue)
        {
            customerContinue = false;
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
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
        customerInquired = true;
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

    public void DoProceed()
    {
        customerProceed = true;
    }

    public void DoBack()
    {
        customerBack = true;
    }

    public void DoContinue()
    {
        customerContinue = true;
    }

    public void DoAbandon()
    {
        customerAbandon = true;
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
