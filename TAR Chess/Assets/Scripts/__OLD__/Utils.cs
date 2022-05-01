using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static int numPiecesUpdated = 0;
    public static float moveTime = 0.1f;
    public static List<string> pins;
    public const string FILE = "abcdefgh";
    public const string RANK = "12345678";
    public static Vector3 NULL_COORDS = new Vector3(-1,-1,-1);
    public static Dictionary<bool, string> kingPosition = new Dictionary<bool, string>();

    public static string position(int file, int rank) {
        if(file < 0 || file > 7 || rank < 0 || rank > 7) return null;
        return FILE.Substring(file,1) + RANK.Substring(rank,1);
    }
    public static bool validPosition(string position) {
        return !(position is null) && position.Length == 2 && file(position) != -1 && rank(position) != -1;
    }
    public static int file(string position) { return FILE.IndexOf(Char.ToLower(position[0])); }
    public static int rank(string position) { return RANK.IndexOf(position[1]); }

    public static string positionFrom(string position, int fileDistance, int rankDistance) {
        int file = Utils.file(position), rank = Utils.rank(position);
        file += fileDistance; rank += rankDistance;
        return Utils.position(file, rank);
    }

    public static string piece(bool white, char type) {
        return (white? "w":"b") + char.ToUpper(type);
    }
    public static bool validPiece(string piece) {
        if(piece is null || piece.Length != 2)
            return false;
        if(!"wb".Contains("" + piece[0]))
            return false;
        if(!"PKQRNB".Contains("" + piece[1]))
            return false;

        return true;
    }
    public static char pieceType(string piece) {
        return piece[1];
    }
    public static char pieceColor(string piece) {
        return piece[0];
    }
    public static bool pieceIsWhite(string piece) {
        return pieceColor(piece) == 'w';
    }

    public static void pinPosition(string pinningPosition, string pinnedPosition) {
        pins.Add(pinningPosition + ">" + pinnedPosition);
    }
    public static void unpinPosition(string pinningPosition, string pinnedPosition) {
        pins.Remove(pinningPosition + ">" + pinnedPosition);
    }

    public static bool validUnderPin(string position, string move) {
        int numPins = 0; string pinningPosition = null;
        foreach(string pin in pins) {
            if(pin.EndsWith(position)) {
                ++numPins;
                if(numPins > 1) return false;
                pinningPosition = pin.Substring(0,2);
            }
        }
        if(pinningPosition is null) return true;
        return move.Equals(pinningPosition) || positionBetween(position, move, pinningPosition);
    }

    public static bool positionBetween(string posA, string posB, string posC) {
        return getPositionsBetween(posA,posC).Contains(posB);
    }

    public static bool sameFile(string positionA, string positionB) {
        return file(positionA) == file(positionB);
    }
    public static bool sameRank(string positionA, string positionB) {
        return rank(positionA) == rank(positionB);
    }
    public static bool diagonal(string positionA, string positionB) {
        return Math.Abs(file(positionA) - file(positionB)) == Math.Abs(rank(positionA) - rank(positionB));
    }

    public static List<string> getPositionsBetween(string positionA, string positionB) {
        List<string> positions = new List<string>();
        int fa= file(positionA), fb= file(positionB);
        int ra=rank(positionA), rb=rank(positionB);
        if(sameFile(positionA,positionB)) {
            int file = fa;
            int dir = (rb > ra)? 1:-1;
            for(int rank=ra+dir; rank<rb; rank += dir)
                positions.Add(position(file,rank));
        }
        if(sameRank(positionA,positionB)) {
            int rank = ra;
            int dir = (fb > fa)? 1:-1;
            for(int file=fa+dir; file<fb; file += dir)
                positions.Add(position(file,rank));
        }
        if(diagonal(positionA,positionB)) {
            int fdir = (fb > fa)? 1:-1;
            int rdir = (rb > ra)? 1:-1;
            for(int file=fa+fdir, rank=ra+rdir; file<fb; ) {
                positions.Add(position(file,rank));
                file += fdir;
                rank += rdir;
            }
        }
        return positions;
    }

    public static Vector3 getLocalCoordsFromPosition(String position) {
        if(position is null || position.Length != 2) return NULL_COORDS;
        float f=file(position), r=rank(position);
        return new Vector3(f, 0, r);
    }
    public static bool pieceAt(Transform transform, String position) {
        if(position is null || position.Length != 2) return false;
        if(_MOVE_position is null) {
            _MOVE_position = position;
            _MOVE_coords = getLocalCoordsFromPosition(position);
        }
        if(Vector3.Distance(transform.localPosition, _MOVE_coords) < 0.001f) {
            _MOVE_position = null;
            _MOVE_speed = -1f;
            return true;
        }
        return false;
    }
    public static Vector3 moveTowards(Transform transform, String position) {
        if(position is null || position.Length != 2) return transform.localPosition;
        if(_MOVE_position is null || _MOVE_position.Length == 0) {
            _MOVE_position = position;
            _MOVE_coords = getLocalCoordsFromPosition(position);
        }
        if(_MOVE_speed == -1f) {
            _MOVE_speed = Vector3.Distance(transform.localPosition, _MOVE_coords) / moveTime;
        }
        return Vector3.MoveTowards(
            transform.localPosition,
            _MOVE_coords,
            _MOVE_speed * Time.deltaTime
        );
    }
        private static String _MOVE_position = null;
        private static Vector3 _MOVE_coords;
        private static float _MOVE_speed = -1f;
    
    public static string getPawnAttackPosition(bool left, bool white, string position) {
        if(!validPosition(position))
            return null;
        int forward = white? 1:-1, side = left? -1:1;
        return positionFrom(position, side, forward);
    }

    // returns true when move is completed
    public static bool updateMove(Board board, Transform transform, String currentPosition, String movePosition) {
        // check for a move position
        if(!validPosition(movePosition))
            return false;
        
        // if the piece has completed its move animation
        if(pieceAt(transform, movePosition)) {
            // update the board position
            board.movePiece(currentPosition, movePosition);
            // update local position
            transform.localPosition = getLocalCoordsFromPosition(movePosition);

            return true;
        }
        // otherwise, move towards the new position
        transform.localPosition = moveTowards(transform, movePosition);
        return false;
    }
    public static bool updateMove(King king) {
        if(!validPosition(king.movePosition))
            return false;
        string pgn = cleanPGN(king.board.moveToMake);
        Rook rook = null;
        string pos ="";
        string rpos = "";
        bool rdone = false;
        
        if(pgn.Equals("o o")) {
            rook = king.kingsRook;
            pos = king.white? "g1":"g8";
            rpos = king.white? "f1":"f8";
        } else if(pgn.Equals("ooo")) {
            rook = king.queensRook;
            pos = king.white? "c1":"c8";
            rpos = king.white? "d1":"d8";
        } else {
            return updateMove(king.board, king.transform, king.position, king.movePosition);
        }

        Debug.Log("For move "+pgn+", king moving to "+pos);

        rdone = updateMove(rook.board, rook.transform, rook.position, rpos);

        if(pieceAt(king, pos) && rdone) {
            king.board.movePiece(king.position, pos);

            // ensure the double-call of board.movePiece doesn't mess up the move order
            king.board.whitesMove = !king.white;

            king.transform.localPosition = getLocalCoordsFromPosition(pos);
            
            rook.position = position(file(rpos), rank(rpos));
            rook.firstMove = false;
            rook.movePosition = null;

            king.updatePossibleMoves();
            rook.updatePossibleMoves();
            
            return true;
        }

        king.transform.localPosition = moveTowards(king, pos);

        return false;
    }
        private static bool pieceAt(King king, string position) {
        if(_CASTLE_position is null) {
            _CASTLE_position = position;
            _CASTLE_coords = getLocalCoordsFromPosition(position);
        }
        if(Vector3.Distance(king.transform.localPosition, _CASTLE_coords) < 0.001f) {
            _CASTLE_position = null;
            _MOVE_speed = -1f;
            return true;
        }
        return false;
        }
        private static Vector3 moveTowards(King king, string position) {
        if(_CASTLE_position is null || _CASTLE_position.Length == 0) {
            _CASTLE_position = position;
            _CASTLE_coords = getLocalCoordsFromPosition(position);
        }
        if(_CASTLE_speed == -1f) {
            _CASTLE_speed = Vector3.Distance(king.transform.localPosition, _CASTLE_coords) / moveTime;
        }
        return Vector3.MoveTowards(
            king.transform.localPosition,
            _CASTLE_coords,
            _CASTLE_speed * Time.deltaTime
        );
        }
        private static string _CASTLE_position = null;
        private static float _CASTLE_speed = -1f;
        private static Vector3 _CASTLE_coords;

    public static List<string> getKingAttacksFrom(string position) {
        List<string> attacks = new List<string>();
        int file = Utils.file(position), rank = Utils.rank(position);
        for(int f = file - 1; f <= file + 1; ++f) {
            if(f < 0 || f > 7)
                continue;
            for(int r = rank - 1; r <= rank + 1; ++r) {
                if(r < 0 || r > 7)
                    continue;
                if(f == file && r == rank)
                    continue;
                
                attacks.Add(Utils.position(f,r));
            }
        }
        return attacks;
    }
    public static List<string> getPawnAttacksFrom(bool white, string position) {
        List<string> attacks = new List<string>();
        string pos = Utils.positionFrom(position, -1, white? 1:-1);
        if(!(pos is null)) attacks.Add(pos);
        pos = Utils.positionFrom(position, 1, white? 1:-1);
        if(!(pos is null)) attacks.Add(pos);
        return attacks;
    }
    public static List<string> getKnightAttacksFrom(string position) {
        List<string> attacks = new List<string>();
        for(int f=-2; f < 3; ++f) {
            if(f == 0) continue;
            for(int r = -1; r < 2; r += 2) {
                string attack = positionFrom(position,
                    f, (Math.Abs(f)==1? 2:1) * r
                );
                if(attack is null)
                    continue;
                attacks.Add(attack);
            }
        }
        return attacks;
    }
    public static List<string> getBishopAttacksFrom(Board board, bool white, string position) {
        string bishop = white? "w":"b" + "b";
        List<string> attacks = new List<string>();
        int file=Utils.file(position), rank=Utils.rank(position);
        bool[] shouldContinue = new bool[4];
        for(int i=0; i<4; ++i) shouldContinue[i] = true;
        int numDirs = 4;
        for(int distance = 1; numDirs > 0; ++distance) {
            // for each direction (up left, up right, down right, then down left)
            for(int dir=0; dir < 4; ++dir) {
                // first check if the direction still needs to be explored
                if(!shouldContinue[dir]) continue;
                // fd ~ file distance, rd ~ rank distance
                int fd = distance, rd = distance;
                fd *= (dir % 3 == 0)? -1 : 1; // if left (dir 0 or 3), negate it
                rd *= (dir > 1)? -1 : 1; // if down (dir 2 or 3), negate it

                // get the attack position for this direction, then check it
                string attack = positionFrom(position, fd, rd);
                string info = getAttackInfo(board, bishop, attack);
                if(info is null) { // the move position does not exist
                    shouldContinue[dir] = false;
                    --numDirs;
                } else { // the attack exists, so we
                    // add the position, then
                    attacks.Add(attack);
                    // designate whether to continue exploring the diagonal
                    // - iff the square is empty, we should continue
                    if(info[0] != 'E') {
                        shouldContinue[dir] = false;
                        --numDirs;
                    }
                }
            }
        }
        return attacks;
    }
    public static List<string> getRookAttacksFrom(Board board, bool white, string position) {
        string rook = white? "w":"b" + "r";
        List<string> attacks = new List<string>();

        int file=Utils.file(position), rank=Utils.rank(position);
        bool[] shouldContinue = new bool[4];
        for(int i=0; i<4; ++i) shouldContinue[i] = true;
        int numDirs=4;
        for(int distance=1; numDirs>0; ++distance) {
            // for each direction (left, up, right, down)
            for(int dir=0; dir<4; ++dir) {
                if(!shouldContinue[dir]) continue;
                int
                    fd = (dir % 2 == 0)? distance : 0,
                    rd = (dir % 2 != 0)? distance : 0;
                if(dir == 0) fd *= -1;
                if(dir == 3) rd *= -1;

                string attack = positionFrom(position, fd, rd);
                string info = getAttackInfo(board, rook, attack);
                if(info is null) { // the move position does not exist
                    shouldContinue[dir] = false;
                    --numDirs;
                } else { // the attack exists, so we
                    // add the position, then
                    attacks.Add(attack);
                    // designate whether to continue exploring the diagonal
                    // - iff the square is empty, we should continue
                    if(info[0] != 'E') {
                        shouldContinue[dir] = false;
                        --numDirs;
                    }
                }
            }
        }

        return attacks;
    }
    public static List<string> getQueenAttacksFrom(Board board, bool white, string position) {
        List<string> attacks = getBishopAttacksFrom(board, white, position);
        attacks.AddRange(getRookAttacksFrom(board, white, position));
        return attacks;
    }

    // getAttackInfo(): useful for checking whether an attack is valid, and if it is
    //   whether the attack would capture a piece or not
    // - mostly useful for something rooks, bishops, & queens, for determining whether or not to
    //   continue checking for attacks beyond the position
    private static string getAttackInfo(Board board, string attackingPiece, string attackedPosition) {
        bool valid = true, sameColor = false;
        string attackedPiece = null;
        // is the attacked position on the board?
        if(!validPosition(attackedPosition))
            valid = false;
        else { // if it is, then
            // get the piece on the attacked square if it exists
            attackedPiece = board.pieceAt(attackedPosition);
            if(validPiece(attackedPiece)) { // if it does
                // check if the piece is capturable (i.e., other color)
                sameColor = pieceColor(attackingPiece) == pieceColor(attackedPiece);
            }
        }

        if(valid) { // if the attack square exists, return a string with
            // 1. a character for "Guarded", "Capturable", or "Empty"
            // 2. the attacked position
            return validPiece(attackedPiece)? (sameColor? "G":"C"):"E" + attackedPosition;
        }
        return null;
    }

    public static string getPin(List<string> pins, string position) {
        if(!validPosition(position))
            return null;
        foreach(string pin in pins) {
            if(pin.Contains(position))
                return pin.Substring(0,2);
        }
        return null;
    }

    public static string cleanPGN(string pgnMove) {
        if(pgnMove is null) return null;
        if(pgnMove.Contains("ooo")) return "ooo";
        if(pgnMove.Contains("o o")) return "o o";
        string res = pgnMove.Replace("x", "");
        res = res.Replace("+", "");
        res = res.Replace("#", "");
        return res;
    }
    private static bool PGN_tooShort(string pgnMove, int minLength) {
        return pgnMove is null || pgnMove.Length < minLength;
    }
    public static string pawnMoveFromPGN(string pgnMove) {
        string move = cleanPGN(pgnMove);
        if(PGN_tooShort(move, 2)) return null;

        if(validPosition(move)) return move;
        if(!FILE.Contains(""+move[0])) return null;
        
        return move.Substring(1);
    }
    public static string backPieceMoveFromPGN(string pgnMove, string pieceType) {
        string move = cleanPGN(pgnMove);
        if(PGN_tooShort(move, 3)) return null;

        if(!move.Contains(pieceType)) return null;

        move = move.Substring(move.Length - 2);
        if(!validPosition(move)) return null;
        return move;
    }
    public static string kingMoveFromPGN(string pgnMove, King king) {
        string move = cleanPGN(pgnMove);
        if(PGN_tooShort(move, 3)) return null;

        if(move.Equals("o o")) { // short castle (kingside)
            return king.white? "g1":"g8";
        }
        if(move.Equals("ooo")) { // long castle (queenside)
            return king.white? "c1":"c8";
        }

        return backPieceMoveFromPGN(move, "K");
    }
    public static string rookMoveFromPGN(string pgnMove, Rook rook) {
        string move = cleanPGN(pgnMove);
        if(PGN_tooShort(move, 3)) return null;

        if(move.Equals("o o")) {
            if(!rook.kingside)
                return null;
            return rook.white? "f1":"f8";
        }
        if(move.Equals("ooo")) {
            if(rook.kingside)
                return null;
            return rook.white? "d1":"d8";
        }

        return backPieceMoveFromPGN(move, "R");
    }

    public static string validateMovePosition(string movePosition, bool white, Board board, List<string> possibleMoves, string position, string pieceType) {
        if(white != board.whitesMove)
            return null;
        if(!possibleMoves.Contains(movePosition)) 
            return null;

        string move = Utils.cleanPGN(board.moveToMake);
        move = move.Replace(pieceType, "");
        if(move.Contains("o")) {
            if(!pieceType.Equals("R")) return null;
            else return movePosition;
        }

        if(move.Length == 3) {
            if(move[0] != position[0] && move[0] != position[1])
                return null;
        } else if(move.Length == 4) {
            if(!move.Substring(0,2).Equals(position))
                return null;
        }

        return movePosition;
    }
}
