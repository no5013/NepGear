// --------------------------------------
// This script is totally optional. It is an example of how you can use the
// destructible versions of the objects as demonstrated in my tutorial.
// Watch the tutorial over at http://youtube.com/brackeys/.
// --------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Destructible : NetworkBehaviour {

    [SyncVar(hook = "OnHealthChanged")]
    public float health = 50f;

	public GameObject destroyedVersion;	// Reference to the shattered version of the object

    public void TakeDamage(float amount)
    {
        health -= amount;
    }

    void OnHealthChanged(float currentHealth)
    {
        if (currentHealth <= 0f)
        {
            RpcDestroyed();
        }
    }

	// If the player clicks on the object
    [ClientRpc]
	void RpcDestroyed ()
	{
		// Spawn a shattered object
		GameObject destroyed = Instantiate(destroyedVersion, transform.position, transform.rotation);
        
        // Remove the current object
		Destroy(gameObject);
	}

}
