using UnityEngine;

public class game_controller : MonoBehaviour
{
    const float EPSILON = 0.1f;
    int[,,] value;
    bool needUpdate = false;
    GameObject textPrefab;
    GameObject[] numList;
    GameObject[] cubeList;
    Material[] materialList;
    Color[] colors = {
        new Color32(192, 180, 164, 128),
        new Color32(233, 221, 209, 128),
        new Color32(232, 217, 189, 128),
        new Color32(238, 162, 102, 128),
        new Color32(240, 130, 80, 128),
        new Color32(241, 101, 77, 128),
        new Color32(241, 71, 45, 128),
        new Color32(232, 198, 95, 128),
        new Color32(232, 195, 80, 128)
    };
    void Start()
    {
        value = new int[3, 3, 3]  {
                                    { { 0,0,0 }, { 0,0,0 } , { 0,0,0 } },
                                    { { 0,0,0 }, { 0,0,0 } , { 0,0,0 } },
                                    { { 0,0,0 }, { 0,0,0 } , { 0,0,0 } }
                                  };

        randomAdd();
        needUpdate = true;

        textPrefab = GameObject.Find("Text Prefab");//Object.Instantiate(textPrefab).GetComponent<Projectile>();
        cubeList = new GameObject[27];
        materialList = new Material[27];
        for (int i = 0; i < 3; ++i)
        {
            for (int j = 0; j < 3; ++j)
            {
                for (int k = 0; k < 3; ++k)
                {
                    cubeList[9 * i + 3 * j + k] = GameObject.Find("Cube " + i + "_" + j + "_" + k);
                    materialList[9 * i + 3 * j + k] = cubeList[9 * i + 3 * j + k].GetComponent<Renderer>().material;
                }
            }
        }

        numList = new GameObject[27];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                if(System.Math.Abs(raycastHit.collider.transform.localPosition.x) > EPSILON)
                {
                    move(0, raycastHit.collider.transform.localPosition.x > 0);
                }
                else if (System.Math.Abs(raycastHit.collider.transform.localPosition.y) > EPSILON)
                {
                    move(1, raycastHit.collider.transform.localPosition.y > 0);
                }
                else if (System.Math.Abs(raycastHit.collider.transform.localPosition.z) > EPSILON)
                {
                    move(2, raycastHit.collider.transform.localPosition.z > 0);
                }
            }
        }

        if (Input.touchCount == 1)
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

    void move(int dir, bool pos)
    {
        Debug.Log("move: "+dir+", "+(pos?"POS":"NEG"));
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
                                            value[i, j, k] = value[finder, j, k];
                                            value[finder, j, k] = 0;
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
                                            value[i, j, k] *= 2;
                                            value[finder, j, k] = 0;
                                            moved = true;
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
                                            moved = true;
                                            value[i, j, k] = value[finder, j, k];
                                            value[finder, j, k] = 0;
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
                                            moved = true;
                                            value[i, j, k] *= 2;
                                            value[finder, j, k] = 0;
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
                                            moved = true;
                                            value[i, j, k] = value[i, finder, k];
                                            value[i, finder, k] = 0;
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
                                            moved = true;
                                            value[i, j, k] *= 2;
                                            value[i, finder, k] = 0;
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
                                            moved = true;
                                            value[i, j, k] = value[i, finder, k];
                                            value[i, finder, k] = 0;
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
                                            moved = true;
                                            value[i, j, k] *= 2;
                                            value[i, finder, k] = 0;
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
                                            moved = true;
                                            value[i, j, k] = value[i, j, finder];
                                            value[i, j, finder] = 0;
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
                                            moved = true;
                                            value[i, j, k] *= 2;
                                            value[i, j, finder] = 0;
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
                                            moved = true;
                                            value[i, j, k] = value[i, j, finder];
                                            value[i, j, finder] = 0;
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
                                            moved = true;
                                            value[i, j, k] *= 2;
                                            value[i, j, finder] = 0;
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

        if (moved)
        {
            randomAdd();
            needUpdate = true;
        }
    }

    void randomAdd()
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
}
