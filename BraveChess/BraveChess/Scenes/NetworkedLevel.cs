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
using Microsoft.Xna.Framework.Net;

namespace BraveChess.Scenes
{
    class NetworkedLevel : Scene
    {
        public NetworkedLevel(GameEngine _engine)
            : base("NetworkLevel", _engine) { }

        public override void Initialize()
        {            

            #region Sound
            //loading songs//efects
            Engine.Audio.LoadSong("BackgroundSong");
            Engine.Audio.PlaySong("BackgroundSong");
            MediaPlayer.IsRepeating = true;
            Engine.Audio.LoadEffect("move");
            #endregion
            
            base.Initialize(); 

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

            _pieces.Add(new Piece("Bishop1", "Untextured\\Bishop Piece", GetStartPos("c1"), 1, Piece.PieceType.Bishop));
            _pieces.Add(new Piece("Bishop2", "Untextured\\Bishop Piece", GetStartPos("f1"), 1, Piece.PieceType.Bishop));
            #endregion

            #region Black Piece Init
            //White Pawn Set
            _pieces.Add(new Piece("pawn1", "Black Pawn", GetStartPos("a7"), 0, Piece.PieceType.Pawn));
            _pieces.Add(new Piece("pawn2", "Black Pawn", GetStartPos("b7"), 0, Piece.PieceType.Pawn));
            _pieces.Add(new Piece("pawn3", "Black Pawn", GetStartPos("c7"), 0, Piece.PieceType.Pawn));
            _pieces.Add(new Piece("pawn4", "Black Pawn", GetStartPos("d7"), 0, Piece.PieceType.Pawn));
            _pieces.Add(new Piece("pawn5", "Black Pawn", GetStartPos("e7"), 0, Piece.PieceType.Pawn));
            _pieces.Add(new Piece("pawn6", "Black Pawn", GetStartPos("f7"), 0, Piece.PieceType.Pawn));
            _pieces.Add(new Piece("pawn7", "Black Pawn", GetStartPos("g7"), 0, Piece.PieceType.Pawn));
            _pieces.Add(new Piece("pawn8", "Black Pawn", GetStartPos("h7"), 0, Piece.PieceType.Pawn));


            _pieces.Add(new Piece("rook1", "Black Rook", GetStartPos("a8"), 0, Piece.PieceType.Rook));
            _pieces.Add(new Piece("rook2", "Black Rook", GetStartPos("h8"), 0, Piece.PieceType.Rook));

            _pieces.Add(new Piece("king1", "Black King", GetStartPos("e8"), 0, Piece.PieceType.King));

            _pieces.Add(new Piece("queen1", "Black Queen", GetStartPos("d8"), 0, Piece.PieceType.Queen));

            _pieces.Add(new Piece("knight1", "Black Knight", GetStartPos("b8"), 0, Piece.PieceType.Knight));
            _pieces.Add(new Piece("knight1", "Black Knight", GetStartPos("g8"), 0, Piece.PieceType.Knight));

            _pieces.Add(new Piece("bishop1", "Untextured\\Bishop Piece", GetStartPos("c8"), 0, Piece.PieceType.Bishop));
            _pieces.Add(new Piece("bishop2", "Untextured\\Bishop Piece", GetStartPos("f8"), 0, Piece.PieceType.Bishop));
            #endregion

            //add Objects
            foreach (Square s in Squares)
                AddObject(s);

            foreach (Piece p in _pieces)
                AddObject(p);

            if (Engine.Network.networkSession.IsHost)
                Engine.Cameras.SetActiveCamera("camWhite");
            else
                Engine.Cameras.SetActiveCamera("camBlack");

            MovesAvailable = new List<Square>();         
            
        }//End of Method

        private void CheckIncomingPackets(GameTime gametime)
        {
            if (Engine.Network.networkSession != null ) 
            {
                MessageType msg = Engine.Network.ProcessIncomingData(gametime);

                if (msg == MessageType.UpdateOtherMove)
                {
                    ReadMovePacket(); // reads incoming packet and process 
                    SwitchTurn(true); // changes Turn
                }

            }
        } //end method

