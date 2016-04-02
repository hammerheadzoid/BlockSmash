using UnityEngine;
using System.Collections;

public class Show_Time : MonoBehaviour
{

    public AudioClip Alarm_Sound;

    public int difficulty = 10;
    public int colorcount = 5;//with higher difficulties, colorcount can be increased to include more colours, (max is 7 for now)
    private int randcolour;
    private int oldcolour;
    public bool alarmstate;
    public GameObject[,] wall;
    public int snooze = 10;
    public int alarmduration = 3;

    private System.DateTime timenow;
    private int time = 0;
    private int oldtime = -1;
    public int alarm_1 = 0000;
    public int alarm_2 = 0000;
    public int alarm_3 = 0000;

    private bool pressed = false;
    private GameObject newblockpiece1;
    private GameObject newblockpiece2;
    private float velocity1;
    private float velocity2;
    private bool reset = false;

    private float t = 0.0f;
    private float threshold = 5.0f;
    public bool playonce;
    private int tmpblockcount = 0;
    private Material tmpmaterial;

    public int remainingblocks;

    private bool screenchange = false;
    private float flashtimer = 0;
    private bool colourtoggle = false;
    private Color[] backgroundcolours = new Color[9]{
        Color.black,
        Color.blue,
        Color.cyan,
        Color.gray,
        Color.green,
        Color.magenta,
        Color.red,
        Color.white,
        Color.yellow,
    };

    public Material settingsicon_material;

