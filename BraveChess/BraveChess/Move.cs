using System.Text;
using BraveChess.Objects;
using BraveChess.Helpers;

namespace BraveChess
{
    public class Move
    {
        public string Algebraic{ get {return ToAlgebraic();}}

        public Square FromSquare { get; set; }
        public Square ToSquare { get; set; }
        public Piece PieceMoved { get; set; }
        public Piece PieceCaptured { get; set; }
        public Piece.PieceType PiecePromoted { get; set; }
        public bool HasPromoted
        {
            get
            {
                return PiecePromoted != Piece.PieceType.None;
            }
        }
        public bool IsEnpassant { get; set; }
        public bool HasCaptured { get; set; }
        public bool IsCastling
        {
            get
            {
                return IsLongCastling || IsShortCastling;
            }
        }
        public bool IsLongCastling
        {
            get
            {
                if (PieceMoved.Piece_Type == Piece.PieceType.King)
                {
                    if (FromSquare.File - ToSquare.File == 2)
                        return true;
                }
                return false;
            }
        }
        public bool IsShortCastling
        {
            get
            {
                if (PieceMoved.Piece_Type == Piece.PieceType.King)
                {
                    if (FromSquare.File - ToSquare.File == -2)
                        return true;
                }
                return false;
            }
        }
        public bool IsCheck { get; set; }
        public bool IsCheckMate { get; set; }
        public bool IsDoublePawnPush
        {
            get
            {
                if (PieceMoved.Piece_Type == Piece.PieceType.Pawn)
                    if (!HasCaptured)
                        if ((BitboardHelper.GetBitboardFromSquare(ToSquare) << 16) == BitboardHelper.GetBitboardFromSquare(FromSquare))
                            return true;
                        else if ((BitboardHelper.GetBitboardFromSquare(ToSquare) >> 16) == BitboardHelper.GetBitboardFromSquare(FromSquare))
                             return true;
                return false;
            }
        }
        public Piece.Color SideMove
        {
            get
            {
                return PieceMoved.ColorType;
            }
        }

        public Move(Square fromSquare, Square toSquare, Piece pieceMoved, bool isCapture, Piece.PieceType piecePromoted = Piece.PieceType.None)
        {
            FromSquare = fromSquare;
            ToSquare = toSquare;
            PieceMoved = pieceMoved;
            PiecePromoted = piecePromoted;
            HasCaptured = isCapture;
        }

        public string ToAlgebraic()
        {
            StringBuilder algebraic = new StringBuilder();

            if (IsCastling)
            {
                algebraic.Append(IsShortCastling ? "O-O" : "O-O-O");
            }
            else
            {
                algebraic.Append(FromSquare.ToAlgebraic());

                if (HasCaptured)
                    algebraic.Append("x");

                algebraic.Append(ToSquare.ToAlgebraic());
            }

            if (HasPromoted)
                algebraic.Append("=" + GetInitial(PiecePromoted));

            if (IsCheck)
                algebraic.Append(IsCheckMate ? "#" : "+");

            return algebraic.ToString();
        }

        private string GetInitial(Piece.PieceType type)
        {
            switch (type)
            {
                case Piece.PieceType.Bishop:
                    return "B";
                case Piece.PieceType.Knight:
                    return "K";
                case Piece.PieceType.Queen:
                    return "Q";
                case Piece.PieceType.Rook:
                    return "R";
                default:
                    return "initial";
            }
        }
    }
}
