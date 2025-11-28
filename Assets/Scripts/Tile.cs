using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField] Image fill;
    [SerializeField] GameObject crownDebug;
    [SerializeField] GameObject fruitIcon;
    [SerializeField] GameObject wrongIcon;
    [SerializeField] Button btn;
    int tileID = -1; // for comparing against one another to determine win state
    bool tested = false; // for stepping and placing fruits/colors
    bool occupied = false; // meaning it has a color and shouldnt be tested
    bool hasGuess = false; // has the user clicked it
    Vector2 pos;

    // properties
    public void SetFill(Color col) { fill.color = col; }
    public void CrownDebugVisibility(bool inVis) { crownDebug.SetActive(inVis); }
    public int GetID() { return tileID; }
    public void SetID(int inID) { tileID = inID; }
    public bool GetTested() { return tested; }
    public void SetTested(bool state) { tested = state; }
    public bool GetOccupied() { return occupied; }
    public void SetOccupied(bool state) { occupied = state; }
    public bool GetGuessState() { return hasGuess; }
    public void SetGuessState(bool state) { hasGuess = state; }
    public Vector2 GetPos() { return pos; }
    public void SetPos(Vector2 inPos) { pos = inPos; }
    public bool GetWrongIconState() { return wrongIcon.activeInHierarchy; }
    public void SetWrongIconState(bool state) { wrongIcon.SetActive(state); }

    void Awake()
    {
        btn.onClick.AddListener(() => Signals.TilePressed());
    }

    void Update()
    {
        
    }

    public void Tile_Pressed()
    {
        if (!occupied) // lets just make it not do anything if its not colored
            return;

        fruitIcon.SetActive(!fruitIcon.gameObject.activeInHierarchy);
        hasGuess = !hasGuess;
    }
}