    void Start()
    {

        GameObject blockparent = new GameObject();
        blockparent.name = "Wall";
        playonce = true;
        //Debug.Log("I am in Start: playonce is " + playonce);

        alarm_1 = PlayerPrefs.GetInt("PlayerPrefs_alarm_1");//temporary
        alarm_2 = PlayerPrefs.GetInt("PlayerPrefs_alarm_2");//temporary
        alarm_3 = PlayerPrefs.GetInt("PlayerPrefs_alarm_3");//temporary

        //lock screen rotation
        Screen.orientation = ScreenOrientation.Portrait;

        //position main camera
        Vector3 temp = Camera.main.transform.position;
        temp.x = Screen.width / 2;
        temp.y = Screen.height / 2;
        temp.z = 1;

        Camera.main.transform.position = temp;
        Camera.main.transform.Rotate(0, 180, 0);

        //set background colour initially
        Camera.main.backgroundColor = Color.black;

        //create wall skeleton
        wall = new GameObject[difficulty, difficulty * 2];//2d array containing all blocks
        float blockwidth = Screen.width / difficulty;
        float blockheight = Screen.height / difficulty / 2;

        //scale maincamera
        Camera.main.orthographicSize = Screen.height / 2 - blockheight;

        //Create Block Prefab (Unbroken)
        GameObject block = new GameObject();
        block.name = "Block";
        Mesh blockmesh = new Mesh();
        blockmesh.name = "Block_Mesh";
        blockmesh.Clear();//clear vertex,triangle data, etc.
        blockmesh.vertices = new Vector3[4]{
            new Vector3 (0, 0, 0),
            new Vector3 (blockwidth, 0, 0),
            new Vector3 (0, blockheight, 0),
            new Vector3 (blockwidth, blockheight, 0)
        };

        blockmesh.uv = new Vector2[4]{
            new Vector2 (0, 0),
            new Vector2 (1, 0),
            new Vector2 (0, 1),
            new Vector2 (1, 1)
        };

        blockmesh.triangles = new int[6] { 0, 1, 2, 1, 3, 2 };
        blockmesh.RecalculateNormals();
        MeshFilter blockmeshfilter = (MeshFilter)block.gameObject.AddComponent(typeof(MeshFilter));
        MeshRenderer blockmeshrenderer = (MeshRenderer)block.gameObject.AddComponent(typeof(MeshRenderer));
        blockmeshfilter.mesh = blockmesh;

        //create colour selection array
        Color32[] colours = new Color32[9]{
            new Color32(246,150,121,1),//red
			new Color32(130,202,156,1),//dark green
			new Color32(131,147,202,1),//dark blue
			new Color32(255,247,153,1),//yellow
			new Color32(095,175,153,1),//green
			new Color32(125,167,217,1),//cyan
			new Color32(161,134,190,1),//violet
			new Color32(244,154,193,1),//magenta
			new Color32(252,186,115,1)//orange
		};

        //default material to apply to blocks
        tmpmaterial = new Material(Shader.Find("Diffuse"));
        tmpmaterial.color = new Color32(255, 255, 255, 1);//white

        //store adjacent blocks
        int blockindex = 0;//give each block group unique identifiers
        GameObject tmpblock1 = block;//right
        GameObject tmpblock2 = block;//down

        GameObject oldname;//used for overwriting block names in cases when both the right and below block are the same colour

        for (int i = 0; (i < difficulty - 1); i++)
        {
            for (int j = 0; (j < difficulty * 2 - 1); j++)
            {
                wall[i, j] = (GameObject)Instantiate(block, Vector3.zero, Quaternion.identity);
                wall[i, j].transform.parent = blockparent.transform;
                wall[i, j].transform.position = new Vector3((i * blockwidth) + blockwidth / 2, (j * blockheight) + blockheight / 2, 0.0f);
                wall[i, j].GetComponent<Renderer>().material.color = colours[Random.Range(0, colorcount)];
                wall[i, j].GetComponent<Renderer>().enabled = false;

                //merge blocks
                if (i >= 1) { tmpblock1 = wall[i - 1, j]; }
                else { tmpblock1 = null; }
                if (j >= 1) { tmpblock2 = wall[i, j - 1]; }
                else { tmpblock2 = null; }
                GameObject[] copies = null;

                //search for all bricks with name of block below, and change it to the block to the right
                if (tmpblock1 != null && tmpblock2 != null && wall[i, j].GetComponent<Renderer>().material.color == tmpblock1.GetComponent<Renderer>().material.color && wall[i, j].GetComponent<Renderer>().material.color == tmpblock2.GetComponent<Renderer>().material.color)
                {
                    wall[i, j].gameObject.tag = tmpblock1.gameObject.tag;
                    if (copies == null)
                    {
                        copies = GameObject.FindGameObjectsWithTag(wall[i, j - 1].gameObject.tag);
                    }

                    foreach (GameObject copy in copies)
                    {
                        copy.tag = wall[i - 1, j].tag;
                    }
                }
                else if (tmpblock1 != null && wall[i, j].GetComponent<Renderer>().material.color == tmpblock1.GetComponent<Renderer>().material.color)
                {
                    wall[i, j].tag = tmpblock1.tag;

                }//down
                else if (tmpblock2 != null && wall[i, j].GetComponent<Renderer>().material.color == tmpblock2.GetComponent<Renderer>().material.color)
                {
                    wall[i, j].tag = tmpblock2.tag;

                }//right
                else {
                    wall[i, j].tag = "" + blockindex;
                    blockindex++;
                }
                //add collider to blocks so they can be destroyed
                BoxCollider2D newcollider = wall[i, j].AddComponent<BoxCollider2D>();
                newcollider.transform.position = wall[i, j].transform.position;
                newcollider.size = wall[i, j].GetComponent<Renderer>().bounds.size;
            }
        }
        Destroy(block);
    }

    void clearscreen(bool mode)
    {//true to erase all, false to create all
        for (int i = 0; (i < difficulty - 1); i++)
        {
            for (int j = 0; (j < difficulty * 2 - 1); j++)
            {
                if (wall[i, j].transform.tag != "Untagged")
                {
                    wall[i, j].GetComponent<Renderer>().enabled = mode;
                }
            }
        }
        remainingblocks = (difficulty - 1) * ((difficulty * 2) - 1);
    }

