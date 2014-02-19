using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using BraveChess.Engines;
using BraveChess.Base;
using BraveChess.Objects;

namespace BraveChess.Scenes
{
    class Level0 : Scene
    {
        protected List<Piece> _pieces = new List<Piece>();
        public List<Piece> Pieces { get { return _pieces; } }

        Square[,] Squares;

        BitboardHelper b = new BitboardHelper();

        bool whiteTurn = true;

        Camera _camera, _cameraOtherSide;
        float aspectRatio = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.AspectRatio;

        #region Bitboards
        
        /* white pieces */
        UInt64 white_pawns = 0xFF00LU;
        UInt64 white_knights = 0x42LU;
        UInt64 white_bishops = 0;  //temporary, initial should be 0X24LU
        UInt64 white_rooks = 0x81LU;
        UInt64 white_queens = 0x8LU;
        UInt64 white_kings = 0x10LU;
        /* black pieces */
        UInt64 black_pawns = 0xFF000000000000LU;
        UInt64 black_knights = 0x4200000000000000LU;
        UInt64 black_bishops = 0x2400000000000000LU;
        UInt64 black_rooks = 0x8100000000000000LU;
        UInt64 black_queens = 0x800000000000000LU;
        UInt64 black_kings = 0x1000000000000000LU;

        public UInt64 WhitePieces { get { return white_bishops | white_kings | white_knights | white_pawns | white_queens | white_rooks; } }
        public UInt64 BlackPieces { get { return black_bishops | black_kings | black_knights | black_pawns | black_queens | black_rooks; } }
        public UInt64 AllPieces { get { return WhitePieces | BlackPieces; } }

        #endregion

        //Piece cBoard;

        int _currentI, _currentJ;
        bool PieceIsSelected = false, SquareIsSelected = false, IsFight = false;
        Piece PieceToCapture, PieceToMove;
        Square _currentSquare, _previousSquare, _goFromSquare, _goToSquare;
        List<Square> Moves;

        public Level0(GameEngine _engine)
            : base("Level0", _engine) { }

        public override void Initialize()
        {
            _camera = new Camera("cam0",
                new Vector3(30,60,130),
                new Vector3(30, 5, 30),
                aspectRatio);


            Engine.Cameras.AddCamera(_camera);

            //loading songs//efects
            Engine.Audio.LoadSong("BackgroundSong");
            Engine.Audio.PlaySong("BackgroundSong");
            MediaPlayer.IsRepeating = true;
            Engine.Audio.LoadEffect("move");


            #region Init Squares

            Squares = new Square[8, 8];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (i % 2 == 0)
                    {
                        if (j % 2 == 0)
                        {
                            // The dark coloured.. pass false for black
                            Squares[i, j] = new Square("square" + i + j, new Vector3(10 * i, 0, 70 - (10 * j)), i, j, false);
                        }
                        else if (j % 2 == 1)
                        {
                            // ..and the light coloured.. pass true for white
                            Squares[i, j] = new Square("square" + i + j, new Vector3(10 * i, 0, 70 - (10 * j)), i, j, true);
                        }
                    }
                    else if (i % 2 == 1)
                    {
                        if (j % 2 == 1)
                        {
                            Squares[i, j] = new Square("square" + i + j, new Vector3(10 * i, 0, 70-(10*j)), i, j, false);
                        }
                        else if (j % 2 == 0)
                        {
                            Squares[i, j] = new Square("square" + i + j, new Vector3(10 * i, 0, 70-(10*j)), i, j, true);
                        }
                    }
                }
            }


            Squares[_currentI, _currentJ].IsHover = true;
#endregion

            #region White Piece Init
            //White Pawn Set
            _pieces.Add(new Piece("Pawn1", "White Pawn", GetStartPos("a2"), 1, Piece.PieceType.Pawn));
            _pieces.Add(new Piece("Pawn2", "White Pawn", GetStartPos("b2"), 1, Piece.PieceType.Pawn));
            _pieces.Add(new Piece("Pawn3", "White Pawn", GetStartPos("c2"), 1, Piece.PieceType.Pawn));
            _pieces.Add(new Piece("Pawn4", "White Pawn", GetStartPos("d2"), 1, Piece.PieceType.Pawn));
            _pieces.Add(new Piece("Pawn5", "White Pawn", GetStartPos("e2"), 1, Piece.PieceType.Pawn));
            _pieces.Add(new Piece("Pawn6", "White Pawn", GetStartPos("f2"), 1, Piece.PieceType.Pawn));
            _pieces.Add(new Piece("Pawn7", "White Pawn", GetStartPos("g2"), 1, Piece.PieceType.Pawn));
            _pieces.Add(new Piece("Pawn8", "White Pawn", GetStartPos("h2"), 1, Piece.PieceType.Pawn));

