using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;	// needed for ordering
using System.Collections.Generic; // array lists

public class spawner : MonoBehaviour {

	public GameObject[] People;

	// protester properties
	public bool RandomWalkingSpeed = true;
	public float MinWalkingSpeed = 1f;
	public float MaxWalkingSpeed = 1f;
	public float minimumProtesterSize = 1.3f;
	public float maximumProtesterSize = 1.8f;
	public float minSignScale = .9f;
	public float maxSignScale = 1.5f;


	public float SpawnInterval = .6f;
	public float maxSpawnInterval = 15f;
	public float minSpawnInterval = .8f;
	public float randomSpawnInterval = .8f;
	private float lastSpawnTime = -99f;
	public int ProtesterCap = 200;

	// vary the order and starting Z of protesters
	private float spawnZ = -1f;
	private int protesterId = 0;


	// texture stuff
	public string path;	// where are images located?
	private string filePath;	// current or random image file path to use
	public List<Texture2D> signTextures;
	private float LastTextureRefresh = 0f;	// reload textures every once in a while in case of deletion
	private float TextureRefreshInterval = 600f;	// every 10 minutes?

	// Use this for initialization
	void Start () {

        // using the StandaloneFileBrowser plugin to allow user folder select
        var paths = SFB.StandaloneFileBrowser.OpenFolderPanel("Open Folder", "", false);
        path = paths[0].ToString();
        Debug.Log(path);

        //MacFullscreenMode.FullscreenWindow;

        lastSpawnTime = Time.time;

		TextureRefresh ();

		Cursor.visible = false;


		//InvokeRepeating("SpawnNew", SpawnInterval, SpawnInterval);
	}

	void TextureRefresh() {

		// clear it totally
		signTextures.Clear ();

		// load all the images into the array
		// check out directory for all image files available
		DirectoryInfo dir = new DirectoryInfo(path);

		//if(!Application.isEditor)
		//	dir = new DirectoryInfo(Application.dataPath + "/../../scans/");

        // do so by date, newest first
        string[] extensions = new[] { ".jpg", ".tiff", ".bmp", ".png" };

        FileInfo[] info =
            dir.EnumerateFiles()
                 .Where(f => extensions.Contains(f.Extension.ToLower()))
                 .OrderByDescending(f => f.LastWriteTime)
                 .ToArray();

        //FileInfo[] info = dir.GetFiles("*.jpg").OrderByDescending(f => f.LastWriteTime).ToArray();

        Debug.Log(info);

		foreach(FileInfo f in info) {

			// new one!
			filePath = f.ToString();

            Debug.Log(filePath);

			// if running as build gotta change path
			//if (!Application.isEditor) 
			//	filePath = Application.dataPath + "/../../scans/" + filePath;
			

			Texture2D newTex = LoadPNG (filePath);
			signTextures.Add (newTex);
		}
	}
	
	// Update is called once per frame
	void Update () {

		// do we need to refresh texture list? in case of deletion
//		if (Time.time > LastTextureRefresh + TextureRefreshInterval) {
//			TextureRefresh ();
//			LastTextureRefresh = Time.time;
//		}

		// shall we spawn a new protester?
		if (Time.time > lastSpawnTime + SpawnInterval) {

			// how many signs are in existance?
			int numSigns = SpawnNew ();

			// make them spawn faster if there are more signs made
			//SpawnInterval = (float)Map (numSigns, 0f, ProtesterCap, maxSpawnInterval, minSpawnInterval);

			// dont let it go outside of acceptable range
			SpawnInterval = Mathf.Clamp (SpawnInterval, minSpawnInterval, maxSpawnInterval);

			//Debug.Log ("current spawn interval is " + SpawnInterval);

			randomSpawnInterval = SpawnInterval / 4f;

			lastSpawnTime = Time.time + Random.Range(-randomSpawnInterval, randomSpawnInterval);

			// next!
			protesterId++;
			if (protesterId >= People.Length)
				protesterId = 0;

			// also vary starting Z position so they don't overlap so much
			spawnZ+=.5f;
			if (spawnZ >= 2f)
				spawnZ = -1f;
		}

		if (Input.GetKeyDown("s"))
			ScreenCapture.CaptureScreenshot("Screenshot.png");
	}
		