    void showdigit(int digit, int offset)
    {
        if (offset == 1)
        {
            offset = 1;
        }
        else if (offset == 2)
        {
            offset = 5;
        }
        else if (offset == 3)
        {
            offset = 11;
        }
        else if (offset == 4)
        {
            offset = 15;
        }
        else {
            //Debug("INVALID TIME");
        }

        if (digit == 0)
        {
            wall[2, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[3, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[4, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[5, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[6, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[6, offset + 1].GetComponent<Renderer>().enabled = true;
            wall[6, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[5, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[4, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[3, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[2, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[2, offset + 1].GetComponent<Renderer>().enabled = true;
        }
        else if (digit == 1)
        {
            wall[6, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[5, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[4, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[3, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[2, offset + 2].GetComponent<Renderer>().enabled = true;
        }
        else if (digit == 2)
        {
            wall[2, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[2, offset + 1].GetComponent<Renderer>().enabled = true;
            wall[2, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[3, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[4, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[4, offset + 1].GetComponent<Renderer>().enabled = true;
            wall[4, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[5, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[6, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[6, offset + 1].GetComponent<Renderer>().enabled = true;
            wall[6, offset + 2].GetComponent<Renderer>().enabled = true;
        }
        else if (digit == 3)
        {
            wall[2, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[3, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[4, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[5, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[6, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[6, offset + 1].GetComponent<Renderer>().enabled = true;
            wall[6, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[4, offset + 1].GetComponent<Renderer>().enabled = true;
            wall[4, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[2, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[2, offset + 1].GetComponent<Renderer>().enabled = true;
        }
        else if (digit == 4)
        {
            wall[6, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[5, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[4, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[3, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[2, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[4, offset + 1].GetComponent<Renderer>().enabled = true;
            wall[4, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[5, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[6, offset + 0].GetComponent<Renderer>().enabled = true;
        }
        else if (digit == 5)
        {
            wall[2, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[2, offset + 1].GetComponent<Renderer>().enabled = true;
            wall[2, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[3, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[4, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[4, offset + 1].GetComponent<Renderer>().enabled = true;
            wall[4, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[5, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[6, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[6, offset + 1].GetComponent<Renderer>().enabled = true;
            wall[6, offset + 2].GetComponent<Renderer>().enabled = true;
        }
        else if (digit == 6)
        {
            wall[2, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[3, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[4, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[5, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[6, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[6, offset + 1].GetComponent<Renderer>().enabled = true;
            wall[4, offset + 1].GetComponent<Renderer>().enabled = true;
            wall[6, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[4, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[3, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[2, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[2, offset + 1].GetComponent<Renderer>().enabled = true;
        }
        else if (digit == 7)
        {
            wall[2, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[3, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[4, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[5, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[6, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[6, offset + 1].GetComponent<Renderer>().enabled = true;
            wall[6, offset + 0].GetComponent<Renderer>().enabled = true;
        }
        else if (digit == 8)
        {
            wall[2, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[3, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[4, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[5, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[6, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[6, offset + 1].GetComponent<Renderer>().enabled = true;
            wall[4, offset + 1].GetComponent<Renderer>().enabled = true;
            wall[6, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[5, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[4, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[3, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[2, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[2, offset + 1].GetComponent<Renderer>().enabled = true;
        }
        else if (digit == 9)
        {
            wall[2, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[4, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[5, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[6, offset + 0].GetComponent<Renderer>().enabled = true;
            wall[6, offset + 1].GetComponent<Renderer>().enabled = true;
            wall[4, offset + 1].GetComponent<Renderer>().enabled = true;
            wall[6, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[5, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[4, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[3, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[2, offset + 2].GetComponent<Renderer>().enabled = true;
            wall[2, offset + 1].GetComponent<Renderer>().enabled = true;
        }
    }

    void destroydigit(int digit, int offset)
    {
        if (offset == 1)
        {
            offset = 1;
        }
        else if (offset == 2)
        {
            offset = 5;
        }
        else if (offset == 3)
        {
            offset = 11;
        }
        else if (offset == 4)
        {
            offset = 15;
        }
        else {
            //Debug("INVALID TIME");
        }

        if (digit == 0)
        {
            wall[2, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[3, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[5, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 1].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[5, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[3, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[2, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[2, offset + 1].GetComponent<Renderer>().enabled = false;
            remainingblocks -= 12;
        }
        else if (digit == 1)
        {
            wall[6, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[5, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[3, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[2, offset + 2].GetComponent<Renderer>().enabled = false;
            remainingblocks -= 5;
        }
        else if (digit == 2)
        {
            wall[2, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[2, offset + 1].GetComponent<Renderer>().enabled = false;
            wall[2, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[3, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 1].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[5, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 1].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 2].GetComponent<Renderer>().enabled = false;
            remainingblocks -= 11;
        }
        else if (digit == 3)
        {
            wall[2, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[3, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[5, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 1].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 1].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[2, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[2, offset + 1].GetComponent<Renderer>().enabled = false;
            remainingblocks -= 11;
        }
        else if (digit == 4)
        {
            wall[6, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[5, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[3, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[2, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 1].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[5, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 0].GetComponent<Renderer>().enabled = false;
            remainingblocks -= 9;
        }
        else if (digit == 5)
        {
            wall[2, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[2, offset + 1].GetComponent<Renderer>().enabled = false;
            wall[2, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[3, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 1].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[5, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 1].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 2].GetComponent<Renderer>().enabled = false;
            remainingblocks -= 11;
        }
        else if (digit == 6)
        {
            wall[2, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[3, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[5, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 1].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 1].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[3, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[2, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[2, offset + 1].GetComponent<Renderer>().enabled = false;
            remainingblocks -= 12;
        }
        else if (digit == 7)
        {
            wall[2, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[3, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[5, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 1].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 0].GetComponent<Renderer>().enabled = false;
            remainingblocks -= 7;
        }
        else if (digit == 8)
        {
            wall[2, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[3, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[5, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 1].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 1].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[5, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[3, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[2, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[2, offset + 1].GetComponent<Renderer>().enabled = false;
            remainingblocks -= 13;
        }
        else if (digit == 9)
        {
            wall[2, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[5, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 1].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 1].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[5, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[3, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[2, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[2, offset + 1].GetComponent<Renderer>().enabled = false;
            remainingblocks -= 12;
        }
        wall[3, 9].GetComponent<Renderer>().enabled = false;//Colon
        wall[5, 9].GetComponent<Renderer>().enabled = false;//Colon
        if (offset == 1)
        {//ensure that the colon is only subtracted once
            remainingblocks -= 2;
        }
    }

    void showtime(int time)
    {
        int digit1 = time / 1000;
        int digit2 = time % 1000 / 100;
        int digit3 = time % 100 / 10;
        int digit4 = time % 10;

        showdigit(digit1, 1);
        showdigit(digit2, 2);
        showdigit(digit3, 3);
        showdigit(digit4, 4);

        wall[3, 9].GetComponent<Renderer>().enabled = true;//Colon
        wall[5, 9].GetComponent<Renderer>().enabled = true;//Colon

        wall[0, 0].GetComponent<Renderer>().enabled = true;//Settings Button
                                                           //wall[0,0].GetComponent<Renderer> ().material=settingsicon_material;
                                                           //wall[0,0].GetComponent<Renderer> ().material=settingsicon_material;//settingsicon_material;

        wall[0, 18].GetComponent<Renderer>().enabled = true;//Date
        wall[0, 17].GetComponent<Renderer>().enabled = true;//Date
        wall[0, 16].GetComponent<Renderer>().enabled = true;//Date

        /*
		wall[8,7].GetComponent<Renderer>().enabled=true;
		wall[8,8].GetComponent<Renderer>().enabled=true;
		wall[8,9].GetComponent<Renderer>().enabled=true;

		wall[0,10].GetComponent<Renderer>().enabled=true;
		wall[0,11].GetComponent<Renderer>().enabled=true;

		wall[8,14].GetComponent<Renderer>().enabled=true;
		wall[8,15].GetComponent<Renderer>().enabled=true;
		*/
    }

    void destroytime(int time)
    {

        int digit1 = time / 1000;
        int digit2 = time % 1000 / 100;
        int digit3 = time % 100 / 10;
        int digit4 = time % 10;

        destroydigit(digit1, 1);
        destroydigit(digit2, 2);
        destroydigit(digit3, 3);
        destroydigit(digit4, 4);

    }

    void Update()
    {
        //Debug.Log("I am in Update - at the beginning : playonce is " + playonce);
        t += Time.deltaTime;

        timenow = System.DateTime.Now;
        time = timenow.Hour * 100 + timenow.Minute;

        if (alarmstate == true && remainingblocks <= 0)
        {
            clearscreen(alarmstate);//true
            Camera.main.backgroundColor = Color.black;
            showtime(time);
            oldtime = time;
            //Debug.Log("One");
        }

        if (alarmstate == true && time >= oldtime + alarmduration)
        {
            alarmstate = false;
            //Debug.Log("Two");
        }

        if (alarmstate == false && (time == alarm_1 || time == alarm_2 || time == alarm_3))
        {
            //Debug.Log ("Alarm Playing Now");

            if (playonce == true)
            {
                AudioSource.PlayClipAtPoint(Alarm_Sound, Vector3.zero);
                playonce = false;

                if (t > threshold)
                {
                    t = 0.0f;
                    playonce = true;
                }
                else {
                    playonce = false;
                }

            }
            alarmstate = true;
            clearscreen(!alarmstate);//false
            clearscreen(alarmstate);//true
            destroytime(time);
        }
        else if (alarmstate == false && oldtime != time)
        {
            Debug.Log("no alarm triggered");
            clearscreen(alarmstate);//false
            Camera.main.backgroundColor = Color.black;
            showtime(time);
            oldtime = time;
        }
        else {
            return;//do nothing
        }

        if (colourtoggle == true)
        {
            randcolour = Random.Range(0, 8);
            if (randcolour == oldcolour)
            {
                if (randcolour == 8)
                {
                    randcolour--;
                }
                else {
                    randcolour++;
                }
            }
            Camera.main.backgroundColor = backgroundcolours[randcolour];
            colourtoggle = false;
            oldcolour = randcolour;
        }
        else {
            //audio.Stop();
            //Camera.main.backgroundColor = Color.blue;
        }

        if (alarmstate == true)
        {
            flashtimer += Time.deltaTime;//time passed since last screen refresh
            if (flashtimer > 1.5f)
            {
                colourtoggle = true;
                GetComponent<AudioSource>().Play();
                flashtimer = 0;
            }
            else {
                colourtoggle = false;
            }
        }

        else {
            showtime(time);
        }

        //Section to handle user destroying blocks
        if (alarmstate == true)
        {
            if (Input.touchCount == 3 && reset == false)
            {
                reset = true;
                Application.LoadLevel(0);
            }
            else if (Input.touchCount == 0)
            {
                reset = false;
            }
            feckoff();//rotate block pieces and fire them off screen
            GameObject[] copies = null;

            if (Input.touchCount == 1 && pressed == false)
            {
                pressed = true;
                RaycastHit2D destroyedblock = Physics2D.Raycast(Camera.main.ScreenToWorldPoint((Input.GetTouch(0).position)), Vector2.zero);
                if (destroyedblock.collider != null)
                {

                    //obsolete, object not rendered now instead
                    //Destroy (destroyedblock.transform.gameObject, 0.0f);//0.5f -> destroy block half a second later
                    //spawn(destroyedblock.transform);

                    if (copies == null)
                    {
                        copies = GameObject.FindGameObjectsWithTag(destroyedblock.transform.tag);
                    }

                    foreach (GameObject copy in copies)
                    {
                        tmpblockcount++;
                        copy.GetComponent<Renderer>().enabled = false;
                        copy.transform.tag = "Untagged";

                        //spawn(copy.transform);
                    }
                    remainingblocks -= tmpblockcount;
                    tmpblockcount = 0;

                }
            }
            else if (Input.touchCount == 0)
            {
                pressed = false;
            }

            //used for debugging only
            if (Input.GetButtonDown("Fire1"))
            {
                pressed = true;
                RaycastHit2D destroyedblock = Physics2D.Raycast(new Vector2(Screen.width - Input.mousePosition.x, Input.mousePosition.y), Vector2.zero);
                if (destroyedblock.collider != null)
                {

                    //obsolete, object not rendered now instead
                    //Destroy (destroyedblock.transform.gameObject, 0.0f);//0.5f -> destroy block half a second later
                    //spawn(destroyedblock.transform);

                    if (copies == null)
                    {
                        copies = GameObject.FindGameObjectsWithTag(destroyedblock.transform.tag);
                    }

                    foreach (GameObject copy in copies)
                    {
                        tmpblockcount++;
                        copy.GetComponent<Renderer>().enabled = false;
                        copy.transform.tag = "Untagged";

                        //spawn(copy.transform);
                    }
                    remainingblocks -= tmpblockcount;
                    tmpblockcount = 0;

                }
            }
            else {
                pressed = false;
            }

        }
    }

    void spawn(Transform blockposition)
    {
        GameObject blockpiece1 = new GameObject();
        GameObject blockpiece2 = new GameObject();
        //GameObject blockpiece3 = new GameObject();
        Mesh blockpiece1mesh = new Mesh();
        Mesh blockpiece2mesh = new Mesh();
        //Mesh blockpiece3mesh = new Mesh ();

        blockpiece1mesh.Clear();
        blockpiece1mesh.vertices = new Vector3[3]{
            new Vector3 (0, 0, 20),
            new Vector3 (Screen.width/difficulty, 0, 20),
            new Vector3 (0, Screen.height/difficulty, 20)
        };
        blockpiece1mesh.uv = new Vector2[3]{
            new Vector2 (0, 0),
            new Vector2 (1, 0),
            new Vector2 (0, 1),

        };
        blockpiece1mesh.triangles = new int[3]{
            0,1,2
        };
        blockpiece1mesh.RecalculateNormals();
        MeshFilter blockpiece1meshfilter = (MeshFilter)blockpiece1.gameObject.AddComponent(typeof(MeshFilter));
        MeshRenderer blockpiece1meshrenderer = (MeshRenderer)blockpiece1.gameObject.AddComponent(typeof(MeshRenderer));
        //blockmeshrenderer.enabled = false;//get rid of that fucking annoying pink bastard
        blockpiece1meshfilter.mesh = blockpiece1mesh;
        //
        blockpiece2mesh.Clear();
        blockpiece2mesh.vertices = new Vector3[3]{
            new Vector3 (Screen.width/difficulty, 0, 20),
            new Vector3 (Screen.width/difficulty, Screen.height/difficulty, 20),
            new Vector3 (0, Screen.height/difficulty, 20)
        };
        blockpiece2mesh.uv = new Vector2[3]{
            new Vector2 (0, 0),
            new Vector2 (1, 0),
            new Vector2 (0, 1),

        };
        blockpiece2mesh.triangles = new int[3]{
            0,1,2
        };
        blockpiece2mesh.RecalculateNormals();
        MeshFilter blockpiece2meshfilter = (MeshFilter)blockpiece2.gameObject.AddComponent(typeof(MeshFilter));
        MeshRenderer blockpiece2meshrenderer = (MeshRenderer)blockpiece2.gameObject.AddComponent(typeof(MeshRenderer));
        //blockmeshrenderer.enabled = false;//get rid of that fucking annoying pink bastard
        blockpiece2meshfilter.mesh = blockpiece2mesh;
        //
        //blockpiece3mesh.Clear ();

        //spawn pieces
        newblockpiece1 = (GameObject)Instantiate(blockpiece1, blockposition.position, Quaternion.identity);
        newblockpiece1.GetComponent<Renderer>().material.color = blockposition.GetComponent<Renderer>().material.color;
        velocity1 = Random.Range(200, 250);
        Destroy(newblockpiece1, 0.5f);

        newblockpiece2 = (GameObject)Instantiate(blockpiece2, blockposition.position, Quaternion.identity);
        newblockpiece2.GetComponent<Renderer>().material.color = blockposition.GetComponent<Renderer>().material.color;
        velocity2 = Random.Range(-200, -250);
        Destroy(newblockpiece2, 0.5f);
    }

    void feckoff()
    {
        if (newblockpiece1)
        {
            newblockpiece1.transform.Rotate(newblockpiece1.transform.position, velocity1 * Time.deltaTime);
            newblockpiece1.transform.Translate(Vector2.right * velocity1 * Time.deltaTime);
        }
        if (newblockpiece2)
        {
            newblockpiece2.transform.Rotate(newblockpiece2.transform.position, velocity2 * Time.deltaTime);
            newblockpiece2.transform.Translate(Vector2.right * velocity2 * Time.deltaTime);
        }
    }
}

/*
Game Mode 1
Entire Screen flashes once with one colour, e.g. blue
Screen then randomly generates blocks and shows them for a short time
screen then greys out the blocks and user has to find all of the blue blocks
Points are gained for every correct selection and lost for every incorrect one
round ends when all blue blocks are found

Game Mode 2
Entire screen Flashes once with one colour, e.g. red
Screen then rapidly regenerating random blocks and player has to try and tap a red block before it changes again
(this can be fairly intense on resources with large numbers of blocks)

Game Mode 3
Entire Screen flashes with a sequence of colours e.g. red->green->blue
then blocks are generated randomly(different than before, all red blocks will be clumped together, all reds will have an adjacent red block to form one sprawling red island)
blocks are then greyed out as above, although all reds will be same shade of grey.
player must select the block groups in correct order.
note the wall can also contain colours that are not in the sequence

Game Mode 4
Screen is randomly generated again, player has to clear the screen as fast as possible, destroying all blocks

Game mode N
(combination of all of the above)
game modes are chosen at random and are distinguished by a number of beeps, e.g. two beeps for GM2
after user completes one of the above game modes (while in mode 5) the next one is randomly chosen again
(scores carry over of course)

*/
