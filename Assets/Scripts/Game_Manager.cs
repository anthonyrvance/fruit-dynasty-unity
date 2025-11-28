using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Game_Manager : MonoBehaviour
{
    public static Game_Manager instance = null;

    // components
    [SerializeField] GameObject tileBoardLayoutGroup;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject tileRowPrefab;

    List<GameObject> tile_rows = new List<GameObject>(); // to use fruit spot coordinates lets save the rows for x compare

    int tiles_in_row;
    int num_fruits;

    List<Tile> tiles = new List<Tile>();
    List<Vector2> fruit_spots = new List<Vector2>();

    private void OnEnable()
    {
        Signals.OnTilePressed += CheckBoard;
    }

    private void OnDisable()
    {
        Signals.OnTilePressed -= CheckBoard;
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        //Debug.Log(tile_colors.colors[0]);
        //Debug.Log(tile_colors.colors[5]);
    }

    void Start()
    {
        Rand_Seed();
        Init();
    }

    void Update()
    {
        
    }

    void Init()
    {
        // for 10/25 im just going to make something that works so i can create a generated campaign
        tiles_in_row = 7; // TODO: set some other way
        num_fruits = 3;

        // create board by dimensions
        for (int i = 0;i < tiles_in_row; i++)
        {
            GameObject row = Instantiate(tileRowPrefab, tileBoardLayoutGroup.transform);
            tile_rows.Add(row);
            for (int j = 0; j < tiles_in_row; j++)
            {
                GameObject tmp = Instantiate(tilePrefab, row.transform);
                tiles.Add(tmp.GetComponent<Tile>());
            }
        }

        SetTilesPositions();

        // pick fruit king spots
        fruit_spots.Add(Roll_Numbers(0, num_fruits)); // 1st is auto

        int remaining = num_fruits - 1;
        while (remaining > 0) 
        {
            Vector2 test_spot = Roll_Numbers(0, tiles_in_row);
            if (Check_Horizontal(test_spot) && Check_Vertical(test_spot) && Check_Adjacent(test_spot))
            {
                fruit_spots.Add(test_spot);
                remaining--;
            }
        }

        Show_Crowns_Debug();

        // create colored tiles
        for (int i = 0; i < num_fruits; i++)
        {
            Tile temp = tile_rows[(int)fruit_spots[i][0]].transform.GetChild((int)fruit_spots[i][1]).GetComponent<Tile>();
            temp.SetFill(Tile_Properties.colors[i]);
            temp.SetID(i);
            temp.SetOccupied(true); // shouldnt need fruit locations because they are in colors anyway
        }

        for (int i = 0; i < num_fruits; i++) // paint 1 color at a time
        {
            // reset non colored/fruits back to not tested
            for (int x = 0; x < tiles_in_row; x++)
            {
                for (int y = 0; y < tiles_in_row; y++)
                {
                    Tile temp = tile_rows[x].transform.GetChild(y).GetComponent<Tile>();
                    temp.SetTested(false);
                }
            }

            Vector2 currPos = fruit_spots[i];
            int tries = num_fruits;
            while (tries > 0)
            {
                Vector2 dir = Roll_Rand_Dir();

                if (currPos[0] + dir[0] >= 0 && currPos[0] + dir[0] < tiles_in_row &&
                    currPos[1] + dir[1] >= 0 && currPos[1] + dir[1] < tiles_in_row) // if in bounds
                {
                    Vector2 testPos = currPos + dir;
                    Tile temp = tile_rows[(int)testPos[0]].transform.GetChild((int)testPos[1]).GetComponent<Tile>();
                    if (!temp.GetTested() && !temp.GetOccupied())
                    {
                        temp.SetTested(true);
                        temp.SetOccupied(true);
                        temp.SetFill(Tile_Properties.colors[i]);
                        temp.SetID(i);
                        tries = num_fruits;
                        currPos = testPos;
                        //break;
                    }

                    tries--; // TODO it only paints 1 additional atm
                }
            }
        }


        // if random (like for pvp)
        //Rand_Seed();
    }

    void CheckBoard()
    {
        // check which tiles have a guess in them
        List<Tile> guessTiles = new List<Tile>();
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].SetWrongIconState(false); // reset them
            if (tiles[i].GetGuessState())
            {
                guessTiles.Add(tiles[i]);
            }
        }

        // see if guesses break any rules
        bool brokeRules = false;
        for (int i = 0; i < guessTiles.Count; i++)
        {
            for (int j = 0; j < guessTiles.Count; j++)
            {
                if (i == j)
                    continue;

                Vector2 nw = new Vector2(guessTiles[i].GetPos()[0] - 1, guessTiles[i].GetPos()[1] - 1);
                Vector2 ne = new Vector2(guessTiles[i].GetPos()[0] + 1, guessTiles[i].GetPos()[1] - 1);
                Vector2 sw = new Vector2(guessTiles[i].GetPos()[0] - 1, guessTiles[i].GetPos()[1] + 1);
                Vector2 se = new Vector2(guessTiles[i].GetPos()[0] + 1, guessTiles[i].GetPos()[1] + 1);
                if (guessTiles[i].GetID() == guessTiles[j].GetID())
                {
                    guessTiles[i].SetWrongIconState(true);
                    guessTiles[j].SetWrongIconState(true);
                }
                else if (guessTiles[i].GetPos()[0] == guessTiles[j].GetPos()[0]) // hori
                {
                    for (int x = 0; x < tiles_in_row; x++)
                    {
                        tile_rows[(int)guessTiles[i].GetPos()[0]].transform.GetChild(x).GetComponent<Tile>().SetWrongIconState(true);
                    }
                }
                else if (guessTiles[i].GetPos()[1] == guessTiles[j].GetPos()[1]) // vert
                {
                    for (int x = 0; x < tiles_in_row; x++)
                    {
                        tile_rows[x].transform.GetChild((int)guessTiles[i].GetPos()[1]).GetComponent<Tile>().SetWrongIconState(true);
                    }
                }
                else if ((nw == guessTiles[j].GetPos() || sw == guessTiles[j].GetPos() || ne == guessTiles[j].GetPos() || se == guessTiles[j].GetPos())) // diag
                {
                    guessTiles[i].SetWrongIconState(true);
                    guessTiles[j].SetWrongIconState(true);
                }
                else
                {
                    // didnt break any rules
                    continue;
                }

                // check if there are multiple crowns placed in the same color that passed otherwise

                // whatever the ifs do together we'll do here
                brokeRules = true;
            }
        }

        // if they dont break rules, see if they pass the test
        // *note* they dont need to be original but also check they are colored (occupied)
        if (guessTiles.Count == num_fruits && !brokeRules)
        {
            IEnumerable<int> ids = guessTiles.Select(t => t.GetID()).ToList();
            IEnumerable<int> uniqueIDS = ids.Distinct().ToList();
            if (uniqueIDS.Count() == guessTiles.Count)
            {
                Debug.Log("yay?");
            }
        }
    }

    #region helpers
    void SetTilesPositions()
    {
        for (int i = 0; i < tile_rows.Count; i++)
        {
            for (int j = 0; j < tile_rows.Count; j++)
            {
                tile_rows[i].transform.GetChild(j).GetComponent<Tile>().SetPos(new Vector2(i, j));
            }
        }
    }

    Vector2 Roll_Numbers(int min, int max)
    {
        int x = Random.Range(min, max);
        int y = Random.Range(min, max);
        return new Vector2(x, y);
    }

    Vector2 Roll_Rand_Dir()
    {
        int dir = Random.Range(1, 4);
        switch (dir)
        {
            case 1:
                return new Vector2(-1, 0);
            case 2:
                return new Vector2(1, 0);
            case 3:
                return new Vector2(0, -1);
            case 4:
                return new Vector2(0, 1);
        }

        return new Vector2(0, 0); // cant happen
    }

    bool Check_Horizontal(Vector2 test)
    {
        for (int i = 0; i < fruit_spots.Count; i++)
        {
            if (fruit_spots[i][0] == test[0])
                return false;
        }
        return true;
    }

    bool Check_Vertical(Vector2 test)
    {
        for (int i = 0; i < fruit_spots.Count; i++)
        {
            if (fruit_spots[i][1] == test[1])
                return false;
        }
        return true;
    }

    bool Check_Adjacent(Vector2 test)
    {
        Vector2 nw = new Vector2(test[0] - 1, test[1] - 1);
        Vector2 ne = new Vector2(test[0] + 1, test[1] - 1);
        Vector2 sw = new Vector2(test[0] - 1, test[1] + 1);
        Vector2 se = new Vector2(test[0] + 1, test[1] + 1);

        for (int i = 0; i < fruit_spots.Count; i++)
        {
            if (nw == fruit_spots[i] || ne == fruit_spots[i] || sw == fruit_spots[i] || se == fruit_spots[i])
                return false;
        }
        return true;
    }
    #endregion

    #region seeds
    void Rand_Seed()
    {
        DateTime now = DateTime.Now;
        string timeStr = now.ToString("MMddyyyyHHmmssfff"); // add milliseconds
        long fullDateTime = long.Parse(timeStr);
        int seed = (int)(fullDateTime ^ (fullDateTime >> 32));
        Debug.Log(seed);
        Random.InitState(seed);
    }
    #endregion

    #region debug
    void Show_Crowns_Debug()
    {
        for (int i = 0; i < fruit_spots.Count; i++)
        {
            tile_rows[(int)fruit_spots[i][0]].transform.GetChild((int)fruit_spots[i][1]).GetComponent<Tile>().CrownDebugVisibility(true);
        }
    }
    #endregion
}
