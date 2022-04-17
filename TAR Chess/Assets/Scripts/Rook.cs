using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : MonoBehaviour {
    public Board board;
    public bool white;
    public string position, movePosition, pin = null;
    public List<string> possibleMoves = new List<string>();

    void Start() {
        board.put(Utils.piece(white, 'r'), position);
    }

    // Update is called once per frame
    void Update() {
        if(board.needsUpdate(position)) {
            updatePossibleMoves();
        }
    }

    void updatePossibleMoves() {

    }
}
