using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OldScripts {
    public class Queen : MonoBehaviour {
        public Board board;
        public bool white, alive;
        public string position, movePosition=null, pin=null;
        public List<string> possibleMoves = new List<string>();

        void Start() {
            board.put(Utils.piece(white, 'q'), position);
        }

        void Update() {
            movePosition = Utils.backPieceMoveFromPGN(board.moveToMake, "Q");
            if(!(movePosition is null))
                movePosition = Utils.validateMovePosition(
                    movePosition, white, board, possibleMoves, position, "Q"
                );

            if(board.needsUpdate(position)) {
                if(Utils.pieceColor(board.pieceAt(position)) != (white? 'w':'b'))
                    gameObject.SetActive(false);
                updatePossibleMoves();
            }
            
            if(Utils.updateMove(board, transform, position, movePosition)) {
                position = Utils.position(Utils.file(movePosition), Utils.rank(movePosition));
                movePosition = null;
            }
        }

        public void updatePossibleMoves() {
            possibleMoves = Utils.getQueenAttacksFrom(board, white, position);
            List<string> illegalMoves = new List<string>();
            foreach(string move in possibleMoves) {
                string piece = board.pieceAt(move);
                if(Utils.validPiece(piece) && Utils.pieceColor(piece) == (white? 'w':'b'))
                    illegalMoves.Add(move);
            }
            if(Utils.validPosition(pin)) { // if this piece is pinned
                List<string> betweens = Utils.getPositionsBetween(position, pin);
                foreach(string move in possibleMoves) {
                    if(move.Equals(pin)) // move is legal if it captures the pinning piece
                        continue;
                    if(betweens.Contains(move)) // move is legal if it is within the pin
                        continue;
                    illegalMoves.Add(move); // otherwise, the move is illegal
                }
            }

            // remove all the illegal moves
            foreach(string move in illegalMoves)
                possibleMoves.Remove(move);
        }
    }
}