	int SpawnNew() {
		
		GameObject Person = People [protesterId];
		
		// create new protestor
		GameObject newProtester = Instantiate (Person, transform.position, Quaternion.Euler(0,90,0));

		// random scale for variation
		float r = Random.Range(minimumProtesterSize, maximumProtesterSize);
		newProtester.transform.localScale = new Vector3(r,r,r);

		// use varied Z starting position
		newProtester.transform.position += new Vector3(0f,0f,spawnZ);

		// random walking speed
		if (RandomWalkingSpeed) {
			newProtester.GetComponent<walk>().WalkingSpeed = Random.Range (MinWalkingSpeed, MaxWalkingSpeed);
		}
		// match animation speed to walking speed
		newProtester.GetComponent<Animator>().speed = newProtester.GetComponent<walk>().WalkingSpeed;

		// also adjust sign positioning
		float signX = Random.Range(.1f,.25f);
		float signY = Random.Range(-.2f,.3f);
		float signZ = Random.Range(0f,-.2f);
		newProtester.GetComponent<walk>().SignPost.transform.position += new Vector3(signX,signY,signZ);
		// and rotation
		float rX = Random.Range(-10f,10);
		float rY = Random.Range(-10f,10);
		float rZ = Random.Range(-10f,10);
		newProtester.GetComponent<walk>().SignPost.transform.rotation = Quaternion.Euler(rX,rY,rZ) * newProtester.GetComponent<walk>().SignPost.transform.rotation;

		// also scale sign
		float prevX = newProtester.GetComponent<walk>().SignPost.transform.localScale.x;
		float prevY = newProtester.GetComponent<walk>().SignPost.transform.localScale.y;
		float prevZ = newProtester.GetComponent<walk>().SignPost.transform.localScale.z;

		float scaler = Random.Range (minSignScale, maxSignScale);

		float x = prevX * scaler;
		float y = prevY * scaler;
		float z = prevZ * scaler;

		newProtester.GetComponent<walk>().SignPost.transform.localScale = new Vector3(x,y,z);




		// ---- texture array stuff -----

		// is there a new texture?
		// check out directory for all image files available
		DirectoryInfo dir = new DirectoryInfo(path);

		//if(!Application.isEditor)
		//	dir = new DirectoryInfo(Application.dataPath + "/../../scans/");

        // do so by date, newest first
        string[] extensions = new[] { ".jpg", ".tiff", ".bmp", ".png" };

        FileInfo[] info =
            dir.EnumerateFiles()
                 .Where(f => extensions.Contains(f.Extension.ToLower()))
                 .OrderByDescending(f => f.LastWriteTime)
                 .ToArray();


		if (info.Length > signTextures.Count) {
			
			// there is a new one.
			filePath = info [0].ToString ();

			// if running as build gotta change path
			//if (!Application.isEditor) {
			//	filePath = Application.dataPath + "/../../scans/" + filePath;
			//}

			// so next time it'll know if there's a new one
//			lastImgLoaded = filePath;

			Texture2D newTex = LoadPNG (filePath);
			signTextures.Add (newTex);

			newProtester.GetComponent<walk> ().Sign.GetComponent<Renderer> ().material.mainTexture = signTextures[signTextures.Count - 1];
			newProtester.GetComponent<walk> ().Sign.GetComponent<Renderer> ().material.shader = Shader.Find("Unlit/Texture");
		} else {

			// not new, pick a random one?
			int rando = Random.Range (0, signTextures.Count);

			newProtester.GetComponent<walk> ().Sign.GetComponent<Renderer> ().material.mainTexture = signTextures [rando];
			newProtester.GetComponent<walk> ().Sign.GetComponent<Renderer> ().material.shader = Shader.Find("Unlit/Texture");
		}


		// random coloring
		Color newColor = new Color( Random.Range(.1f,1f), Random.Range(.1f,1f), Random.Range(.1f,1f), 1.0f );
		Renderer[] rends = newProtester.GetComponentsInChildren<Renderer>( );

		foreach (Renderer rend in rends) {
			if (!rend.transform.name.StartsWith ("Sign")) {
				rend.material.color = newColor;

				//Debug.Log (rend.material.color);
			}
		}


		return signTextures.Count;	// let us know how many protesters there are so we can map accordingly
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


	public double Map(double x, double in_min, double in_max, double out_min, double out_max)
	{
		return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
	}
}