        public override void Update(GameTime gametime)
        {
            CheckIncomingPackets(gametime);

            //Check LocalGamers .Tag property, if true, then it is their move and we must accept Input
            if((bool)Engine.Network.networkSession.LocalGamers[0].Tag == true)
                HandleInput();

            UpdateSelection();
            
            base.Update(gametime);
        }//End of Method

        private void UpdateSelection()
        {
            switch (SelectState)
            {
                case SelectionState.SelectPiece:
                    if (_currentSquare != null)
                    {
                        PieceToMove = GetPiece(_currentSquare.World.Translation + new Vector3(0, 2, 0));

                        if (PieceToMove != null && ((int)PieceToMove.ColorType) == (int)Turn)
                        {
                            _goFromSquare = _currentSquare;
                            SelectState = SelectionState.PieceSelected;
                        }

                        _currentSquare = null;
                    }
                    break;

                case SelectionState.PieceSelected:
                    MovesAvailable = GenerateMoves(PieceToMove, _goFromSquare);
                    if (MovesAvailable != null)
                    {
                        foreach (Square s in MovesAvailable)
                            s.IsMoveOption = true;
                        SelectState = SelectionState.SelectMove;
                    }
                    else
                    {
                        ResetMoves();
                        SelectState = SelectionState.SelectPiece;
                    }
                    break;

                case SelectionState.SelectMove:
                    if (_currentSquare != null && MovesAvailable.Contains(_currentSquare))
                    {
                        PieceToCapture = GetPiece(_currentSquare.World.Translation + new Vector3(0, 2, 0));

                        if (PieceToCapture != null)
                            IsFight = true;

                        _goToSquare = _currentSquare;

                        SelectState = SelectionState.MoveSelected;
                    }
                    else if (_currentSquare != null)
                    {
                        Piece p = GetPiece(_currentSquare.World.Translation + new Vector3(0, 2, 0));

                        if (p != null && ((int)PieceToMove.ColorType) == (int)Turn) //Replace selection with this piece
                        {
                            PieceToMove = p;
                            _goFromSquare = _currentSquare;
                            SelectState = SelectionState.PieceSelected;
                        }
                        else
                            SelectState = SelectionState.SelectPiece;
                        ResetMoves();
                    }

                    _currentSquare = null;
                    break;

                case SelectionState.MoveSelected:
                    ResetMoves();

                    //check for pawn queening

                    //Make sure the move doesnt leave king in check(test the move)
                    if (TestMove(PieceToMove, _goFromSquare, _goToSquare))
                    {
                        if (IsFight)
                            PieceToCapture.Destroy(); //Remove the captured piece, if any

                        MovePiece(PieceToMove, _goFromSquare, _goToSquare); //make the move

                        SelectState = SelectionState.SelectPiece; //reset
                        IsFight = false;

                        SwitchTurn(false); //Changes turnState 
                    }
                    //else> Write message "sorry that would leave you in check"

                    break;

            } // end switch
        }

        public void ReadMovePacket()
        {
            Vector3 pos = Engine.Network.packetReader.ReadVector3();
            int pieceType = Engine.Network.packetReader.ReadInt32();
            int pieceColor = Engine.Network.packetReader.ReadInt32();
            UInt64 fromSq = Engine.Network.packetReader.ReadUInt64();
            UInt64 toSq = Engine.Network.packetReader.ReadUInt64();

            MoveOtherPiece(pos, (Piece.PieceType)pieceType, (Piece.Color)pieceColor, fromSq, toSq);
        }

        private void MovePiece(Piece piece, Square from, Square to)
        {
            UInt64 bbFrom = BitboardHelper.getBitboardFromSquare(from);
            UInt64 bbTo = BitboardHelper.getBitboardFromSquare(to);

            UpdateRelevantbb(piece.Piece_Type, piece.ColorType, bbFrom, bbTo); //update bitboards with new piece position

            Engine.Network.WriteMovePacket(piece.World.Translation, (int)piece.Piece_Type, (int)piece.ColorType, bbFrom, bbTo);

            piece.UpdateWorld(GetNewPos(to)); //update world position of model

        }

        private bool TestMove(Piece piece, Square from, Square to)
        {
            UInt64 bbFrom = BitboardHelper.getBitboardFromSquare(from);
            UInt64 bbTo = BitboardHelper.getBitboardFromSquare(to);

            UpdateRelevantbb(piece.Piece_Type, piece.ColorType, bbFrom, bbTo);

            if (TestForCheck(piece.ColorType))
            {
                UpdateRelevantbb(piece.Piece_Type, piece.ColorType, bbTo, bbFrom);
                return false;
            }

            return true;
        } //sorry that would leave you in check

