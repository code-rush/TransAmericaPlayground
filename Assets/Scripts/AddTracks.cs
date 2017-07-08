using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AddTracks : MonoBehaviour {

//	public GameObject TrackBlocks;
//	public Text countText;
//
//	Ray ray;
//	RaycastHit hit;
//
//	private int count;
//
//	// Use this for initialization
//	void Start () {
//		count = 0;
//		SetCountText();
//	}
//	
//	// Update is called once per frame
//	void Update () {
//		ray = Camera.main.ScreenPointToRay (Input.mousePosition);
//		if (Physics.Raycast (ray, out hit)) {
//			if (hit.collider.gameObject.tag == "Point") {
//				if (Input.GetMouseButton (0)) {
//					GameObject block = Instantiate (TrackBlocks, new Vector3 (hit.point.x, hit.point.y, hit.point.z), Quaternion.identity) as GameObject;
//					count = count + 1;
//					SetCountText ();
//				}
//			}
//		}
//	}
//
//	void SetCountText () {
//		countText.text = count + count.ToString ();
//	}
}
