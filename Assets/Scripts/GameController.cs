using UnityEngine;
using GoogleMobileAds.Api;

public class GameController : MonoBehaviour
{
    const float EPSILON = 0.1f;
    int[,,] value;
    bool needUpdate = false;
    bool inGame = false;
    GameObject textPrefab;
    GameObject[] numList;
    GameObject[] cubeList;
    GameObject endGameUICanvas;
    Material[] materialList;
    Animator[] animatorList;
    int[] animatorStateList;

    private BannerView bannerView;
    private void RequestBanner()
    {
        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(Constants.bannerId, AdSize.SmartBanner, AdPosition.BottomLeft);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);
    }

    Color[] colors = {
        new Color32(192, 180, 164, 96),
        new Color32(255, 0 , 0, 96),
        new Color32(255, 127, 0, 96),
        new Color32(255, 255, 0, 96),
        new Color32(0, 255, 0, 96),
        new Color32(0, 0, 255, 96),
        new Color32(75, 0, 130, 96),
        new Color32(148, 0, 211, 96),
        new Color32(255, 0 , 0, 128),
        new Color32(255, 127, 0, 128),
        new Color32(255, 255, 0, 128),
        new Color32(0, 255, 0, 128),
        new Color32(0, 0, 255, 128),
        new Color32(75, 0, 130, 128),
        new Color32(148, 0, 211, 128),
        new Color32(255, 0 , 0, 192),
        new Color32(255, 127, 0, 192),
        new Color32(255, 255, 0, 192),
        new Color32(0, 255, 0, 192),
        new Color32(0, 0, 255, 192),
        new Color32(75, 0, 130, 192),
        new Color32(148, 0, 211, 192),
    };

    void Start()
    {
        MobileAds.Initialize(Constants.appId);
        RequestBanner();

        textPrefab = GameObject.Find("Text Prefab");
        endGameUICanvas = GameObject.Find("EndGameUICanvas");
        cubeList = new GameObject[27];
        materialList = new Material[27];
        animatorList = new Animator[27];
        for (int i = 0; i < 3; ++i)
        {
            for (int j = 0; j < 3; ++j)
            {
                for (int k = 0; k < 3; ++k)
                {
                    cubeList[9 * i + 3 * j + k] = GameObject.Find("Cube " + i + "_" + j + "_" + k);
                    materialList[9 * i + 3 * j + k] = cubeList[9 * i + 3 * j + k].GetComponent<Renderer>().material;
                    animatorList[9 * i + 3 * j + k] = cubeList[9 * i + 3 * j + k].GetComponent<Animator>();
                }
            }
        }
        numList = new GameObject[27];

        Restart();
    }

    // Update is called once per frame
    void Update()
    {
        if (inGame && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                if (!raycastHit.transform.name.Contains("Capsule"))
                {
                    //do nothing
                }
                else if (System.Math.Abs(raycastHit.collider.transform.localPosition.x) > EPSILON)
                {
                    Move(0, raycastHit.collider.transform.localPosition.x > 0);
                }
                else if (System.Math.Abs(raycastHit.collider.transform.localPosition.y) > EPSILON)
                {
                    Move(1, raycastHit.collider.transform.localPosition.y > 0);
                }
                else if (System.Math.Abs(raycastHit.collider.transform.localPosition.z) > EPSILON)
                {
                    Move(2, raycastHit.collider.transform.localPosition.z > 0);
                }
            }
        }

        if (inGame && Input.touchCount == 1)
        {
            float rotateSpeed = 0.09f;
            Touch touchZero = Input.GetTouch(0);

            //Rotate the model based on offset
            Vector3 localAngle = GameObject.Find("Cubes").transform.localEulerAngles;
            localAngle.y -= rotateSpeed * touchZero.deltaPosition.x;
            GameObject.Find("Cubes").transform.localEulerAngles = localAngle;
            GameObject.Find("Controller").transform.localEulerAngles = localAngle;
        }

        for (int i = 0; i < 3; ++i)
        {
            for (int j = 0; j < 3; ++j)
            {
                for (int k = 0; k < 3; ++k)
                {
                    if (value[i, j, k] != 0)
                    {
                        if (needUpdate)
                        {
                            if (numList[i * 9 + j * 3 + k] != null)
                            {
                                Destroy(numList[i * 9 + j * 3 + k]);
                                numList[i * 9 + j * 3 + k] = null;
                            }
                            GameObject cube = cubeList[i * 9 + j * 3 + k];
                            GameObject text = Object.Instantiate(textPrefab);
                            text.transform.parent = cube.transform;
                            text.transform.position = cube.transform.position;
                            text.GetComponent<TextMesh>().text = value[i, j, k].ToString();
                            numList[i * 9 + j * 3 + k] = text;
                            materialList[i * 9 + j * 3 + k].color = (Color)colors[(int)Mathf.Log(value[i, j, k], 2)];
                            if(animatorStateList[9 * i + 3 * j + k] == 1)
                            {
                                animatorList[9 * i + 3 * j + k].Play("bounce");
                                animatorStateList[9 * i + 3 * j + k] = 0;
                            }
                            else
                            {
                                animatorList[9 * i + 3 * j + k].Play("shrink");
                            }
                            if (!CheckAlive())
                            {
                                Endgame();
                            }
                        }

                        numList[i * 9 + j * 3 + k].GetComponent<TextMesh>().transform.LookAt(
                            numList[i * 9 + j * 3 + k].GetComponent<TextMesh>().transform.position - Camera.main.transform.position
                        );
                    }
                    else
                    {
                        if (numList[i * 9 + j * 3 + k] != null)
                        {
                            Destroy(numList[i * 9 + j * 3 + k]);
                            numList[i * 9 + j * 3 + k] = null;
                        }
                        if (!cubeList[i * 9 + j * 3 + k].GetComponent<Renderer>().material.color.Equals(colors[0]))
                        {
                            cubeList[i * 9 + j * 3 + k].GetComponent<Renderer>().material.color = colors[0];
                        }
                    }
                }
            }
        }
        needUpdate = false;
    }

    bool CheckAlive()
    {
        bool canMove = false;
        for(int i = 0; i < 6 && !canMove; ++i)
        {
            canMove = Move(i/2, i%2==0, false);
        }
        return canMove;
    }

    bool Move(int dir, bool pos, bool performAction = true)
    {
        Debug.Log("move: "+dir + ", " + (pos ? "POS" : "NEG") + ", " + (performAction ? "action" : "no-action"));
        bool moved = false;
        switch (dir)
        {
            case 0:
                if(pos)
                {
                    for(int i = 2; i > 0; --i)
                    {
                        for (int j = 0; j <= 2; ++j)
                        {
                            for (int k = 0; k <= 2; ++k)
                            {
                                if(value[i, j, k] == 0)
                                {
                                    for (int finder = i - 1; finder >= 0; --finder)
                                    {
                                        if (value[finder, j, k] != 0)
                                        {
                                            if (performAction)
                                            {
                                                value[i, j, k] = value[finder, j, k];
                                                value[finder, j, k] = 0;
                                            }
                                            moved = true;
                                            break;
                                        }
                                    }
                                }

                                if (value[i, j, k] != 0)
                                {
                                    for (int finder = i - 1; finder >= 0; --finder)
                                    {
                                        if (value[finder, j, k] == value[i, j, k])
                                        {
                                            if (performAction)
                                            {
                                                value[i, j, k] *= 2;
                                                value[finder, j, k] = 0;
                                                animatorStateList[9 * i + 3 * j + k] = 1;
                                            }
                                            moved = true;
                                            break;
                                        }
                                        if (value[finder, j, k] != 0)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 2; ++i)
                    {
                        for (int j = 0; j <= 2; ++j)
                        {
                            for (int k = 0; k <= 2; ++k)
                            {
                                if (value[i, j, k] == 0)
                                {
                                    for(int finder = i + 1; finder <= 2; ++finder)
                                    {
                                        if(value[finder, j, k] != 0)
                                        {
                                            if (performAction)
                                            {
                                                value[i, j, k] = value[finder, j, k];
                                                value[finder, j, k] = 0;
                                            }
                                            moved = true;
                                            break;
                                        }
                                    }
                                }

                                if (value[i, j, k] != 0)
                                {
                                    for (int finder = i + 1; finder <= 2; ++finder)
                                    {
                                        if (value[finder, j, k] == value[i, j, k])
                                        {
                                            if (performAction)
                                            {
                                                value[i, j, k] *= 2;
                                                value[finder, j, k] = 0;
                                                animatorStateList[9 * i + 3 * j + k] = 1;
                                            }
                                            moved = true;
                                            break;
                                        }
                                        if (value[finder, j, k] != 0)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                break;
            case 1:
                if (pos)
                {
                    for (int j = 2; j > 0; --j)
                    {
                        for (int i = 0; i <= 2; ++i)
                        {
                            for (int k = 0; k <= 2; ++k)
                            {
                                if (value[i, j, k] == 0)
                                {
                                    for (int finder = j - 1; finder >= 0; --finder)
                                    {
                                        if (value[i, finder, k] != 0)
                                        {
                                            if (performAction)
                                            {
                                                value[i, j, k] = value[i, finder, k];
                                                value[i, finder, k] = 0;
                                            }
                                            moved = true;
                                            break;
                                        }
                                    }
                                }

                                if (value[i, j, k] != 0)
                                {
                                    for (int finder = j - 1; finder >= 0; --finder)
                                    {
                                        if (value[i, j, k] == value[i, finder, k])
                                        {
                                            if (performAction)
                                            {
                                                value[i, j, k] *= 2;
                                                value[i, finder, k] = 0;
                                                animatorStateList[9 * i + 3 * j + k] = 1;
                                            }
                                            moved = true;
                                            break;
                                        }
                                        if (value[i, finder, k] != 0)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < 2; ++j)
                    {
                        for (int i = 0; i <= 2; ++i)
                        {
                            for (int k = 0; k <= 2; ++k)
                            {
                                if (value[i, j, k] == 0)
                                {
                                    for (int finder = j + 1; finder <= 2; ++finder)
                                    {
                                        if (value[i, finder, k] != 0)
                                        {
                                            if (performAction)
                                            {
                                                value[i, j, k] = value[i, finder, k];
                                                value[i, finder, k] = 0;
                                            }
                                            moved = true;
                                            break;
                                        }
                                    }
                                }

                                if (value[i, j, k] != 0)
                                {
                                    for (int finder = j + 1; finder <= 2; ++finder)
                                    {
                                        if (value[i, j, k] == value[i, finder, k])
                                        {
                                            if (performAction)
                                            {
                                                value[i, j, k] *= 2;
                                                value[i, finder, k] = 0;
                                                animatorStateList[9 * i + 3 * j + k] = 1; ;
                                            }
                                            moved = true;
                                            break;
                                        }
                                        if (value[i, finder, k] != 0)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                break;
            case 2:
                if (pos)
                {
                    for (int k = 2; k > 0; --k)
                    {
                        for (int j = 0; j <= 2; ++j)
                        {
                            for (int i = 0; i <= 2; ++i)
                            {
                                if (value[i, j, k] == 0)
                                {
                                    for (int finder = k - 1; finder >= 0; --finder)
                                    {
                                        if (value[i, j, finder] != 0)
                                        {
                                            if (performAction)
                                            {
                                                value[i, j, k] = value[i, j, finder];
                                                value[i, j, finder] = 0;
                                            }
                                            moved = true;
                                            break;
                                        }
                                    }
                                }

                                if (value[i, j, k] != 0)
                                {
                                    for (int finder = k - 1; finder >= 0; --finder)
                                    {
                                        if (value[i, j, finder] == value[i, j, k])
                                        {
                                            if (performAction)
                                            {
                                                value[i, j, k] *= 2;
                                                value[i, j, finder] = 0;
                                                animatorStateList[9 * i + 3 * j + k] = 1;
                                            }
                                            moved = true;
                                            break;
                                        }
                                        if (value[i, j, finder] != 0)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int k = 0; k < 2; ++k)
                    {
                        for (int j = 0; j <= 2; ++j)
                        {
                            for (int i = 0; i <= 2; ++i)
                            {
                                if (value[i, j, k] == 0)
                                {
                                    for (int finder = k + 1; finder <= 2; ++finder)
                                    {
                                        if (value[i, j, finder] != 0)
                                        {
                                            if (performAction)
                                            {
                                                value[i, j, k] = value[i, j, finder];
                                                value[i, j, finder] = 0;
                                            }
                                            moved = true;
                                            break;
                                        }
                                    }
                                }

                                if (value[i, j, k] != 0)
                                {
                                    for (int finder = k + 1; finder <= 2; ++finder)
                                    {
                                        if (value[i, j, finder] == value[i, j, k])
                                        {
                                            if (performAction)
                                            {
                                                value[i, j, k] *= 2;
                                                value[i, j, finder] = 0;
                                                animatorStateList[9 * i + 3 * j + k] = 1;
                                            }
                                            moved = true;
                                            break;
                                        }
                                        if (value[i, j, finder] != 0)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                break;
        }

        if (moved && performAction)
        {
            RandomAdd();
            needUpdate = true;
        }
        return moved;
    }

    void RandomAdd()
    {
        while (true)
        {
            int x = Mathf.FloorToInt((Random.value * 3f) % 3), y = Mathf.FloorToInt((Random.value * 3f) % 3), z = Mathf.FloorToInt((Random.value * 3f) % 3);
            if (value[x, y, z] == 0)
            {
                value[x, y, z] = 2;
                break;
            }
        }
    }

    void Endgame()
    {
        inGame = false;
        endGameUICanvas.SetActive(true);
    }

    public void Restart()
    {
        value = new int[3, 3, 3]  {
                                    { { 0,0,0 }, { 0,0,0 } , { 0,0,0 } },
                                    { { 0,0,0 }, { 0,0,0 } , { 0,0,0 } },
                                    { { 0,0,0 }, { 0,0,0 } , { 0,0,0 } }
                                  };
        animatorStateList = new int[27];
        RandomAdd();
        needUpdate = true;
        inGame = true;
        endGameUICanvas.SetActive(false);
    }
}
