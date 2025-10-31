using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField] Image fill;
    [SerializeField] GameObject crownDebug;
    bool tested = false;

    // properties
    public void SetFill(Color col) { fill.color = col; }
    public void CrownDebugVisibility(bool inVis) { crownDebug.SetActive(inVis); }
    public bool GetTested() { return tested; }
    public void SetTested() { tested = true; }

    void Awake()
    {
        
    }

    void Update()
    {
        
    }
}
