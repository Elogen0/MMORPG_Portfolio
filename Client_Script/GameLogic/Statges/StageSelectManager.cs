using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StageSelectManager : MonoBehaviour
{
    [SerializeField] private StageNode[] stageNodes;
    [SerializeField] private StageNode startNode;
    private StageNode currentNode;
    
    private GameObject playerAvatar;
    private Animator _anim;
    private bool isMoving = false;
    void Start()
    {
        playerAvatar = GameObject.FindWithTag("Player");
        _anim = playerAvatar.GetComponent<Animator>();

        currentNode = startNode;
        playerAvatar.transform.position = currentNode.transform.position;
        _anim.Play("Idle");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 touchPos = new Vector2(wp.x, wp.y);
            var cols = Physics2D.OverlapPointAll(touchPos);

            foreach (var col in cols)
            {
                if (col.gameObject.TryGetComponent(out StageNode node))
                {
                    StartCoroutine(GoToNode(node));
                }    
            }
        }
    }


    IEnumerator GoToNode(StageNode selectNode)
    {
        if (selectNode == currentNode)
        {
            playerAvatar.transform.position = selectNode.transform.position;
            selectNode.ShowNodeInfo();
            currentNode = selectNode;
            yield break;
        }

        //currentNode = selectNode;
        _anim.Play("Walk");
        
        while (!Mathf.Approximately(Vector3.SqrMagnitude(playerAvatar.transform.position - selectNode.transform.position), 0)  )
        {
            playerAvatar.transform.position = Vector3.MoveTowards(playerAvatar.transform.position, currentNode.nextNode.transform.position, 0.2f);    
            yield return null;
        }
        _anim.Play("Idle");
        selectNode.ShowNodeInfo();
    }
}
