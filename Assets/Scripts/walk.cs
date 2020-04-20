using UnityEngine;
using System.Collections;
using System.IO;

using System.Linq;	// needed for ordering

public class walk : MonoBehaviour {

	public float WalkingSpeed = 1f;

	public GameObject Sign;
	public GameObject SignPost;

	// Use this for initialization
	void Start () {
		// sort of hackish for now, find the sign in children
        // probs a better way to make this happen automagically
        Sign = transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/Sign Post/Sign intermediary/Sign").gameObject;
    }

	public void SetSign(string imgPath) {
		StartCoroutine (YieldLoad(imgPath));
//		Texture2D newTex = LoadPNG (imgPath);
//		Sign.GetComponent<Renderer> ().material.mainTexture = newTex;
		//Sign.GetComponent<Renderer> ().material.mainTexture = LoadPNG (imgPath);
	}

	IEnumerator YieldLoad(string imgPath) {
		//Debug.Log ("!");
		Texture2D newTex = LoadPNG (imgPath);

		yield return newTex;

		Sign.GetComponent<Renderer> ().material.mainTexture = newTex;

	}
	
	// Update is called once per frame
	void Update () {
		transform.position += new Vector3 (WalkingSpeed * Time.deltaTime, 0, 0);
	}

	void OnCollisionEnter(Collision collision) {
		if(collision.gameObject.name == "killer")
			Destroy (this.gameObject);
	}

	public static Texture2D LoadPNG(string filePath) {

		Texture2D tex = null;
		byte[] fileData;

		if (File.Exists(filePath))     {
			fileData = File.ReadAllBytes(filePath);
			tex = new Texture2D(2, 2);
			tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
		}
		return tex;
	}
}
