using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : MonoBehaviour {
    public Board board;
    public bool white;
    public string position, movePosition, pin = null;
    public List<string> possibleMoves = new List<string>();
    void Start() {
        
    }

    void Update() {
        
    }

    void updatePossibleMoves() {
        if(Utils.validPosition(pin)) { // if this piece is pinned
            List<string>
                betweens = Utils.getPositionsBetween(position, pin),
                illegalMoves = new List<string>();
            foreach(string move in possibleMoves) {
                if(move.Equals(pin)) // move is legal if it captures the pinning piece
                    continue;
                if(betweens.Contains(move)) // move is legal if it is within the pin
                    continue;
                illegalMoves.Add(move); // otherwise, the move is illegal
            }

            // remove all the illegal moves
            foreach(string move in illegalMoves)
                possibleMoves.Remove(move);
            return;
        }
    }
}
