using ChessChallenge.API;
using System;
using System.Collections.Generic;
using System.Linq;

public class MyBot : IChessBot
{
    private const int depth = 3; // depth to search
                                
    public Move Think(Board board, Timer timer)
    {
        var bestMove = MiniMax(board, depth, int.MinValue, int.MaxValue, board.IsWhiteToMove).Item2;
        return bestMove;
    }

    private Tuple<double, Move> MiniMax(Board board, int depth, double alpha, double beta, bool maximizingPlayer)
    {
        if (depth == 0 || board.IsInCheckmate() || board.IsDraw())
        {
            return Tuple.Create(EvaluateBoard(board), new Move());
        }

        Move bestMove = new Move();

        if (maximizingPlayer)
        {
            double maxEval = double.MinValue;
            var moves = board.GetLegalMoves();
            foreach (var move in moves)
            {
                board.MakeMove(move);
                var eval = MiniMax(board, depth - 1, alpha, beta, !maximizingPlayer).Item1;
                board.UndoMove(move);
                if (eval > maxEval)
                {
                    maxEval = eval;
                    bestMove = move;
                }
                alpha = Math.Max(alpha, eval);
                if (beta <= alpha)
                {
                    break;
                }
            }
            return Tuple.Create(maxEval, bestMove);
        }
        else
        {
            double minEval = int.MaxValue;
            var moves = board.GetLegalMoves();
            foreach (var move in moves)
            {
                board.MakeMove(move);
                var eval = MiniMax(board, depth - 1, alpha, beta, !maximizingPlayer).Item1;
                board.UndoMove(move);
                if (eval < minEval)
                {
                    minEval = eval;
                    bestMove = move;
                }
                beta = Math.Min(beta, eval);
                if (beta <= alpha)
                {
                    break;
                }
            }
            return Tuple.Create(minEval, bestMove);
        }
    }

    private double EvaluateBoard(Board board)
    {
        var pieceValueDict = new System.Collections.Generic.Dictionary<PieceType, double>()
        {
            { PieceType.Pawn, 1 },
            { PieceType.Knight, 3.1 },
            { PieceType.Bishop, 3.15 },
            { PieceType.Rook, 5 },
            { PieceType.Queen, 9 },
            { PieceType.King,999999 }
        };

        double score = 0;

        foreach (var pieceType in pieceValueDict.Keys)
        {
            var whitePieces = board.GetPieceList(pieceType, true);
            var blackPieces = board.GetPieceList(pieceType, false);
            score += whitePieces.Count() * pieceValueDict[pieceType];
            score -= blackPieces.Count() * pieceValueDict[pieceType];
        }

        return score;
    }
}