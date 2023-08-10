using ChessChallenge.API;

public class MyBot : IChessBot
{
    public const int CHECKMATE = int.MaxValue;
    public struct countSquare{
        Piece piece;
        int myAttack;
        int enemyDefence;
    }

    public int GetPieceValue(PieceType t){
        switch(t){
        case PieceType.Bishop:
        case PieceType.Knight:
            return 3;
        case PieceType.Rook:
            return 5;
        case PieceType.Queen:
            return 9;
        default:
            return 0;
        }
    }

    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();
        Move chosen = Move.NullMove;

        float maxVal = 0, val = 0;

        for(int i = 0; i < moves.Length && maxVal != CHECKMATE; i++){
            val = getPositionValue(board, moves[i]);
            if(val > maxVal){
                maxVal = val;
                chosen = moves[i];
            }
        }

        if(chosen.IsNull){
            System.Random rng = new();
            chosen = moves[rng.Next(moves.Length)];
        }
        return chosen;
    }

    public float getPositionValue(Board board, Move move){
        //CHECKMATE CHECK
        int checkPoint = 0;
        board.MakeMove(move);
        if(board.IsInCheckmate()) return CHECKMATE;
        else if(board.IsInCheck()) checkPoint = 1;
        board.UndoMove(move);

        //CAPTURES
        int capturePoint = 0;
        if(move.IsCapture){
            capturePoint = GetPieceValue(move.CapturePieceType);
        }

        //SECURITY
        PieceList[] allPiece = board.GetAllPieceLists();
        int securityPoint = 39;
        for(int i = 0; i < allPiece.Length; i++){
            for(int j = 0; j < allPiece[i].Count; j++){
                if(board.SquareIsAttackedByOpponent(allPiece[i].GetPiece(j).Square))
                    securityPoint -= GetPieceValue(allPiece[i].GetPiece(j).PieceType);
            }
        }

        return checkPoint * 2
            + capturePoint * 3
            + (float) securityPoint / 39 * 4;
    }
}