            _pieces.Add(new Piece("Rook1", "White Rook", GetStartPos("a1"), 1, Piece.PieceType.Rook));
            _pieces.Add(new Piece("Rook2", "White Rook", GetStartPos("h1"), 1, Piece.PieceType.Rook));

            _pieces.Add(new Piece("King1", "White King", GetStartPos("e1"), 1, Piece.PieceType.King));

            _pieces.Add(new Piece("Queen1", "White Queen", GetStartPos("d1"), 1, Piece.PieceType.Queen));

            _pieces.Add(new Piece("Knight1", "White Knight", GetStartPos("b1"), 1, Piece.PieceType.Knight));
            _pieces.Add(new Piece("Knight1", "White Knight", GetStartPos("g1"), 1, Piece.PieceType.Knight));


            _pieces.Add(new Piece("Rook1", "White Rook", GetStartPos("a1"), 1, Piece.PieceType.Rook));
            _pieces.Add(new Piece("Rook2", "White Rook", GetStartPos("h1"), 1, Piece.PieceType.Rook));

            _pieces.Add(new Piece("Bishop1", "Untextured\\Bishop Piece", GetStartPos("c1"), 1, Piece.PieceType.Bishop));
            _pieces.Add(new Piece("Bishop2", "Untextured\\Bishop Piece", GetStartPos("f1"), 1, Piece.PieceType.Bishop));
            #endregion

            #region Black Piece Init
            //White Pawn Set
            _pieces.Add(new Piece("Pawn1", "Black Pawn", GetStartPos("a7"), 0, Piece.PieceType.Pawn));
            _pieces.Add(new Piece("Pawn2", "Black Pawn", GetStartPos("b7"), 0, Piece.PieceType.Pawn));
            _pieces.Add(new Piece("Pawn3", "Black Pawn", GetStartPos("c7"), 0, Piece.PieceType.Pawn));
            _pieces.Add(new Piece("Pawn4", "Black Pawn", GetStartPos("d7"), 0, Piece.PieceType.Pawn));
            _pieces.Add(new Piece("Pawn5", "Black Pawn", GetStartPos("e7"), 0, Piece.PieceType.Pawn));
            _pieces.Add(new Piece("Pawn6", "Black Pawn", GetStartPos("f7"), 0, Piece.PieceType.Pawn));
            _pieces.Add(new Piece("Pawn7", "Black Pawn", GetStartPos("g7"), 0, Piece.PieceType.Pawn));
            _pieces.Add(new Piece("Pawn8", "Black Pawn", GetStartPos("h7"), 0, Piece.PieceType.Pawn));


            _pieces.Add(new Piece("Rook1", "Black Rook", GetStartPos("a8"), 0, Piece.PieceType.Rook));
            _pieces.Add(new Piece("Rook2", "Black Rook", GetStartPos("h8"), 0, Piece.PieceType.Rook));

            _pieces.Add(new Piece("King1", "Black King", GetStartPos("e8"), 0, Piece.PieceType.King));

            _pieces.Add(new Piece("Queen1", "Black Queen", GetStartPos("d8"), 0, Piece.PieceType.Queen));

            _pieces.Add(new Piece("Knight1", "Black Knight", GetStartPos("b8"), 0, Piece.PieceType.Knight));
            _pieces.Add(new Piece("Knight1", "Black Knight", GetStartPos("g8"), 0, Piece.PieceType.Knight));

            _pieces.Add(new Piece("Bishop1", "Untextured\\Bishop Piece", GetStartPos("c8"), 0, Piece.PieceType.Bishop));
            _pieces.Add(new Piece("Bishop2", "Untextured\\Bishop Piece", GetStartPos("f8"), 0, Piece.PieceType.Bishop));
            #endregion

            //add Objects
            foreach (Square s in Squares)
                AddObject(s);

            foreach (Piece p in _pieces)
                AddObject(p);

            Moves = new List<Square>();
            
