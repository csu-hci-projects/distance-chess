using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    public bool playerIsWhite;
    public string engineMoves = "";
    public int engineMoveIndex = 0;
    private string engineMove = "";
    private List<Piece> pieces = new List<Piece>(32);
    public Piece[,] board = new Piece[8,8];
    public const float moveTime = 0.1f;

    void Start() {
        
    }

    void Update() {
        if(engineMoveIndex + engineMove.Length < engineMoves.Length) {
            engineMove = engineMoves.Substring(engineMoveIndex);
            Debug.Log(engineMove);
        }
        if(engineMoveIndex < engineMoves.Length && validMove()) {
            executeMove();
        }
    }

    public void addPiece(Piece piece) {
        pieces.Add(piece);
        board[piece.file(), piece.rank()] = piece;
    }

    public bool occupied(int file, int rank) {
        return board[file,rank] is null;
    }

    public bool validMove() {
        if(engineMove.Length < 2)
            return false;
        const string PIECEENUM = "rnsqk";
        if(validateMove(Piece.validPosition(engineMove)))
            return true;
        if(PIECEENUM.Contains(engineMove.Substring(0,1))) {
            return validateMove(Piece.validPosition(engineMove.Substring(1)));
        }

        if("abcdefgh".Contains(engineMove.Substring(0,1))) {
            char mtype = engineMove[1];
            if("abcdefgh".Contains(""+mtype))
                return validateMove(Piece.validPosition(engineMove.Substring(1)));
            if(mtype == '=')
                return validateMove(PIECEENUM.Contains(engineMove.Substring(2)));
        }

        return false;

    }
        private bool validateMove(bool predicate) {
            if(predicate) {
                engineMoveIndex = engineMoves.Length;
                return true;
            } else
                return false;
        }
    
    Piece movingPiece = null;
    string movePosition = "";
    void executeMove() {
        if(movingPiece is null) {
            foreach(Piece piece in pieces) {
                if(appliesToPiece(piece)) {
                    movingPiece = piece;
                    break;
                }
            }
        }
        if(movingPiece is null)
            return;
        if(movePosition is null)
            movePosition = getMovePosition();
        
        movingPiece.glideTo(movePosition);
    }
    bool appliesToPiece(Piece piece) {
        if(Piece.validPosition(movePosition)) {
            if(piece.type != Piece.PieceType.pawn)
                return false;
            return piece.validMove(movePosition);
        }

        return false;
    }
    string getMovePosition() {
        if(engineMove is null || engineMove.Length < 2)
            return null;
        
        return null;
    }
}
