using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Panda;

public class RobotTasks : MonoBehaviour
{
    public float speed = 5f;
    public Transform[] exhibits;
    public Text displayText;

    private Rigidbody rb;
    private NavMeshAgent navAgent;
    private RobotController robot;
    private bool isPresenting;

    Transform target;
    GameObject player;
    
    int currentExhibit = 0;

    private float stoppingDistance = 0f;
    public bool playerActive = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        navAgent = GetComponent<NavMeshAgent>();
        robot = GetComponent<RobotController>();
        player = GameObject.FindGameObjectWithTag("Player");
        exhibits = robot.exhibits;
    }

    [Task]
    bool IsPlayerActive()
    {
        return playerActive;
    }

    [Task]
    bool SetPlayerActive(bool active)
    {
        playerActive = active;
        return true;
    }

    [Task]
    void IsLow()
    {
        if (robot.charge < robot.lowCharge)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    bool IsClose(string tag, string task)
    {
        Transform target = GameObject.FindGameObjectWithTag(tag).transform;
        float distance = 0f;
        switch (task)
        {
            case "approach":
                distance = robot.approachDistance;
                break;
            case "interact":
                distance = robot.interactDistance;
                break;
            default:
                distance = robot.approachDistance;
                break;
        }
        return Vector3.Distance(target.position, gameObject.transform.position) < distance;
    }

    [Task]
    bool ValidExhibit()
    {
        if(currentExhibit < exhibits.Length)
        {
            return true;
        }
        else
        {
            playerActive = false;
            return false;
        }
    }

    [Task]
    bool IsPresenting()
    {
        return isPresenting;
    }

    [Task]
    void MoveTo(string tag)
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag(tag).transform;
        }

        navAgent.stoppingDistance = 1f;

        if(tag == "Exhibit")
        {
            target = exhibits[currentExhibit];
        }

        if (navAgent.destination != target.transform.position)
        {
            navAgent.SetDestination(target.position);
        }

        if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            Task.current.Succeed();
            target = null;
        }
    }

    [Task]
    void MoveToPlayer()
    {
        navAgent.stoppingDistance = 2f;

        if (navAgent.destination != player.transform.position)
        {
            navAgent.SetDestination(player.transform.position);
        }

        if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            Task.current.Succeed();
            target = null;
        }
    }

    [Task]
    bool QueryPlayer(string text)
    {
        if (!playerActive)
        {
            return false;
        }
        else
        {
            displayText.text = text;
            //Task.current.Complete(Input.GetKeyDown(KeyCode.Y) || Input.GetKeyDown(KeyCode.N));
            return true;
        }
    }

    [Task]
    bool StartPresenting()
    {
        currentExhibit = 0;
        isPresenting = true;
        return true;
    }

    [Task]
    bool EndPresenting()
    {
        currentExhibit = 0;
        isPresenting = false;
        return true;
    }

    [Task]
    bool Present()
    {
        displayText.text = "Presentation " + currentExhibit;
        currentExhibit++;
        return true;
    }

    [Task]
    void Charge()
    {
        robot.Charge(true);
        if (robot.charge >= 100f)
        {
            Task.current.Succeed();
        }
    }

    [Task]
    bool Idle()
    {
        displayText.text = "Idling";
        return true;
    }

    [Task]
    bool Display(string text)
    {
        if (displayText != null)
        {
            displayText.text = text;
            displayText.enabled = text != "";
        }
        return true;
    }
}