        private void MoveOtherPiece(Vector3 pos, Piece.PieceType type, Piece.Color color, UInt64 bbFrom, UInt64 bbTo)
        {
            UpdateRelevantbb(type, color, bbFrom, bbTo); //update bitboards with new piece position

            Square s = getSquareFromBB(bbTo);
            Vector3 newPos = GetNewPos(s);

            Piece capturedPiece = GetPiece(s.World.Translation + new Vector3(0, 2, 0));
            if (capturedPiece != null)
                capturedPiece.Destroy();
            
            GetPiece(pos).UpdateWorld(newPos);
        }

        private void SwitchTurn(bool recieved)
        {
            Turn = Turn == TurnState.White ? TurnState.Black : TurnState.White;

            if(!recieved)
                Engine.Network.TurnSwitch();
        }

        protected override void HandleInput()
        {
            base.HandleInput();
        }

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

            return Squares[v.Item2, v.Item1];
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
                    return getSquareListFromBB(GenerateKingMoves(s, p.ColorType));

                case Piece.PieceType.Pawn:
                    return getSquareListFromBB(GeneratePawnMoves(s, p.ColorType));

                case Piece.PieceType.Knight:
                    return getSquareListFromBB(GenerateKnightMoves(s, p.ColorType));

                case Piece.PieceType.Bishop:
                    return getSquareListFromBB(GenerateBishopMoves(s, p.ColorType));

                case Piece.PieceType.Rook:
                    return getSquareListFromBB(GenerateRookMoves(s, p.ColorType));

