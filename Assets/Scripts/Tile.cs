using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField] Image fill;
    [SerializeField] GameObject crownDebug;
    [SerializeField] GameObject fruitIcon;
    [SerializeField] Button btn;
    bool tested = false;
    bool occupied = false;

    // properties
    public void SetFill(Color col) { fill.color = col; }
    public void CrownDebugVisibility(bool inVis) { crownDebug.SetActive(inVis); }
    public bool GetTested() { return tested; }
    public void SetTested(bool state) { tested = state; }
    public bool GetOccupied() { return occupied; }
    public void SetOccupied(bool state) { occupied = state; }

    void Awake()
    {
        btn.onClick.AddListener(() => Signals.TilePressed());
    }

    void Update()
    {
        
    }

    public void Tile_Pressed()
    {
        fruitIcon.SetActive(!fruitIcon.gameObject.activeInHierarchy);
    }
}
