using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject plane;
    public GameObject directionLight;
    public GameObject mainCamera;
    public GameObject player1;
    public GameObject player2;
    public GameObject bomb1Prefab;
    public GameObject bomb2Prefab;
    public GameObject splash1Prefab;
    public GameObject splash2Prefab;
    public GameObject wallPrefab;
    public GameObject blockPrefab;
    public GameObject item11Prefab;
    public GameObject item12Prefab;
    public GameObject item13Prefab;
    public GameObject item14Prefab;
    public GameObject[] itemPrefab;

    private Player[] player;
    private CMap mapData;

    private bool[][] destroyCheck;
    private Vector3[] beforeMove;

    // Start is called before the first frame update
    void Start()
    {
        int i;

        // Player

        player = new Player[3];
        player[1] = player1.GetComponent<Player>();
        player[2] = player2.GetComponent<Player>();

        for (i = 1; i <= 2; i++)
        {
            string player_name = PlayerPrefs.GetString("Player " + i);

            if (player_name == null) player_name = "Basic";

            if (player_name == "Speedy") player[i].setCharacter(1);
            else if (player_name == "Heavy") player[i].setCharacter(2);
            else if (player_name == "Long") player[i].setCharacter(3);
            else player[i].setCharacter(0); //Basic
        }

        // Map

        string map_name = PlayerPrefs.GetString("Map");

        mapData = new CMap();
        if (mapData.openMap(map_name) == false)
        {
            UnityEngine.Application.Quit();
            return;
        }

        // Key
        player[1].upKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up1", KeyCode.W.ToString()));
        player[1].downKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down1", KeyCode.S.ToString()));
        player[1].leftKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left1", KeyCode.A.ToString()));
        player[1].rightKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right1", KeyCode.D.ToString()));
        player[1].bombKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Bomb1", KeyCode.LeftControl.ToString()));

        player[2].upKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up2", KeyCode.UpArrow.ToString()));
        player[2].downKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down2", KeyCode.DownArrow.ToString()));
        player[2].leftKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left2", KeyCode.LeftArrow.ToString()));
        player[2].rightKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right2", KeyCode.RightArrow.ToString()));
        player[2].bombKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Bomb2", KeyCode.Period.ToString()));

        // etc

        destroyCheck = new bool[mapData.size.x + 1][];
        for (i = 1; i <= mapData.size.x; i++) destroyCheck[i] = new bool[mapData.size.z + 1];
        beforeMove = new Vector3[3];
        beforeMove[1] = beforeMove[2] = Vector3.zero;

        itemPrefab = new GameObject[4 + 1];
        itemPrefab[1] = item11Prefab;
        itemPrefab[2] = item12Prefab;
        itemPrefab[3] = item13Prefab;
        itemPrefab[4] = item14Prefab;

        grantItem();
        drawMap();
        setStarting();
        setCamera();

    }

    // Update is called once per frame
    void Update()
    {
        int i;
        Vector3Int afterLocation;

        processKeyboard();

        for (i = 1; i <= 2; i++)
        {
            checkLastBomb(i);

            afterLocation = locationPlayer(i);

            if (player[i].location != afterLocation)
            {
                player[i].location = afterLocation;
            }
        }
    }

    // brokable block에 아이템 부여
    public void grantItem()
    {
        int i, j, r, lblocks = 0, sum = 0;
        int[] cnt = new int[5];
        float[] percentage = new float[5] { 0, 0.15f, 0.15f, 0.15f, 0.05f};
        Vector2Int[] blocks = new Vector2Int[mapData.size.x * mapData.size.z + 1];

        for (i = 1; i <= mapData.size.x; i++) for (j = 1; j <= mapData.size.z; j++) if ((mapData.map[i][j].type & (CMap.B_BLOCK | CMap.B_BROCKABLE)) == (CMap.B_BLOCK | CMap.B_BROCKABLE)) blocks[++lblocks] = new Vector2Int(i, j);

        for (i = 1; i <= 4; i++)
        {
            cnt[i] = (int)(percentage[i] * lblocks);
            sum += cnt[i];
        }
        cnt[0] = lblocks - sum;

        for (i = 1; i <= lblocks; i++)
        {
            while (true)
            {
                r = UnityEngine.Random.Range(0, 5);
                if (cnt[r] != 0)
                {
                    cnt[r]--;
                    break;
                }
            }

            if (r == 0) mapData.map[blocks[i].x][blocks[i].y].state = 0;
            else mapData.map[blocks[i].x][blocks[i].y].state = 10 + r;
        }
    }

    // 맵 그리기
    public void drawMap()
    {
        int i, j;
        Vector3[] position = new Vector3[4];
        Vector3[] scale = new Vector3[4];
        Vector3 rotation = new Vector3(0, 0, 0);

        // 벽 그리기
        position[0] = new Vector3((mapData.size.x + 1) / 2f, 1f, 0f);
        position[1] = new Vector3((mapData.size.x + 1) / 2f, 1f, mapData.size.z + 1);
        position[2] = new Vector3(0f, 1f, (mapData.size.z + 1) / 2f);
        position[3] = new Vector3(mapData.size.x + 1, 1f, (mapData.size.z + 1) / 2f);

        scale[0] = new Vector3(mapData.size.x + 1, 2f, 0.2f);
        scale[1] = new Vector3(mapData.size.x + 1, 2f, 0.2f);
        scale[2] = new Vector3(0.2f, 2f, mapData.size.z + 1);
        scale[3] = new Vector3(0.2f, 2f, mapData.size.z + 1);

        plane.transform.position = new Vector3((mapData.size.x + 1) / 2f, 0, (mapData.size.z + 1) / 2f);
        plane.transform.localScale = new Vector3((mapData.size.x + 1) / 10f, 1, (mapData.size.z + 1) / 10f);
        directionLight.transform.position = plane.transform.position;

        for (i = 0; i < 4; i++)
        {
            GameObject wall = Instantiate(wallPrefab, position[i], Quaternion.Euler(rotation));
            wall.transform.localScale = scale[i];
        }

        // 맵 그리기
        scale[0] = new Vector3(0.8f, 1.6f, 0.8f);
        for (i = 1; i <= mapData.size.x; i++)
        {
            for (j = 1; j <= mapData.size.z; j++)
            {
                if ((mapData.map[i][j].type & CMap.B_BLOCK) == 1) // 빈 공간이 아닐 경우
                {
                    position[0] = new Vector3(i, scale[0].y / 2, j);
                    GameObject block = Instantiate(blockPrefab, position[0], Quaternion.Euler(rotation));
                    block.transform.localScale = scale[0];

                    Renderer blockRenderer = block.GetComponent<Renderer>();
                    if (blockRenderer != null)
                    {
                        blockRenderer.material.color = mapData.map[i][j].color;
                    }

                    Block blockComponent = block.GetComponent<Block>();
                    mapData.map[i][j].block = blockComponent;
                }
            }
        }
    }

    // 스타팅으로
    public void setStarting()
    {
        int len = mapData.starting.Length, r1 = 0, r2 = 0;

        // 모두 생성
        r1 = UnityEngine.Random.Range(0, len);

        do r2 = UnityEngine.Random.Range(0, len);
        while (r1 == r2);

        player1.transform.position = mapData.starting[r1];
        player2.transform.position = mapData.starting[r2];

        player[1].location = mapData.starting[r1];
        player[2].location = mapData.starting[r2];
    }

    // 카메라 조절
    public void setCamera()
    {
        Vector3 pos = new Vector3(mapData.size.x / 2, 25f, mapData.size.z / 2);

        mainCamera.transform.position = pos;
    }

    // 키 입력 처리
    public void processKeyboard()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("OpeningScene");
            return;
        }

        int i;
        for (i = 1; i <= 2; i++)
        {
            if (player[i].state != 1) continue; // 살아있는게 아니면

            Vector3 move = processKeyboardMove(i);
            processKeyboardBomb(i);

            movePlayer(i, move);
        }
    }

    // 키 이동 입력 처리
    public Vector3 processKeyboardMove(int number)
    {
        Vector3 move = Vector3.zero;
        if (Input.GetKey(player[number].upKey)) // 위쪽
        {
            if (beforeMove[number] == Vector3.zero || beforeMove[number] == Vector3.forward || beforeMove[number] == Vector3.back)
            {
                move += Vector3.forward;
            }
        }
        if (Input.GetKey(player[number].downKey)) // 아래쪽
        {
            if (beforeMove[number] == Vector3.zero || beforeMove[number] == Vector3.forward || beforeMove[number] == Vector3.back)
            {
                move += Vector3.back;
            }
        }
        if (Input.GetKey(player[number].leftKey)) // 왼쪽
        {
            if (beforeMove[number] == Vector3.zero || beforeMove[number] == Vector3.left || beforeMove[number] == Vector3.right)
            {
                move += Vector3.left;
            }
        }
        if (Input.GetKey(player[number].rightKey)) // 오른쪽
        {
            if (beforeMove[number] == Vector3.zero || beforeMove[number] == Vector3.left || beforeMove[number] == Vector3.right)
            {
                move += Vector3.right;
            }
        }
        return move;
    }

    // 키 폭탄 입력 처리
    public void processKeyboardBomb(int number)
    {
        if (Input.GetKeyDown(player[number].bombKey))
        {
            int j;
            GameObject bomb = placeBomb(number);

            if (bomb != null)
            {
                for (j = 1; j <= 2; j++)
                {
                    if (player[j].location == player[number].location) // 폭탄 위치에 있는 플레이어
                    {
                        player[j].lastBomb = bomb;
                        player[j].lastBombLocation = player[j].location;
                        Collider playerCollider = (j == 1) ? player1.GetComponent<Collider>() : player2.GetComponent<Collider>();
                        Collider bombCollider = bomb.GetComponent<Collider>();
                        Physics.IgnoreCollision(playerCollider, bombCollider, true);
                    }
                }
            }
        }
    }

    // 플레이어 이동
    public void movePlayer(int number, Vector3 move)
    {
        Vector3 playerPosition = (number == 1) ? player1.transform.position : player2.transform.position;
        Rigidbody rb = (number == 1) ? player1.GetComponent<Rigidbody>() : player2.GetComponent<Rigidbody>();
        Vector3 lr = playerPosition - player[number].location;

        bool moveCheck = adjustPlayerPositionIngap(number, ref playerPosition, move, lr);
        beforeMove[number] = move;

        if (move != Vector3.zero)
        {
            Vector3 newPosition = new Vector3(player[number].location.x, 0.1f, player[number].location.z);
            rb.MoveRotation(Quaternion.LookRotation(move));

            // 앞에 movable 블록 or 폭탄이 있는지 확인
            checkObstacle(number, newPosition, move);
        }

        if (moveCheck) rb.velocity = move * player[number].speed;
        if (number == 1) player1.transform.position = playerPosition;
        else player2.transform.position = playerPosition;
    }

    // 틈새 이동 시 플레이어의 위치 보정
    public bool adjustPlayerPositionIngap(int number, ref Vector3 playerPosition, Vector3 move, Vector3 lr)
    {
        if (move == Vector3.forward)
        {
            if (playerPosition.z >= mapData.size.z)
            {
                playerPosition.z = mapData.size.z;
                return false;
            }
            else if ((mapData.map[player[number].location.x][player[number].location.z + 1].type & CMap.B_BLOCK) == CMap.B_BLOCK) // 바로 오른쪽 벽
            {
                if (lr.z > 0)
                {
                    playerPosition.z = player[number].location.z;
                    return false;
                }
            }
            else if (-0.5f < lr.x && lr.x < 0f && player[number].location.x - 1 >= 1 && (mapData.map[player[number].location.x - 1][player[number].location.z + 1].type & CMap.B_BLOCK) == CMap.B_BLOCK) // 바로 오른쪽 벽x, 그위 벽
            {
                playerPosition.x = player[number].location.x;
            }
            else if (0f < lr.x && lr.x < 0.5f && player[number].location.x + 1 <= mapData.size.x && (mapData.map[player[number].location.x + 1][player[number].location.z + 1].type & CMap.B_BLOCK) == CMap.B_BLOCK) // 바로 오른쪽 벽x, 그아래 벽
            {
                playerPosition.x = player[number].location.x;
            }
        }
        else if (move == Vector3.back)
        {
            if (playerPosition.z < 1)
            {
                playerPosition.z = 1f;
                return false;
            }
            else if ((mapData.map[player[number].location.x][player[number].location.z - 1].type & CMap.B_BLOCK) == CMap.B_BLOCK) // 바로 왼쪽 벽
            {
                if (lr.z < 0)
                {
                    playerPosition.z = player[number].location.z;
                    return false;
                }
            }
            else if (-0.5f < lr.x && lr.x < 0f && player[number].location.x - 1 >= 1 && (mapData.map[player[number].location.x - 1][player[number].location.z - 1].type & CMap.B_BLOCK) == CMap.B_BLOCK) // 바로 왼쪽 벽x, 그위 벽
            {
                playerPosition.x = player[number].location.x;
            }
            else if (0f < lr.x && lr.x < 0.5f && player[number].location.x + 1 <= mapData.size.x && (mapData.map[player[number].location.x + 1][player[number].location.z - 1].type & CMap.B_BLOCK) == CMap.B_BLOCK) // 바로 왼쪽 벽x, 그아래 벽
            {
                playerPosition.x = player[number].location.x;
            }
        }
        else if (move == Vector3.left)
        {
            if (playerPosition.x < 1)
            {
                playerPosition.x = 1f;
                return false;
            }
            else if ((mapData.map[player[number].location.x - 1][player[number].location.z].type & CMap.B_BLOCK) == CMap.B_BLOCK) // 바로위 벽
            {
                if (lr.x < 0)
                {
                    playerPosition.x = player[number].location.x;
                    return false;
                }
            }
            else if (-0.5f < lr.z && lr.z < 0f && 1 <= player[number].location.z - 1 && (mapData.map[player[number].location.x - 1][player[number].location.z - 1].type & CMap.B_BLOCK) == CMap.B_BLOCK) // 바로위 벽x, 그왼쪽 벽
            {
                playerPosition.z = player[number].location.z;
            }
            else if (0f < lr.z && lr.z < 0.5f && player[number].location.z + 1 <= mapData.size.z && (mapData.map[player[number].location.x - 1][player[number].location.z + 1].type & CMap.B_BLOCK) == CMap.B_BLOCK) // 바로위 벽x, 그오른쪽 벽
            {
                playerPosition.z = player[number].location.z;
            }
        }
        else if (move == Vector3.right)
        {
            if (playerPosition.x >= mapData.size.x)
            {
                playerPosition.x = mapData.size.x;
                return false;
            }
            else if ((mapData.map[player[number].location.x + 1][player[number].location.z].type & CMap.B_BLOCK) == CMap.B_BLOCK) // 바로 아래 벽
            {
                if (lr.x > 0)
                {
                    playerPosition.x = player[number].location.x;
                    return false;
                }
            }
            else if (-0.5f < lr.z && lr.z < 0f && player[number].location.z - 1 >= 1 && (mapData.map[player[number].location.x + 1][player[number].location.z - 1].type & CMap.B_BLOCK) == CMap.B_BLOCK) // 바로 아래 벽x, 그왼쪽 벽
            {
                playerPosition.z = player[number].location.z;
            }
            else if (0f < lr.z && lr.z < 0.5f && player[number].location.z + 1 <= mapData.size.z && (mapData.map[player[number].location.x + 1][player[number].location.z + 1].type & CMap.B_BLOCK) == CMap.B_BLOCK) // 바로 아래 벽x, 그오른쪽 벽
            {
                playerPosition.z = player[number].location.z;
            }
        }
        return true;
    }

    // 벽과 폭탄 체크
    public void checkObstacle(int number, Vector3 newPosition, Vector3 move)
    {
        Ray ray = new Ray(newPosition, move);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1.0f))
        {
            if (hit.collider.CompareTag("Block")) // movable 블록
            {
                Vector3Int moveInt = new Vector3Int((int)move.x, (int)move.y, (int)move.z);
                Vector3Int nextPos = player[number].location + moveInt;
                Vector3Int nextnextPos = nextPos + moveInt;

                if (1 <= nextnextPos.x && nextnextPos.x <= mapData.size.x && 1 <= nextnextPos.z && nextnextPos.z <= mapData.size.z // boundary
                    && (mapData.map[nextPos.x][nextPos.z].type & CMap.B_MOVABLE) == CMap.B_MOVABLE // 움직일 수 있는 블록이고
                    && (mapData.map[nextnextPos.x][nextnextPos.z].type & CMap.B_BLOCK) == 0 // 뒤에 블록이 없고
                    && mapData.map[nextnextPos.x][nextnextPos.z].bomb == null) // 폭탄도 없으면
                {
                    if (player[number].pushTimer == 0f)
                    {
                        player[number].pushTimer = Time.time;
                        player[number].pushDirection = move;
                    }
                    else
                    {
                        if (player[number].pushDirection != move) // 도중에 방향 바꾸면 초기화
                        {
                            player[number].pushTimer = 0f;
                            player[number].pushDirection = Vector3.zero;
                        }
                        else if (Time.time - player[number].pushTimer >= 0.3f) // 벽을 0.3초 이상 밀었다면
                        {
                            mapData.map[nextPos.x][nextPos.z].block.moveBlock(move);
                            moveBlock(nextPos, nextnextPos);
                            player[number].pushTimer = 0f;
                            player[number].pushDirection = Vector3.zero;
                        }
                    }
                }
                else
                {
                    player[number].pushTimer = 0f;
                    player[number].pushDirection = Vector3.zero;
                }
            }
            else if (player[number].ableKick && hit.collider.CompareTag("Bomb"))
            {
                Vector3Int moveInt = new Vector3Int((int)move.x, (int)move.y, (int)move.z);
                Vector3Int nextPos = player[number].location + moveInt;
                Vector3Int nextnextPos = nextPos + moveInt;

                if (1 <= nextnextPos.x && nextnextPos.x <= mapData.size.x && 1 <= nextnextPos.z && nextnextPos.z <= mapData.size.z // boundary
                    && (mapData.map[nextnextPos.x][nextnextPos.z].type & CMap.B_BLOCK) == 0 // 뒤에 블록이 없고
                    && mapData.map[nextnextPos.x][nextnextPos.z].bomb == null) // 폭탄도 없으면
                {
                    if (player[number].pushTimer == 0f)
                    {
                        player[number].pushTimer = Time.time;
                        player[number].pushDirection = move;
                    }
                    else
                    {
                        if (player[number].pushDirection != move) // 도중에 방향 바꾸면 초기화
                        {
                            player[number].pushTimer = 0f;
                            player[number].pushDirection = Vector3.zero;
                        }
                        else if (Time.time - player[number].pushTimer >= 0.3f) // 폭탄을 0.3초 이상 밀었다면
                        {
                            Vector3Int destination = kickBomb(nextPos, move);
                            mapData.map[destination.x][destination.z].bomb.kickBomb(destination);
                            player[number].pushTimer = 0f;
                            player[number].pushDirection = Vector3.zero;
                        }
                    }
                }
                else
                {
                    player[number].pushTimer = 0f;
                    player[number].pushDirection = Vector3.zero;
                }
            }
            else
            {
                player[number].pushTimer = 0f;
                player[number].pushDirection = Vector3.zero;
            }
        }
        else
        {
            player[number].pushTimer = 0f;
            player[number].pushDirection = Vector3.zero;
        }
    }

    // 플레이어의 int 위치
    public Vector3Int locationPlayer(int number)
    {
        Vector3 position;
        Vector3Int result = new Vector3Int();

        position = player[number].transform.position;

        result.x = Mathf.RoundToInt(position.x);
        result.y = Mathf.RoundToInt(position.y);
        result.z = Mathf.RoundToInt(position.z);

        result.x = Mathf.Clamp(result.x, 1, mapData.size.x);
        result.z = Mathf.Clamp(result.z, 1, mapData.size.z);

        return result;
    }

    // lastBomb 폭탄 확인
    public void checkLastBomb(int number)
    {
        if (player[number].lastBomb == null) return;

        Vector3 location = new Vector3();
        location = player[number].transform.position;
        location -= player[number].lastBombLocation;

        if(Mathf.Abs(location.x) >= 1.0f || Mathf.Abs(location.z) >= 1.0f)
        {
            Collider playerCollider = (number == 1) ? player1.GetComponent<Collider>() : player2.GetComponent<Collider>();
            Collider bombCollider = player[number].lastBomb.GetComponent<Collider>();
            Physics.IgnoreCollision(playerCollider, bombCollider, false);
            player[number].lastBomb = null;
        }
    }

    // 플레이어 사망
    public void diePlayer(int number)
    {
        player[number].state = 0;
        GameObject playerObject;

        if (number == 1) playerObject = player1;
        else playerObject = player2;

        Renderer[] renderers = playerObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }
    }

    // 폭탄을 놓음
    public GameObject placeBomb(int number)
    {
        GameObject bomb = null;
        Vector3Int position;
        Vector3 bombPosition = new Vector3();
        Vector3 rotation = new Vector3(0, 0, 0);

        // 폭탄 여유 갯수 확인
        if (player[number].nowcount >= player[number].count) return bomb;

        position = locationPlayer(number);

        // 폭탄을 놓을 수 있는 자리인지 확인
        if ((mapData.map[position.x][position.z].type & CMap.B_BLOCK) == 1 || mapData.map[position.x][position.z].state == 1) return bomb;

        // 폭탄 설치
        bombPosition = position;
        bombPosition.y = 0.5f;
        if (number == 1) bomb = Instantiate(bomb1Prefab, bombPosition, Quaternion.Euler(rotation));
        else bomb = Instantiate(bomb2Prefab, bombPosition, Quaternion.Euler(rotation));

        // Bomb 내부 변수 설정
        Bomb bombComponent = bomb.GetComponent<Bomb>();
        if (bombComponent != null)
        {
            if (number == 1) bombComponent.splashPrefab = splash1Prefab;
            else bombComponent.splashPrefab = splash2Prefab;
            bombComponent.position = position;
            bombComponent.player = number;
            bombComponent.length = player[number].length;
        }

        // 나머지 설정
        player[number].nowcount++;
        mapData.map[position.x][position.z].state = 1;
        mapData.map[position.x][position.z].bomb = bombComponent;

        return bomb;
    }

    // 폭탄이 터짐
    public void splashBomb(int number, Vector3Int position, int length, GameObject splashPrefab)
    {
        int i;
        bool result;
        Vector3Int[] direction = new Vector3Int[] { new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, 0, 1), new Vector3Int(0, 0, -1) };

        // 폭탄 사라짐 나머지 설정
        player[number].nowcount--;
        if ((mapData.map[position.x][position.z].type & CMap.B_BLOCK) == 0) // 원래 빈 공간이면
        {
            mapData.map[position.x][position.z].state = 0;
        }

        // splash 생성
        placeSplash(position, splashPrefab);

        foreach(Vector3Int dir in direction)
        {
            for (i = 1; i <= length; i++)
            {
                result = placeSplash(position + dir * i, splashPrefab);

                if (result == false) break;
            }
        }
    }

    // 스플래시 체크 & 놓음
    public bool placeSplash(Vector3Int position, GameObject splashPrefab)
    {
        int i;
        bool result = false;
        Vector3 pos = new Vector3();
        Vector3 rotation = new Vector3(0, 0, 0);

        if (position.x < 1 || position.x > mapData.size.x || position.z < 1 || position.z > mapData.size.z) return result; // boundary
        else if ((mapData.map[position.x][position.z].type & CMap.B_BLOCK) == 0) // 빈 공간
        {
            switch (mapData.map[position.x][position.z].state)
            {
            case 0: // 없음
                result = true;
                break;

            case 1: // 폭탄
                mapData.map[position.x][position.z].bomb.Explode();
                result = true;
                break;

            default: // 아이템
                mapData.map[position.x][position.z].state = 0;
                mapData.map[position.x][position.z].item.Explode();
                result = true;
                break;
            }
        }
        else if ((mapData.map[position.x][position.z].type & CMap.B_BROCKABLE) == 0) // unbrokable block
        {
            return result;
        }
        else // brokable block
        {
            destroyCheck[position.x][position.z] = true;
            return result;
        }

        pos = position;
        Instantiate(splashPrefab, pos, Quaternion.Euler(rotation));

        // 플레이어 확인
        for (i = 1; i <= 2; i++)
        {
            if (player[i].location == position) diePlayer(i);
        }

        return result;
    }

    public void placeItem(Vector3Int position)
    {
        if (mapData.map[position.x][position.z].state < 11 || mapData.map[position.x][position.z].state > 14) return;

        Vector3 pos = new Vector3(position.x, 0.5f, position.z);
        Vector3 scale = new Vector3(1f, 1f, 1f);
        Vector3 rotation = new Vector3(90, 0, 0);

        GameObject item = Instantiate(itemPrefab[mapData.map[position.x][position.z].state - 10], pos, Quaternion.Euler(rotation));
        item.transform.localScale = scale;

        Item itemComponent = item.GetComponent<Item>();
        itemComponent.item = mapData.map[position.x][position.z].state;
        itemComponent.position = position;

        mapData.map[position.x][position.z].item = itemComponent;
    }

    public void getItem(int number, Vector3Int position, int item)
    {
        switch(item)
        {
        case 11: // 폭탄 갯수
            if (player[number].count < player[number].countBoundary.y) player[number].count++;
            break;

        case 12: // 폭탄 길이
            if (player[number].length < player[number].lengthBoundary.y) player[number].length++;
            break;

        case 13: // 이속
            if (player[number].speed < player[number].speedBoundary.y) player[number].speed += player[number].speedBoundary.z;
            break;

        case 14: // 신발
            player[number].ableKick = true;
            break;

        }

        mapData.map[position.x][position.z].state = 0;
    }

    public void moveBlock(Vector3Int start, Vector3Int end)
    {
        CMapElement tmpBlock;

        tmpBlock = mapData.map[start.x][start.z];
        mapData.map[start.x][start.z] = mapData.map[end.x][end.z];
        mapData.map[end.x][end.z] = tmpBlock;

        mapData.map[start.x][start.z].state = 0;

        if (mapData.map[start.x][start.z].item != null) mapData.map[start.x][start.z].item.Explode();
    }

    public Vector3Int kickBomb(Vector3Int start, Vector3 move)
    {
        Vector3Int end = new Vector3Int();
        Vector3Int mv = new Vector3Int((int)move.x, (int)move.y, (int)move.z);

        end = start;
        while (true)
        {
            end += mv;

            if (1 <= end.x && end.x <= mapData.size.x && 1 <= end.z && end.z <= mapData.size.z // boundary
            && (mapData.map[end.x][end.z].type & CMap.B_BLOCK) == 0 // 블록이 아니고
            && mapData.map[end.x][end.z].bomb == null // 폭탄이 없고
            && (end.x != player[1].location.x || end.z != player[1].location.z) // 1P랑 안겹치고
            && (end.x != player[2].location.x || end.z != player[2].location.z) // 2P랑 안겹치고
            )
            {
                continue;
            }
            else break;
        }
        end -= mv;

        mapData.map[end.x][end.z].bomb = mapData.map[start.x][start.z].bomb;
        mapData.map[start.x][start.z].bomb = null;

        mapData.map[start.x][start.z].state = 0;
        mapData.map[end.x][end.z].state = 1;

        if (mapData.map[end.x][end.z].item != null) mapData.map[end.x][end.z].item.Explode();

        return end;
    }

    public void clearCheck()
    {
        int i, j;

        for (i = 1; i <= mapData.size.x; i++) for (j = 1; j <= mapData.size.z; j++) destroyCheck[i][j] = false;
    }

    public void destroyBlockCheck()
    {
        int i, j;

        for (i = 1; i <= mapData.size.x; i++)
        {
            for (j = 1; j <= mapData.size.z; j++)
            {
                if (destroyCheck[i][j])
                {
                    mapData.map[i][j].type = 0;
                    mapData.map[i][j].block.destroyBlock();

                    if (mapData.map[i][j].state != 0) placeItem(new Vector3Int(i, 0, j));
                }
            }
        }
    }
}
