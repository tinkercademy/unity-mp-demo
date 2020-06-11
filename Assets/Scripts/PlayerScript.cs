using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerScript : NetworkBehaviour
{
	[SyncVar]
	public bool isReady = false;
	
	[SyncVar]
	public bool gameStart = false;
	
	public List<Material> materialList = new List<Material>();
	public Button choiceButtonPrefab;
	private Transform canvasTransform;
	private Button readyButton;
	
    public override void OnStartClient()
    {
		base.OnStartClient();
        if (!base.hasAuthority) {
			return;
		}
				
		canvasTransform = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Transform>();
		readyButton = GameObject.FindGameObjectWithTag("ReadyButton").GetComponent<Button>();
		readyButton.onClick.AddListener(CmdChangeReady);
		
		for (int i=0; i<materialList.Count; i++) {
			Button choiceButton = Instantiate(choiceButtonPrefab, canvasTransform);
			choiceButton.GetComponentInChildren<Text>().text=(i+1).ToString();
			Vector2 pos = choiceButton.GetComponent<RectTransform>().anchoredPosition;
			pos.x += i*30;
			choiceButton.GetComponent<RectTransform>().anchoredPosition = pos;
			int temp = i;
			choiceButton.onClick.AddListener(delegate {CmdSetTexture(temp);});
		}
    }
	
	[Command]
	void CmdChangeReady() {
		isReady ^= true;//toggles isReady
		TargetChangeReady(isReady);
		
		List<PlayerScript> playerScriptList = new List<PlayerScript>();
		foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
			PlayerScript ps = player.GetComponent<PlayerScript>();
			if (ps.isReady == false) {
					return;
			}
			playerScriptList.Add(ps);
		}
		
		//only runs if all players are ready
		foreach (PlayerScript ps in playerScriptList) {
			ps.gameStart = true;
		}
	}
	
	[TargetRpc]
	//no NetworkConnection specified - ie it always targets the owner of this object 
	//why include bool toChange - there's latency when the server updates the syncvar, so include it in the function to be safe
	void TargetChangeReady(bool change) {
		readyButton.GetComponentInChildren<Text>().text = change ? "READY!!!" : "Not Ready...";
	}
		
	
	[Command]
	void CmdSetTexture(int index) {
		RpcSetTexture(index);
	}
	
	[ClientRpc]
	void RpcSetTexture(int index) {
		GetComponent<Renderer>().material = materialList[index];
	}

    // Update is called once per frame
    void Update()
    {
        if (!base.hasAuthority) {
			return;
		}
		
		if (!gameStart) {
			return;
		}
		
		float xMov = Input.GetAxis("Horizontal");
		float zMov = Input.GetAxis("Vertical");
		CmdMoveCube(xMov, zMov);
    }
	
	[Command]
	void CmdMoveCube(float xMov, float zMov) {
		transform.position += xMov * transform.right;
		transform.position += zMov * transform.forward;
	}
}