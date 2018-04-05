using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class MechManager : NetworkBehaviour {

    //[SerializeField] ToggleEvent onToggleShared;
    //[SerializeField] ToggleEvent onToggleRemote;

    //[SyncVar] public bool dead;
    //private GameObject mainCamera;

    //private void Start()
    //{
    //    mainCamera = Camera.main.gameObject;
    //}

    //public void DisableController()
    //{
    //    if (isLocalPlayer)
    //        mainCamera.SetActive(true);

    //    onToggleShared.Invoke(false);

    //    if (isLocalPlayer)
    //        onToggleLocal.Invoke(false);
    //    else
    //        onToggleRemote.Invoke(false);
    //}

    //public void EnableController()
    //{
    //    ragdollManager.DisableRagdoll();

    //    if (isLocalPlayer)
    //        mainCamera.SetActive(false);

    //    onToggleShared.Invoke(true);

    //    if (isLocalPlayer)
    //        onToggleLocal.Invoke(true);
    //    else
    //        onToggleRemote.Invoke(true);
    //}
    
    //public void DisablePrefab()
    //{
    //    if (isLocalPlayer)
            
    //}

    //public void EnablePrefab()
    //{

    //}

}
