using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class Pipe : MonoBehaviour
{
    public Collider pipeCollider;
    [SerializeField] private Transform position1;
    [SerializeField] private Transform position2;
    [SerializeField] private Vector3 positionOffset = new Vector3(0,-1,0);

    [Space(10)]

    [SerializeField] private GameObject enterEffect;
    [SerializeField] private GameObject exitEffect;

    [Space(10)]

    [SerializeField] private float reachDistance = 0.2f;
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float exitForce = 10;
    [SerializeField] private Vector3 exitOffest = new Vector3(0, 0, 0);

    [Header("Debug:")]
    [SerializeField] private bool isMoving;
    [SerializeField] private bool isExiting;
    public bool isTeleporting;

    [Space(10)]
    [SerializeField] private UnityEvent onEnter;
    [SerializeField] private UnityEvent onExit;

    public static Coroutine coMoving;
    public static Pipe pipeMoving;

    public static List<Pipe> pipes = new List<Pipe>();

    private Transform currentPlayer;

    private void Start()
    {
        pipes.Add(this);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Verifica si el objeto que colision� es el jugador
        {
            if (isTeleporting)
                return;

            if (isMoving)
                return;

            try
            {
                if (coMoving != null)
                {
                    pipeMoving.StopCoroutine(coMoving);
                }
            }
            catch (System.Exception ex)
            {
                print(ex.Message);
            }

            isMoving = true;
     
            coMoving = StartCoroutine(Move(other.transform));
            pipeMoving = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isExiting)
            return;


        if (other.CompareTag("Player"))
        {
            isExiting = true;
            Invoke("Leave", 1);
        }
           
    }

    private void Leave()
    {
        isMoving = false;
        isExiting = false;

    }


    IEnumerator Move(Transform player)
    {
        foreach (var p in pipes)
        {
            Physics.IgnoreCollision(player.GetComponent<Collider>(), p.pipeCollider, true);
        }

        player.GetComponent<PlayerController>().anim.ResetTrigger("ExitPipe");
        player.GetComponent<PlayerController>().anim.SetTrigger("EnterPipe");

        player.GetComponent<PlayerController>().enabled = false;
        player.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        player.GetComponent<Rigidbody>().isKinematic = true;
   

        float dist1 = Vector3.Distance(player.position, position1.position + positionOffset);
        float dist2 = Vector3.Distance(player.position, position2.position + positionOffset);

        Vector3 endPosition;
        Vector3 startPosition;


        if (dist1 > dist2) 
        {
            startPosition = position2.position + positionOffset;
            endPosition = position1.position + positionOffset;

            if (exitEffect)
            {
                var clone = Instantiate(exitEffect, endPosition - positionOffset, transform.rotation);
                Destroy(clone, 4);
            }

        }
        else if (dist2 > dist1) 
        {

            startPosition = position1.position + positionOffset;
            endPosition = position2.position + positionOffset;

            if (enterEffect)
            {
                var clone = Instantiate(enterEffect, startPosition - positionOffset, transform.rotation);
                Destroy(clone, 4);
            }
        }
        else
        {

            startPosition = position1.position + positionOffset;
            endPosition = position2.position + positionOffset;
        }


        currentPlayer = player;

        onEnter.Invoke();

        Vector3 dir = (endPosition - startPosition).normalized;
        Vector3 currentPosition = player.position;

        while (Vector3.Distance(player.position, endPosition) > reachDistance) 
        {
            currentPosition = Vector3.MoveTowards(currentPosition, endPosition, Time.deltaTime * moveSpeed);
            player.position = currentPosition;

            if (!player)
                yield break;

            yield return null;
        }

        if (!player)
            yield break;

        player.position = endPosition;

        onExit.Invoke();

        player.GetComponent<PlayerController>().anim.SetTrigger("ExitPipe");
        player.GetComponent<PlayerController>().enabled = true;
        player.GetComponent<Rigidbody>().isKinematic = false;
        player.GetComponent<Rigidbody>().AddForce((dir + exitOffest) * exitForce, ForceMode.Impulse);

        coMoving = null;
        yield return new WaitForSeconds(1);

        foreach (var p in pipes)
        {
            Physics.IgnoreCollision(player.GetComponent<Collider>(), p.pipeCollider, false);
        }

        yield break;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(position1.position, 0.5f);
        Gizmos.DrawSphere(position2.position, 0.5f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(position1.position + positionOffset, 0.25f);
        Gizmos.DrawSphere(position2.position + positionOffset, 0.25f);
    }

    public void Teleport(Pipe endPipe)
    {
        if (!currentPlayer)
            return;

  
        StartCoroutine(TeleportSequence(endPipe));
    }

    IEnumerator TeleportSequence(Pipe endPipe)
    {

        endPipe.enabled = false;
        endPipe.isTeleporting = true;

        currentPlayer.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        currentPlayer.transform.position = endPipe.transform.position;

        yield return new WaitForSeconds(2);

        endPipe.enabled = true;
        endPipe.isTeleporting = false;
    }
}
