using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Kame.AI;
using UnityEngine;
using UnityEngine.UIElements;

public class Portal : MonoBehaviour
{
    public string sceneName;
    public Transform spawnPoint;
    public int destination;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagAndLayer.TagName.Player))
        {
            C_LeaveGame leaveGame = new C_LeaveGame();
            NetworkManager.Instance.Send(leaveGame);
            
            SceneLoader.Instance.LoadScene(sceneName, () =>
            {
                C_EnterGame enterGame = new C_EnterGame();
                enterGame.Name = ObjectManager.Instance.MyPlayerInfo.Name;
                enterGame.SceneName = sceneName;
                Portal portal = GetPortal(destination);
                Vector3 curPos = portal.spawnPoint.position;
                Debug.Log($"SpawnPosition = {curPos}");
                Vector3 dirVec = portal.spawnPoint.TransformDirection(Vector3.forward);
                enterGame.PosInfo = new PositionInfo()
                {
                    PosX = curPos.x,
                    PosY = curPos.z,
                    PosH = curPos.y,
                    DirX = dirVec.x,
                    DirY = dirVec.z,
                    State = CreatureState.Idle
                };
                
                Debug.Log("enterGame Pos : " + enterGame.PosInfo.PosX + "," + enterGame.PosInfo.PosY);
                NetworkManager.Instance.Send(enterGame);
            });
        }
    }
    
    public static Portal GetPortal(int destination)
    {
        foreach (Portal portal in FindObjectsOfType<Portal>())
        {
            // if (portal == this) continue;
            if (portal.destination != destination) continue;

            return portal;
        }
        return null;
    }
    
    private static void PutPlayer(Transform player, Portal otherPortal)
    {
        player.position = otherPortal.transform.position;
        player.rotation = otherPortal.transform.rotation;
    }
}
