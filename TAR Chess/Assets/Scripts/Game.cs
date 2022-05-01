using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    public bool playerIsWhite;
    public string engineMoves = "";
    public uint engineMoveIndex = 0;
    private List<Piece> pieces = new List<Piece>(32);

    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void addPiece(Piece piece) {
        pieces.Add(piece);
    }
}
