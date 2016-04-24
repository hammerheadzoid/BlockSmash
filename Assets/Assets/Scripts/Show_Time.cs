using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Show_Time : MonoBehaviour
{

    public AudioClip Alarm_Sound;

    private int difficulty = 10;// don't change, screen scaling depends on this for the alarm clock functionality (value should be 10)
    public int colorcount = 5;//with higher difficulties, colorcount can be increased to include more colours, (max is 7 for now)
    private int randcolour;
    private int oldcolour;
    private bool alarmstate;
    private bool alarmfinished;//used to grey out the screen when the alarm is finished
    private Material tmpmaterial;//used to to assign random block colours when wall is being built, also to store rgb values of each block to create the greyscale equivalent when alarm is finished (turned off by user)
    public GameObject[,] wall;
    public int snooze = 10;
    public int alarmduration = 3;

    private System.DateTime timenow;
    public int time = 0;
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
    private const int backgroundarraysize = 8;
    private const int blockcolourarraysize = 9;

    //used to distinguish between greyscale equivalents (https://en.wikipedia.org/wiki/Luma_(video)
    private const float redweight=0.2989f;
    private const float greenweight = 0.5870f;
    private const float blueweight = 0.01140f;

    private bool greyscaleflag;//used to execute code only once per alarm, when setting greyscale
    private bool cleargreyscaleflag;//used to execute code only once per alarm, when turning off greyscale, two flags required to prevent flickering

    public int backgroundblocks;
    public int remainingblocks;
    private bool alarminitialtrigger;
    public Transform MainScreen_Button;

    private bool screenchange = false;
    private float flashtimer = 0;
    private bool colourtoggle = false;

    private float blockwidth;
    private float blockheight;

    private GameObject blockparent;
    private GameObject block;
    private Mesh blockmesh;
    private MeshFilter blockmeshfilter;
    private MeshRenderer blockmeshrenderer;
    private GameObject tmpblock1;
    private GameObject tmpblock2;
    private int blockindex;
    private BoxCollider2D newcollider;
    private GameObject[] copies;
    RaycastHit2D destroyedblock;

    //create colour selection array
    private Color32[] colours = new Color32[blockcolourarraysize]{
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

    public Material settingsicon_material;

    void Start(){
        alarmstate = false;
        alarmfinished = false;
        greyscaleflag = false;
        cleargreyscaleflag = false;
        //lock screen rotation
        Screen.orientation = ScreenOrientation.Portrait;

        //position main camera
        Vector3 temp = Camera.main.transform.position;
        temp.x = Screen.width / 2;
        temp.y = Screen.height / 2;
        temp.z = 10;

        Camera.main.transform.position = temp;
        Camera.main.transform.Rotate(0, 180, 0);

        //set background colour initially
        Camera.main.backgroundColor = new Color(0.4f, 0.4f, 0.4f);

        //set alarm trigger flag, used to execute code once per alarm activation
        alarminitialtrigger = true;

        //create wall skeleton
        wall = new GameObject[difficulty, difficulty * 2];//2d array containing all blocks
        blockwidth = Screen.width / difficulty;
        blockheight = Screen.height / difficulty / 2;

        //scale maincamera
        Camera.main.orthographicSize = Screen.height / 2 - blockheight;

        //create wall
        blockparent = new GameObject();
        blockparent.name = "Wall";
        playonce = true;
        //Debug.Log("I am in Start: playonce is " + playonce);

        alarm_1 = PlayerPrefs.GetInt("PlayerPrefs_alarm_1");//temporary
        alarm_2 = PlayerPrefs.GetInt("PlayerPrefs_alarm_2");//temporary
        alarm_3 = PlayerPrefs.GetInt("PlayerPrefs_alarm_3");//temporary


        //Create Block Prefab (Unbroken)
        block = new GameObject();
        block.name = "Block";
        blockmesh = new Mesh();
        blockmesh.name = "Block_Mesh";
        blockmesh.Clear();//clear vertex,triangle data, etc.
        blockmesh.vertices = new Vector3[4]{
            new Vector3 (0, 0, 0),
            new Vector3 (blockwidth, 0, 0),
            new Vector3 (0, blockheight, 0),
            new Vector3 (blockwidth, blockheight, 0)
        };

        //Set up block unit vectors
        blockmesh.uv = new Vector2[4]{
            new Vector2 (0, 0),
            new Vector2 (1, 0),
            new Vector2 (0, 1),
            new Vector2 (1, 1)
        };

        //setupscreen up block triangles for rendering
        blockmesh.triangles = new int[6] { 0, 1, 2, 1, 3, 2 };
        blockmesh.RecalculateNormals();
        blockmeshfilter = (MeshFilter)block.gameObject.AddComponent(typeof(MeshFilter));
        blockmeshrenderer = (MeshRenderer)block.gameObject.AddComponent(typeof(MeshRenderer));
        blockmeshfilter.mesh = blockmesh;

        //default material to apply to blocks
        tmpmaterial = new Material(Shader.Find("Diffuse"));
        tmpmaterial.color = new Color32(255, 255, 255, 1);//white

        for (int i = 0; (i < difficulty - 1); i++)
        {
            for (int j = 0; (j < difficulty * 2 - 1); j++)
            {
                //create wall of blocks by instantiating the prefab created earlier
                wall[i, j] = (GameObject)Instantiate(block, Vector3.zero, Quaternion.identity);
                wall[i, j].transform.parent = blockparent.transform;
                wall[i, j].transform.position = new Vector3((i * blockwidth) + blockwidth / 2, (j * blockheight) + blockheight / 2, 0.0f);

                //add collider to blocks so they can be destroyed
                newcollider = wall[i, j].AddComponent<BoxCollider2D>();
                newcollider.transform.position = wall[i, j].transform.position;
                newcollider.size = wall[i, j].GetComponent<Renderer>().bounds.size;
            }
        }
        //set time variable initially
        timenow = System.DateTime.Now;
        time = timenow.Hour * 100 + timenow.Minute;
        oldtime = time;

        setupscreen();
        Destroy(block);//delete prefab as its no longer needed
    }

    void Update()
    {
        //Debug.Log("I am in Update - at the beginning : playonce is " + playonce);
        t += Time.deltaTime;

        timenow = System.DateTime.Now;
        time = timenow.Hour * 100 + timenow.Minute;

        if (alarmstate == true)
        {
            if (remainingblocks >= 1)
            {
                if (Input.touchCount == 1 && pressed == false)
                {
                    copies = null;
                    pressed = true;
                    destroyedblock = Physics2D.Raycast(Camera.main.ScreenToWorldPoint((Input.GetTouch(0).position)), Vector2.zero);
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
                        Debug.Log("tmpblockcount: " + tmpblockcount);
                        remainingblocks -= tmpblockcount;
                        tmpblockcount = 0;

                    }
                }
                else if (Input.touchCount == 0)
                {
                    pressed = false;
                }

                //used for pc debugging only
                if (Input.GetButton("Fire1"))//GetButtonDown for more realistic single click (GetButton is faster for testing)
                {
                    copies = null;
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
                        Debug.Log("backgroundblocks: " + backgroundblocks + ". remainingblocks: " + remainingblocks);
                        tmpblockcount = 0;

                    }
                }
                else {
                    pressed = false;
                }

                if (remainingblocks <= 0)//will execute once per alarm, as it is closing
                {
                    setupscreen();//required for resetting tags
                    alarmfinished = true;
                    Camera.main.backgroundColor = new Color(0.4f, 0.4f, 0.4f);
                    oldtime = time;
                    //remainingblocks = backgroundblocks;
                    //return;
                    //Debug.Log("One");
                }

                //this will turn off the alarm after a few minutes, check above, i'm not arsed. we will change this to a customisable number
                if (time%oldtime>=alarmduration)
                {
                    alarmstate = false;
                    //Debug.Log("Two");
                }

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

                if (Input.touchCount == 3 && reset == false)
                {
                    reset = true;
                    //Application.LoadLevel(0);
                }
                else if (Input.touchCount == 0)
                {
                    reset = false;
                }
                feckoff();//rotate block pieces and fire them off screen

            }
            else {//remainingblocks<=1
                alarmstate = false;//trun off alarm
                                   //setupscreen();

                //clearscreen(alarmstate);
            }
        }
        else {//alarm state is false
            if (alarmfinished == true)
            {
                if (time==oldtime)//add in alarm duration functionality here, for now it turns off after 1 minute
                {
                    clearscreen(true);
                    if (greyscaleflag == false) {
                        makegreyscale();
                        greyscaleflag = true;
                    }
                }
                else
                {
                    if (cleargreyscaleflag == false)
                    {
                        Debug.Log("clearing greyscale (ONCE ONLY PER ALARM)");
                        for (int i = 0; (i < difficulty - 1); i++)
                        {
                            for (int j = 0; (j < difficulty * 2 - 1); j++)
                            {
                                if (wall[i, j].transform.tag != "Untagged")
                                {
                                    wall[i, j].GetComponent<Renderer>().enabled = true;
                                }
                            }
                        }
                        setupscreen();
                        clearscreen(false);
                        cleargreyscaleflag = true;
                    }
                    
                    //clearscreen(true);
                }
            }
            else {
                if (time == alarm_1 || time == alarm_2 || time == alarm_3)
                {
                    alarmstate = true;
                    clearscreen(true);

                    //trigger once per alarm activation
                    if (alarminitialtrigger == true)
                    {
                        Debug.Log("initial alarm trigger activated (ONCE ONLY PER ALARM)");
                        remainingblocks = backgroundblocks;//remainingblocks will be changed when the user destroys 1 or more blocks but initially, depending on the current time, the app will destroy some blocks to make the digits visible, this is our starting point
                        alarminitialtrigger = false;
                    }
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
                }
                if (oldtime != time)
                {
                    Debug.Log("no alarm triggered");
                    clearscreen(alarmstate);//false
                    Camera.main.backgroundColor = new Color(0.4f, 0.4f, 0.4f);
                    oldtime = time;
                }
            }
        }

        //change background colour randomly while alarm is on
        if (colourtoggle == true)
        {
            randcolour = Random.Range(0, backgroundarraysize - 1);
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
            Camera.main.backgroundColor = colours[Random.Range(0, colorcount)];
            colourtoggle = false;
            oldcolour = randcolour;
        }
        else {
            //audio.Stop();
            //Camera.main.backgroundColor = Color.blue;
        }
    }

    void makegreyscale(){
        Camera.main.backgroundColor = new Color(0.4f,0.4f,0.4f);
        for (int i = 0; (i < difficulty - 1); i++)
        {
            for (int j = 0; (j < difficulty * 2 - 1); j++)
            {
                if (wall[i, j] != null) {
                    float greyvalue = (wall[i, j].GetComponent<Renderer>().material.color.r + wall[i, j].GetComponent<Renderer>().material.color.g + wall[i, j].GetComponent<Renderer>().material.color.b) / 3;
                    wall[i, j].GetComponent<Renderer>().material.color = new Color(greyvalue, greyvalue, greyvalue);
            }
                //wall[i, j].GetComponent<Renderer>().material.color= new Color(wall[i, j].GetComponent<Renderer>().material.color.grayscale*redweight, wall[i, j].GetComponent<Renderer>().material.color.grayscale*greenweight, wall[i, j].GetComponent<Renderer>().material.color.grayscale*blueweight);
                //wall[i, j].GetComponent<Renderer>().enabled = true;
                
            }
        }
    }

    void setupscreen() {
    
        //store adjacent blocks
        blockindex = 0;//give each block group unique identifiers
        tmpblock1 = block;//right
        tmpblock2 = block;//down

        wall[0,0].tag = "" + blockindex;

        for (int i = 0; (i < difficulty - 1); i++)
        {
            for (int j = 0; (j < difficulty * 2 - 1); j++)
            {
                wall[i, j].GetComponent<Renderer>().material.color = colours[Random.Range(0, colorcount)];
                wall[i, j].GetComponent<Renderer>().material.shader = Shader.Find("Standard");
                wall[i, j].GetComponent<Renderer>().enabled = true;

                //merge blocks
                if (i >= 1) { tmpblock1 = wall[i - 1, j]; }
                else { tmpblock1 = null; }
                if (j >= 1) { tmpblock2 = wall[i, j - 1]; }
                else { tmpblock2 = null; }
                copies = null;

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
            }
        }
        clearscreen(false);
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

        backgroundblocks = (difficulty - 1) * ((difficulty * 2) - 1);
        if (mode == false)
        {
            MainScreen_Button.GetComponent<Button>().interactable = true;
            showtime(time);
        }
        else {
            MainScreen_Button.GetComponent<Button>().interactable = false;
            destroytime(time);
        }
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

        //the colon (:) between hours and minutes accounts for the extra jump in offset

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
            //disable object rendering
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

            //untag removed blocks
            wall[2, offset + 0].transform.tag = "Untagged";
            wall[3, offset + 0].transform.tag = "Untagged";
            wall[4, offset + 0].transform.tag = "Untagged";
            wall[5, offset + 0].transform.tag = "Untagged";
            wall[6, offset + 0].transform.tag = "Untagged";
            wall[6, offset + 1].transform.tag = "Untagged";
            wall[6, offset + 2].transform.tag = "Untagged";
            wall[5, offset + 2].transform.tag = "Untagged";
            wall[4, offset + 2].transform.tag = "Untagged";
            wall[3, offset + 2].transform.tag = "Untagged";
            wall[2, offset + 2].transform.tag = "Untagged";
            wall[2, offset + 1].transform.tag = "Untagged";

            //keep track of auto-depleted blocks
            backgroundblocks -= 12;
        }
        else if (digit == 1)
        {
            //disable object rendering
            wall[6, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[5, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[3, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[2, offset + 2].GetComponent<Renderer>().enabled = false;

            //untag removed blocks
            wall[6, offset + 2].transform.tag = "Untagged";
            wall[5, offset + 2].transform.tag = "Untagged";
            wall[4, offset + 2].transform.tag = "Untagged";
            wall[3, offset + 2].transform.tag = "Untagged";
            wall[2, offset + 2].transform.tag = "Untagged";

            //keep track of auto-depleted blocks
            backgroundblocks -= 5;
        }
        else if (digit == 2)
        {
            //disable object rendering
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

            //untag removed blocks
            wall[2, offset + 0].transform.tag = "Untagged";
            wall[2, offset + 1].transform.tag = "Untagged";
            wall[2, offset + 2].transform.tag = "Untagged";
            wall[3, offset + 0].transform.tag = "Untagged";
            wall[4, offset + 0].transform.tag = "Untagged";
            wall[4, offset + 1].transform.tag = "Untagged";
            wall[4, offset + 2].transform.tag = "Untagged";
            wall[5, offset + 2].transform.tag = "Untagged";
            wall[6, offset + 0].transform.tag = "Untagged";
            wall[6, offset + 1].transform.tag = "Untagged";
            wall[6, offset + 2].transform.tag = "Untagged";

            //keep track of auto-depleted blocks
            backgroundblocks -= 11;
        }
        else if (digit == 3)
        {
            //disable object rendering
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

            //untag removed blocks
            wall[2, offset + 0].transform.tag = "Untagged";
            wall[3, offset + 2].transform.tag = "Untagged";
            wall[4, offset + 0].transform.tag = "Untagged";
            wall[5, offset + 2].transform.tag = "Untagged";
            wall[6, offset + 0].transform.tag = "Untagged";
            wall[6, offset + 1].transform.tag = "Untagged";
            wall[6, offset + 2].transform.tag = "Untagged";
            wall[4, offset + 1].transform.tag = "Untagged";
            wall[4, offset + 2].transform.tag = "Untagged";
            wall[2, offset + 2].transform.tag = "Untagged";
            wall[2, offset + 1].transform.tag = "Untagged";

            //keep track of auto-depleted blocks
            backgroundblocks -= 11;
        }
        else if (digit == 4)
        {
            //disable object rendering
            wall[6, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[5, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[3, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[2, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 1].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[5, offset + 0].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 0].GetComponent<Renderer>().enabled = false;

            //untag removed blocks
            wall[6, offset + 2].transform.tag = "Untagged";
            wall[5, offset + 2].transform.tag = "Untagged";
            wall[4, offset + 2].transform.tag = "Untagged";
            wall[3, offset + 2].transform.tag = "Untagged";
            wall[2, offset + 2].transform.tag = "Untagged";
            wall[4, offset + 1].transform.tag = "Untagged";
            wall[4, offset + 0].transform.tag = "Untagged";
            wall[5, offset + 0].transform.tag = "Untagged";
            wall[6, offset + 0].transform.tag = "Untagged";

            //keep track of auto-depleted blocks
            backgroundblocks -= 9;
        }
        else if (digit == 5)
        {
            //disable object rendering
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

            //untag removed blocks
            wall[2, offset + 0].transform.tag = "Untagged";
            wall[2, offset + 1].transform.tag = "Untagged";
            wall[2, offset + 2].transform.tag = "Untagged";
            wall[3, offset + 2].transform.tag = "Untagged";
            wall[4, offset + 0].transform.tag = "Untagged";
            wall[4, offset + 1].transform.tag = "Untagged";
            wall[4, offset + 2].transform.tag = "Untagged";
            wall[5, offset + 0].transform.tag = "Untagged";
            wall[6, offset + 0].transform.tag = "Untagged";
            wall[6, offset + 1].transform.tag = "Untagged";
            wall[6, offset + 2].transform.tag = "Untagged";

            //keep track of auto-depleted blocks
            backgroundblocks -= 11;
        }
        else if (digit == 6)
        {
            //disable object rendering
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

            //untag removed blocks
            wall[2, offset + 0].transform.tag = "Untagged";
            wall[3, offset + 0].transform.tag = "Untagged";
            wall[4, offset + 0].transform.tag = "Untagged";
            wall[5, offset + 0].transform.tag = "Untagged";
            wall[6, offset + 0].transform.tag = "Untagged";
            wall[6, offset + 1].transform.tag = "Untagged";
            wall[4, offset + 1].transform.tag = "Untagged";
            wall[6, offset + 2].transform.tag = "Untagged";
            wall[4, offset + 2].transform.tag = "Untagged";
            wall[3, offset + 2].transform.tag = "Untagged";
            wall[2, offset + 2].transform.tag = "Untagged";
            wall[2, offset + 1].transform.tag = "Untagged";

            //keep track of auto-depleted blocks
            backgroundblocks -= 12;
        }
        else if (digit == 7)
        {
            //disable object rendering
            wall[2, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[3, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[4, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[5, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 2].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 1].GetComponent<Renderer>().enabled = false;
            wall[6, offset + 0].GetComponent<Renderer>().enabled = false;

            //untag removed blocks
            wall[2, offset + 2].transform.tag = "Untagged";
            wall[3, offset + 2].transform.tag = "Untagged";
            wall[4, offset + 2].transform.tag = "Untagged";
            wall[5, offset + 2].transform.tag = "Untagged";
            wall[6, offset + 2].transform.tag = "Untagged";
            wall[6, offset + 1].transform.tag = "Untagged";
            wall[6, offset + 0].transform.tag = "Untagged";

            //keep track of auto-depleted blocks
            backgroundblocks -= 7;
        }
        else if (digit == 8)
        {
            //disable object rendering
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

            //untag removed blocks
            wall[2, offset + 0].transform.tag = "Untagged";
            wall[3, offset + 0].transform.tag = "Untagged";
            wall[4, offset + 0].transform.tag = "Untagged";
            wall[5, offset + 0].transform.tag = "Untagged";
            wall[6, offset + 0].transform.tag = "Untagged";
            wall[6, offset + 1].transform.tag = "Untagged";
            wall[4, offset + 1].transform.tag = "Untagged";
            wall[6, offset + 2].transform.tag = "Untagged";
            wall[5, offset + 2].transform.tag = "Untagged";
            wall[4, offset + 2].transform.tag = "Untagged";
            wall[3, offset + 2].transform.tag = "Untagged";
            wall[2, offset + 2].transform.tag = "Untagged";
            wall[2, offset + 1].transform.tag = "Untagged";

            //keep track of auto-depleted blocks
            backgroundblocks -= 13;
        }
        else if (digit == 9)
        {
            //disable object rendering
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

            //untag removed blocks
            wall[2, offset + 0].transform.tag = "Untagged";
            wall[4, offset + 0].transform.tag = "Untagged";
            wall[5, offset + 0].transform.tag = "Untagged";
            wall[6, offset + 0].transform.tag = "Untagged";
            wall[6, offset + 1].transform.tag = "Untagged";
            wall[4, offset + 1].transform.tag = "Untagged";
            wall[6, offset + 2].transform.tag = "Untagged";
            wall[5, offset + 2].transform.tag = "Untagged";
            wall[4, offset + 2].transform.tag = "Untagged";
            wall[3, offset + 2].transform.tag = "Untagged";
            wall[2, offset + 2].transform.tag = "Untagged";
            wall[2, offset + 1].transform.tag = "Untagged";

            //keep track of auto-depleted blocks
            backgroundblocks -= 12;
        }
        //disable object rendering
        wall[3, 9].GetComponent<Renderer>().enabled = false;//Colon
        wall[5, 9].GetComponent<Renderer>().enabled = false;//Colon

        //untag removed blocks
        wall[3, 9].transform.tag = "Untagged";
        wall[5, 9].transform.tag = "Untagged";

        //keep track of auto-depleted blocks
        if (offset == 1){//ensure that the colon is only subtracted once
            backgroundblocks -= 2;
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

        blockpiece1meshfilter.mesh = blockpiece1mesh;

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

        blockpiece2meshfilter.mesh = blockpiece2mesh;

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
