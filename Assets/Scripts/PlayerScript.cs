using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerScript : NetworkBehaviour
{
	//public List<Material> materialList = new List<Material>();
	//public Button choiceButtonPrefab;
	//private Transform canvasTransform;
	
    public override void OnStartClient()
    {
		base.OnStartClient();
        if (!base.hasAuthority) {
			return;
		}
		
		
		/*canvasTransform = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Transform>();
		
		for (int i=0; i<materialList.Count; i++) {
			Button choiceButton = Instantiate(choiceButtonPrefab, canvasTransform);
			choiceButton.GetComponentInChildren<Text>().text=(i+1).ToString();
			Vector2 pos = choiceButton.GetComponent<RectTransform>().anchoredPosition;
			pos.x += i*30;
			choiceButton.GetComponent<RectTransform>().anchoredPosition = pos;
			int temp = i;
			choiceButton.onClick.AddListener(delegate {CmdSetTexture(temp);});
		}*/
    }
	
	/*[Command]
	void CmdSetTexture(int index) {
		RpcSetTexture(index);
	}
	
	[ClientRpc]
	void RpcSetTexture(int index) {
		GetComponent<Renderer>().material = materialList[index];
	}*/

    // Update is called once per frame
    void Update()
    {
        if (!base.hasAuthority) {
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