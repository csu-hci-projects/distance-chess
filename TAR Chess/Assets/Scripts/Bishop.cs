using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : MonoBehaviour {
    public Board board;
    public bool white;
    public string position, movePosition, pin = null;
    public List<string> possibleMoves = new List<string>();
    void Start() {
        board.put(Utils.piece(white, 'b'), position);
    }

    void Update() {
        if(board.needsUpdate(position)) {
            updatePossibleMoves();
        }
    }

    void updatePossibleMoves() {
        possibleMoves = Utils.getBishopAttacksFrom(board, position);
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