            base.Initialize();
        }//End of Method

        public override void Update(GameTime gametime)
        {
            if (Moves == null)
            {
                foreach (Square s in Squares)
                    s.IsMoveOption = false;
            }

            if (PieceIsSelected)
                Moves = GenerateMoves(PieceToMove, _goFromSquare);

            if (Moves != null)
            {
                foreach (Square s in Moves)
                    s.IsMoveOption = true;
            }

            //when _currentSquare != null, a selection has occurred and we need to process

            if (!PieceIsSelected && _currentSquare != null) //havent selected a Piece to move yet
            {
                    //PieceToMove = GetPiece(_currentSquare.World * Matrix.CreateTranslation(new Vector3(0,2,0)));

                PieceToMove = GetPiece(_currentSquare.World.Translation + new Vector3(0, 2, 0));


                    //make sure PieceTomove is not null, and have selected own colour
                    if (PieceToMove != null && ((int)PieceToMove.ColorType) == (int)Turn)
                    {
                        PieceIsSelected = true;
                        _goFromSquare = _currentSquare;
                    }
                    else
                        PieceToMove = null;
                
                _currentSquare = null;
            }

            if (PieceIsSelected && !SquareIsSelected && _currentSquare != null)  //A piece has been selected, no destination selected yet
            {
                if (Moves != null && Moves.Contains(_currentSquare)) //if highlighted square was selected 
                {
                    //PieceToCapture = GetPiece(_currentSquare.World * Matrix.CreateTranslation(new Vector3(0,2,0)));
                    PieceToCapture = GetPiece(_currentSquare.World.Translation + new Vector3(0, 2, 0));

                    if (PieceToCapture != null)
                    {
                        IsFight = true;
                    }

                    SquareIsSelected = true;
                    _goToSquare = _currentSquare;
                    Moves = null;
                }

                else //user selected a square outside Moves array
                {
                    Piece p = GetPiece(_currentSquare.World.Translation + new Vector3(0,2,0));

                    if (p != null && ((int)PieceToMove.ColorType) == (int)Turn) //Replace selection with this piece
                    {
                        SquareIsSelected = true;
                        PieceToMove = p;
                        _goFromSquare = _currentSquare;
                    }
                    else //reset
                    {
                        PieceIsSelected = false;
                        PieceToMove = null;
                        _goFromSquare = null;
                    }

                    Moves = null;
                }
                _currentSquare = null;
           }

            
            if (PieceIsSelected && SquareIsSelected) //Piece is Selected, destination is selected, Need to process Move
            {
                //check for pawn queening, piece pinned

                //play animations

                //make move
                _currentSquare = null;
                PieceIsSelected = false;
                SquareIsSelected = false;

                if(IsFight)
                    PieceToCapture.Destroy();
                MovePiece(PieceToMove, _goFromSquare, _goToSquare);

                PieceToMove = null;
                PieceToCapture = null;
                IsFight = false;
                Moves = null;
            }

            base.Update(gametime);
        }//End of Method

        protected override void HandleInput()
        {

            #region Camera Controls
            if (InputEngine.IsKeyHeld(Keys.A))
            {
                _camera.MoveCamera();
                //_camera.World *= Matrix.CreateTranslation(-0.1f, 0, 0);
                
            }

            if (InputEngine.IsKeyHeld(Keys.D))
            {
                _camera.World *= Matrix.CreateTranslation(0.1f, 0, 0);
                
            }

            if (InputEngine.IsKeyHeld(Keys.W))
            {
                _camera.World *= Matrix.CreateTranslation(0, 0.1f, 0);

            }

            if (InputEngine.IsKeyHeld(Keys.S))
            {
                _camera.World *= Matrix.CreateTranslation(0, -0.1f, 0);

            }

            if (InputEngine.IsKeyPressed(Keys.D1))
            {
                Engine.Cameras.SetActiveCamera("camTurn");
            }

            if (InputEngine.IsKeyPressed(Keys.D2))
            {
                Engine.Cameras.SetActiveCamera("cam0");
            }

            if (InputEngine.IsKeyPressed(Keys.N))
            {
                MediaPlayer.Pause();
            }

            if (InputEngine.IsKeyPressed(Keys.M))
            {
                MediaPlayer.Resume();
            }


            #endregion


            #region hover move/selection complete

            Squares[_currentI, _currentJ].IsHover = false;

            if (InputEngine.IsKeyPressed(Keys.Right))
            {
                _currentI++;

                if (_currentI > 7)
                    _currentI = 0;
            }

            if (InputEngine.IsKeyPressed(Keys.Left))
            {
                _currentI--;

                if (_currentI < 0)
                    _currentI = 7;
            }

            if (InputEngine.IsKeyPressed(Keys.Up))
            {
                _currentJ++;

                if (_currentJ > 7)
                    _currentJ = 0;
            }

            if (InputEngine.IsKeyPressed(Keys.Down))
            {
                _currentJ--;

                if (_currentJ < 0)
                    _currentJ = 7;
            }
            Squares[_currentI, _currentJ].IsHover = true;

            if (InputEngine.IsKeyPressed(Keys.Enter))
            {
                if (_previousSquare != null)
                    _previousSquare.IsSelected = false;

                _currentSquare = Squares[_currentI, _currentJ];

                if (!PieceIsSelected) //if a piece hasnt been selected, highlight
                {
                    _currentSquare.IsSelected = true;
                }
                else                    //if piece already selected, no highlight, remove highlights
                    _previousSquare.IsSelected = false;


                _previousSquare = _currentSquare;
            }
            #endregion

            base.HandleInput();
        }//End of Method

        private Piece GetPiece(Vector3 pos)
        {
            for (int i = 0; i < _pieces.Count; i++)
                if (_pieces[i].World.Translation == pos)
                    return _pieces[i];

            return null;
        }

        private Vector3 GetStartPos(string pos)
        {
            char[] c = pos.ToCharArray();

            int file = c[0] - 97;
            int rank = c[1] - 49;

            return Squares[file, rank].World.Translation + new Vector3(0,2,0);
        }

        private Vector3 GetNewPos(Square destination)
        {
            return destination.World.Translation + new Vector3(0, 2, 0);
        }

        private Square getSquareFromBB(ulong bb)
        {
            var v = BitboardHelper.getSquareFromBitboard(bb);

            return Squares[v.Item1, v.Item2];
        }

        private List<Square> getSquareListFromBB(ulong bb)
        {
            List<Square> s = new List<Square>();
            var sList = BitboardHelper.getSquareListFromBB(bb);

            if (sList != null)
            {
                foreach (Tuple<int, int> t in sList)
                {
                    s.Add(Squares[t.Item2, t.Item1]);
                }

                return s;
            }
            return null;
        }

        private List<Square> GenerateMoves(Piece p, Square s)
        {
            //Call method based on Type of Piece passed in
            switch (p.Piece_Type)
            {
                case Piece.PieceType.King:
                    return GenerateKingMoves(s, p.ColorType);

                case Piece.PieceType.Pawn:
                    return GeneratePawnMoves(s, p.ColorType);

                case Piece.PieceType.Knight:
                    return GenerateKnightMoves(s, p.ColorType);

                case Piece.PieceType.Bishop:
                    return GenerateBishopMoves(s, p.ColorType);

                case Piece.PieceType.Rook:
                    return GenerateRookMoves(s, p.ColorType);

                case Piece.PieceType.Queen:
                    return GenerateQueenMoves(s, p.ColorType);

                default:
                    return null;
            }
        }

        private List<Square> GenerateKingMoves(Square s, Piece.Color c)
        {
            List<Square> moves = new List<Square>();
            UInt64 myPieceBB = BitboardHelper.getBitboardFromSquare(s);
            
            UInt64 myPieceBB_H_Clip = (myPieceBB & BitboardHelper.ClearFile[7]);
            UInt64 myPieceBB_A_Clip = (myPieceBB & BitboardHelper.ClearFile[0]);

            UInt64 validMovesBB = (myPieceBB_A_Clip << 7) | (myPieceBB << 8) | (myPieceBB_H_Clip << 9) | (myPieceBB_H_Clip << 1) | (myPieceBB_H_Clip >> 7) | (myPieceBB >> 8) | (myPieceBB_A_Clip >> 9) | (myPieceBB_A_Clip >> 1);

            
            if (c == Piece.Color.White)
                validMovesBB = validMovesBB ^( validMovesBB & WhitePieces);
            else
                validMovesBB = validMovesBB ^(validMovesBB & BlackPieces);

            moves = getSquareListFromBB(validMovesBB);
            return moves;
        }

        private List<Square> GeneratePawnMoves(Square s, Piece.Color c)
        {
            List<Square> movesList = new List<Square>();
            UInt64 validMovesBB;
            UInt64 myPieceBB = BitboardHelper.getBitboardFromSquare(s); //bitboard representation of the pawns position

            if (c == Piece.Color.White)
            {
                validMovesBB = (myPieceBB << 7 | myPieceBB << 9) & BlackPieces;

                if (((myPieceBB << 8) & AllPieces) == 0)
                    validMovesBB = validMovesBB | (myPieceBB << 8);

                if (((myPieceBB & BitboardHelper.MaskRank[(int)Rank.two]) != 0) && ((myPieceBB << 16) & AllPieces) == 0)
                    validMovesBB = validMovesBB | (myPieceBB << 16);
            }
            else //for black
            {
                validMovesBB = (myPieceBB >> 7 | myPieceBB >> 9) & WhitePieces;

                if (((myPieceBB << 8) & AllPieces) == 0)
                    validMovesBB |= myPieceBB >> 8;

                if (((myPieceBB & BitboardHelper.MaskRank[(int)Rank.seven]) != 0) && ((myPieceBB >> 16) & AllPieces) == 0)
                    validMovesBB |= myPieceBB >> 16;
            }

            movesList = getSquareListFromBB(validMovesBB);
            return movesList;
        }

        private List<Square> GenerateKnightMoves(Square s, Piece.Color c)
        {
            UInt64 validMovesBB;
            int i = BitboardHelper.getIndexFromSquare(s);

            if (c == Piece.Color.Black)
            {
                validMovesBB = BitboardHelper.KnightAttacks[i] ^ (BitboardHelper.KnightAttacks[i]) & BlackPieces;
            }
            else //for white
            {
                validMovesBB = BitboardHelper.KnightAttacks[i] ^ (BitboardHelper.KnightAttacks[i]) & WhitePieces;
            }

            return getSquareListFromBB(validMovesBB);
        }

        private List<Square> GenerateBishopMoves(Square s, Piece.Color c)
        {
            List<Square> moves = new List<Square>();

            //write

            return moves;
        }

        private List<Square> GenerateRookMoves(Square s, Piece.Color c) // iNCOMPLETE
        {
            UInt64 validSquares;
            List<Square> moves = new List<Square>();

            UInt64 bbBlockers = AllPieces & BitboardHelper.occupancyMaskRook[BitboardHelper.getIndexFromSquare(s)];

            //DATABASE INDEX VALUE ISN'T RIGHT HERE
            int databaseIndex = (int)((bbBlockers * BitboardHelper.magicNumberRook[BitboardHelper.getIndexFromSquare(s)]) >> BitboardHelper.magicNumberShiftsRook[BitboardHelper.getIndexFromSquare(s)]);

            if(c == Piece.Color.White)
                validSquares = BitboardHelper.magicMovesRook[BitboardHelper.getIndexFromSquare(s)][databaseIndex] &  ~WhitePieces;
            else
                validSquares = BitboardHelper.magicMovesRook[BitboardHelper.getIndexFromSquare(s)][databaseIndex] & ~BlackPieces;

            //write

            return moves;
        }

        private List<Square> GenerateQueenMoves(Square s, Piece.Color c)
        {
            List<Square> moves = new List<Square>();

            //write

            return moves;
        }

        private void MovePiece(Piece piece, Square from, Square to)
        {
            UInt64 bbFrom = BitboardHelper.getBitboardFromSquare(from);
            UInt64 bbTo = BitboardHelper.getBitboardFromSquare(to);

            UpdateRelevantbb(piece.Piece_Type, piece.ColorType, bbFrom, bbTo); //update bitboards with new piece position

            piece.UpdateWorld(GetNewPos(to)); //update world position of model
     
        }

        private void UpdateRelevantbb(Piece.PieceType type, Piece.Color c, ulong bbFrom, ulong bbTo)
        {
            if (c == Piece.Color.White)
            {
                switch (type)
                {
                    case Piece.PieceType.Bishop:
                         white_bishops ^= bbFrom;
                         white_bishops ^= bbTo;
                         break;
                    case Piece.PieceType.King:
                         white_kings ^= bbFrom;
                         white_kings ^= bbTo;
                         break;
                    case Piece.PieceType.Knight:
                         white_knights ^= bbFrom;
                         white_knights ^= bbTo;
                         break;
                    case Piece.PieceType.Queen:
                         white_queens ^= bbFrom;
                         white_queens ^= bbTo;
                         break;
                    case Piece.PieceType.Rook:
                         white_rooks ^= bbFrom;
                         white_rooks ^= bbTo;
                         break;
                    case Piece.PieceType.Pawn:
                         white_pawns ^= bbFrom;
                         white_pawns ^= bbTo;
                         break;
                    default:
                         break;
                }
            }
            else
            {
                switch (type)
                {
                    case Piece.PieceType.Bishop:
                        black_bishops ^= bbFrom;
                        black_bishops ^= bbTo;
                         break;
                    case Piece.PieceType.King:
                        black_kings ^= bbFrom;
                        black_kings ^= bbTo;
                         break;
                    case Piece.PieceType.Knight:
                        black_knights ^= bbFrom;
                        black_knights ^= bbTo;
                         break;
                    case Piece.PieceType.Queen:
                        black_queens ^= bbFrom;
                        black_queens ^= bbTo;
                         break;
                    case Piece.PieceType.Rook:
                        black_rooks ^= bbFrom;
                        black_rooks ^= bbTo;
                         break;
                    case Piece.PieceType.Pawn:
                         black_pawns ^= bbFrom;
                         black_pawns ^= bbTo;
                         break;
                    default:
                        break;
                }
            }
        }

    }//End of Class
}//End of Namespace
