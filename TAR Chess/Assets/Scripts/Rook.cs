using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : MonoBehaviour {
    public Board board;
    public bool white;
    public string position, movePosition, pin = null;
    public List<string> possibleMoves = new List<string>();

    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }
}
