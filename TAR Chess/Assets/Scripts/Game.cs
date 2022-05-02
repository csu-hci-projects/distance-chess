using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    public Utils util;
    public Utils.PieceColor playerColor = Utils.white;
    public string engineMoves = "";
    public int engineMoveIndex = 0;
    public string engineMove = "";
    private List<Piece> pieces = new List<Piece>(32);
    public Piece[,] board = new Piece[8,8];
    public const float moveTime = 0.1f;

    void Start() {
        
    }

    void Update() {
        if(engineMoveIndex + engineMove.Length != engineMoves.Length) {
            if(engineMoveIndex == engineMoves.Length)
                engineMove = "";
            else
                engineMove = engineMoves.Substring(engineMoveIndex);
        }
        executeMoves();
    }

    public void addPiece(Piece piece) {
        pieces.Add(piece);
        board[piece.file(), piece.rank()] = piece;
    }
    public void kill(string position) => kill(Utils.file(position), Utils.rank(position));
    public void kill(int file, int rank) {
        if(file<0 || file>7 || rank<0 || rank>7)
            return;
        Piece p = board[file,rank];
        if(!(p is null))
            p.die();
        board[file,rank] = null;
    }

    public bool occupied(int file, int rank) {
        return !(board[file,rank] is null);
    }

    public bool validMove() {
        if(engineMove.Length < 2)
            return false;
        if(engineMove[0] == '-' && Utils.validPosition(engineMove.Substring(1))) // kill piece at pos
            return true;
        const string PIECEENUM = "rnsqk";
        if(validateMove(Utils.validPosition(engineMove)))
            return true;
        if(PIECEENUM.Contains(engineMove.Substring(0,1))) {
            return validateMove(Utils.validPosition(engineMove.Substring(1)));
        }

        if("abcdefgh".Contains(engineMove.Substring(0,1))) {
            char mtype = engineMove[1];
            if("abcdefgh".Contains(""+mtype)) { // pawn capture
                if(engineMove.Length == 4 && engineMove[2] == '=') // with promotion, ex. e-capture-d promoting to queen would be `ed=q`
                    return validateMove(
                        engineMove[3] != 'k' && PIECEENUM.Contains(engineMove.Substring(3))
                    );
                return validateMove(Utils.validPosition(engineMove.Substring(1)));
            }
            if(engineMove.Length == 3 && mtype == '=') // pawn promotion, ex. e-pawn to queen would be `e=q`
                return validateMove(engineMove[2] != 'k' && PIECEENUM.Contains(engineMove.Substring(2)));
        }

        if(engineMove.Equals("o o") || engineMove.Equals("ooo"))
            return validateMove(true);

        return false;

    }
        private bool validateMove(bool predicate) {
            if(predicate) {
                engineMoveIndex = engineMoves.Length;
                return true;
            } else
                return false;
        }
    
    Dictionary<Piece, string> moves = new Dictionary<Piece, string>();
    Dictionary<Piece, string> done = new Dictionary<Piece, string>();
    void executeMoves() {
        getMoves();
        foreach(KeyValuePair<Piece, string> move in moves) {
            Piece piece = move.Key;
            if(piece.glideTo(move.Value)) done.Add(move.Key, move.Value);
        }
        foreach(Piece p in done.Keys) {
            moves.Remove(p);
            if(p.type == Utils.pawn)
                ((Pawn) p).hasMoved = true;
            if(p.type == Utils.king)
                ((King) p).hasMoved = true;
            if(p.type == Utils.rook)
                ((Rook) p).hasMoved = true;
            if(p.type == Utils.pawn && engineMoves.Substring(engineMoveIndex - 2)[0] == '=') {
                char pType = engineMoves[engineMoveIndex-1];
                promotePawn(done[p], (
                    pType == 'r'? Utils.rook :
                    pType == 'n'? Utils.knight :
                    pType == 's'? Utils.bishop :
                    pType == 'q'? Utils.queen :
                    Utils.pawn
                ));
            }
            signalAllPiecesForUpdates();
        }
        done.Clear();
    }
    void getMoves() {
        if(!validMove())
            return;
        if(moves.Count == 0) {
            if(engineMove[0] == '-') {
                kill(engineMove.Substring(1));
                engineMove = "";
                engineMoveIndex = engineMoves.Length;
                return;
            } else foreach(Piece piece in pieces) {
                if(appliesToPiece(piece)) {
                    moves.Add(piece, getMovePosition(piece));
                }
            }
            if(moves.Count > 0) {
                engineMove = "";
                engineMoveIndex = engineMoves.Length;
            }
        }
    }
    bool appliesToPiece(Piece piece) {
        if(Utils.validPosition(engineMove)) {
            if(piece.type != Utils.pawn)
                return false;
            if(piece.file() != Utils.file(engineMove))
                return false;
            return piece.validMove(engineMove);
        } else if(engineMove[0] >= 'a' && engineMove[0] <= 'h') {
            if(piece.type != Utils.pawn)
                return false;
            if(piece.position[0] != engineMove[0])
                return false;
            if(engineMove[1] == '=' && "rnsq".Contains(engineMove.Substring(2)))
                return piece.validMove(getMovePosition(piece));
            if(engineMove.Length == 4 && engineMove[2] == '=' && "rnsq".Contains(engineMove.Substring(3)))
                return piece.validMove(getMovePosition(piece));

            return piece.validMove(engineMove.Substring(1));
        } else if(engineMove.Contains("o")) {
            if(piece.type == Utils.king && !((King) piece).hasMoved)
                return true;
            else if(piece.type == Utils.rook && !((Rook) piece).hasMoved) {
                string correctFile = engineMove.Contains(" ")? "h":"a"; // 'h' if short, 'a' if long
                return piece.position.Contains(correctFile);
            } else
                return false;
        }

        return false;
    }
    string getMovePosition(Piece piece) {
        if(engineMove is null || engineMove.Length < 2)
            return null;
        if(engineMove.Length == 3) {
            char file = engineMove[0];
            if(file >= 'a' && file <= 'h') { // it's a pawn
                if(engineMove[1] == '=') { // it's a promotion
                    string rank = playerColor == Utils.white? "1":"8";
                    return file + rank;
                }
            }
            if(engineMove.Contains("o")) { // castle
                string rank = (piece.color == Utils.white? "1":"8");
                if(engineMove.Equals("o o")) // short
                    return (piece.type == Utils.king? "c":"d") + rank;
                else // if(engineMove.Equals("ooo")) // long
                    return (piece.type == Utils.king? "g":"f") + rank;
            }
        }
        if(engineMove.Length == 4) {
            char file = engineMove[0];
            if(file >= 'a' && file <= 'h') {
                char nfile = engineMove[1];
                if(nfile >= 'a' && nfile <= 'h' && engineMove[2] == '=') {
                    string rank = playerColor == Utils.white? "1":"8";
                    return "" + nfile + rank;
                }
            }
        }
        return engineMove.Substring(engineMove.Length - 2);
    }
    void signalAllPiecesForUpdates() {
        foreach(Piece p in pieces)
            p.updated = false;
    }

    void promotePawn(string position, Utils.PieceType toPieceType) {
        Debug.Log("Trying to promote pawn at "+position);
        if(toPieceType == Utils.pawn)
            return;
        Debug.Log("Valid promotion type "+toPieceType);
        kill(position);
        GameObject obj = util.spawnPiece(this, toPieceType, position);
        obj.SetActive(true);
        Piece piece = obj.GetComponent<Piece>();
        piece.game = this;
        piece.reposition(position);
        addPiece(piece);
    }
}
