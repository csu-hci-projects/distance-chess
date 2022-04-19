using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : MonoBehaviour {
    public Board board;
    public bool white, alive;
    public string position, movePosition=null;
    public List<string> possibleMoves = new List<string>();

    void Start() {
        board.put(Utils.piece(white, 'q'), position);
    }

    void Update() {
        if(board.needsUpdate(position)) {
            updatePossibleMoves();
        }
    }

    public void updatePossibleMoves() {
        
    }
}