                case Piece.PieceType.Queen:
                    return getSquareListFromBB(GenerateQueenMoves(s, p.ColorType));
                default:
                    return null;
            }
        }

        private UInt64 GenerateKingMoves(Square s, Piece.Color c)
        {
            UInt64 myPieceBB = BitboardHelper.getBitboardFromSquare(s);
            
            UInt64 myPieceBB_H_Clip = (myPieceBB & BitboardHelper.ClearFile[7]);
            UInt64 myPieceBB_A_Clip = (myPieceBB & BitboardHelper.ClearFile[0]);

            UInt64 validMovesBB = (myPieceBB_A_Clip << 7) | (myPieceBB << 8) | (myPieceBB_H_Clip << 9) | (myPieceBB_H_Clip << 1) | (myPieceBB_H_Clip >> 7) | (myPieceBB >> 8) | (myPieceBB_A_Clip >> 9) | (myPieceBB_A_Clip >> 1);

            
            if (c == Piece.Color.White)
                validMovesBB = validMovesBB ^( validMovesBB & WhitePieces);
            else
                validMovesBB = validMovesBB ^(validMovesBB & BlackPieces);

            return validMovesBB;
        }

        private UInt64 GeneratePawnMoves(Square s, Piece.Color c)
        {
            UInt64 validMovesBB;
            UInt64 myPieceBB = BitboardHelper.getBitboardFromSquare(s); //bitboard representation of the pawns position

            if (c == Piece.Color.White)
            {
                validMovesBB = (myPieceBB << 7 | myPieceBB << 9) & BlackPieces;

                if (((myPieceBB << 8) & AllPieces) == 0)
                {
                    validMovesBB = validMovesBB | (myPieceBB << 8);

                    if (((myPieceBB & BitboardHelper.MaskRank[(int)Rank.two]) != 0) && ((myPieceBB << 16) & AllPieces) == 0)
                        validMovesBB = validMovesBB | (myPieceBB << 16);
                }
            }
            else
            {
                validMovesBB = (myPieceBB >> 7 | myPieceBB >> 9) & WhitePieces;

                if (((myPieceBB >> 8) & AllPieces) == 0)
                {
                    validMovesBB = validMovesBB | (myPieceBB >> 8);

                    if (((myPieceBB & BitboardHelper.MaskRank[(int)Rank.seven]) != 0) && ((myPieceBB >> 16) & AllPieces) == 0)
                        validMovesBB = validMovesBB | myPieceBB >> 16;
                }
            }

            return validMovesBB;
        }

        private UInt64 GenerateKnightMoves(Square s, Piece.Color c)
        {
            UInt64 validMovesBB;
            int sqIndex = BitboardHelper.getIndexFromSquare(s);

            if (c == Piece.Color.White)
                validMovesBB = BitboardHelper.KnightAttacks[sqIndex] ^ (BitboardHelper.KnightAttacks[sqIndex]) & WhitePieces;

            else if (c == Piece.Color.Black)
                validMovesBB = BitboardHelper.KnightAttacks[sqIndex] ^ (BitboardHelper.KnightAttacks[sqIndex]) & BlackPieces;

            else
                validMovesBB = BitboardHelper.KnightAttacks[sqIndex];

            return validMovesBB;
        }

        private UInt64 GenerateBishopMoves(Square s, Piece.Color c)
        {
            UInt64 validSquares;
            int sqIndex = BitboardHelper.getIndexFromSquare(s);

            UInt64 bbBlockers = AllPieces & BitboardHelper.occupancyMaskBishop[sqIndex];
            
            int databaseIndex = (int)((bbBlockers * BitboardHelper.magicNumberBishop[sqIndex]) >> BitboardHelper.magicNumberShiftsBishop[sqIndex]);

            if (c == Piece.Color.White)
                validSquares = BitboardHelper.magicMovesBishop[sqIndex][databaseIndex] & ~WhitePieces;
            else if(c == Piece.Color.Black)
                validSquares = BitboardHelper.magicMovesBishop[sqIndex][databaseIndex] & ~BlackPieces;
            else
                validSquares = BitboardHelper.magicMovesBishop[sqIndex][databaseIndex];

            return validSquares;
        }

        private UInt64 GenerateRookMoves(Square s, Piece.Color c) 
        {
            UInt64 validSquares;
            int sqIndex = BitboardHelper.getIndexFromSquare(s);

            UInt64 bbBlockers = AllPieces & BitboardHelper.occupancyMaskRook[sqIndex];

            int databaseIndex = (int)((bbBlockers * BitboardHelper.magicNumberRook[sqIndex]) >> BitboardHelper.magicNumberShiftsRook[sqIndex]);

            if(c == Piece.Color.White)
                validSquares = BitboardHelper.magicMovesRook[sqIndex][databaseIndex] &  ~WhitePieces;
            else if(c == Piece.Color.Black)
                validSquares = BitboardHelper.magicMovesRook[sqIndex][databaseIndex] & ~BlackPieces;
            else
                validSquares = BitboardHelper.magicMovesRook[sqIndex][databaseIndex];
            
            return validSquares;
        }

        private UInt64 GenerateQueenMoves(Square s, Piece.Color c)
        {
            UInt64 validSquares;

            //first calulate Rook movements for queen
            validSquares = GenerateRookMoves(s, c);

            //then calculate Bishop moves for queen and OR with rook movements
            validSquares |= GenerateBishopMoves(s, c);
           
            return validSquares;
        }

        private UInt64 FindAttacksToSquare(Square s) // returns bitboard with all pieces attacking the specified Square
        {
            UInt64 attackersBB;
            int sqIndex = BitboardHelper.getIndexFromSquare(s);

            attackersBB = (BitboardHelper.KnightAttacks[sqIndex] & white_knights & black_knights);
            attackersBB |= (GenerateBishopMoves(s, Piece.Color.None) & black_bishops & white_bishops & black_queens & white_queens);
            attackersBB |= (GenerateRookMoves(s, Piece.Color.None) & black_rooks & white_rooks & black_queens & white_queens);
            //add pawn and king attacks

            return attackersBB;
        }

        private bool TestForCheck(Piece.Color c)
        {
            Square kingPos;

            if (c == Piece.Color.White)
            {
                kingPos = getSquareFromBB(white_kings);

                //if all pieces attacking the kings position minus pieces of his own colour != 0, then the king is in check
                if ((FindAttacksToSquare(kingPos) & ~WhitePieces) != 0)
                    return true;
            }
            else if (c == Piece.Color.Black)
            {
                kingPos = getSquareFromBB(black_kings);
                if ((FindAttacksToSquare(kingPos) & ~BlackPieces) != 0)
                    return true;
            }
            return false;
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
