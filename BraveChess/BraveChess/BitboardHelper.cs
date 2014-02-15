using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BraveChess.Objects;

namespace BraveChess
{
    class BitboardHelper
    {
        #region Look up arrays

        static public UInt64[] KnightAttacks = new UInt64[] {0x20400LU, 0x50800LU, 0xA1100LU, 0x142200LU, 284400LU, 0x508800, 0xA01000LU, 0x402000,
                                                            0x2040004LU, 0x5080008LU, 0xA110011LU, 0x14220022LU, 0x28440044LU, 0x50880088LU, 0xA0100010LU, 0x40200020LU,
                                                            0x204000402LU, 0x508000805LU, 0xA1100110ALU, 0x1422002214LU, 0x2844004428LU, 0x5088008850LU, 0xA0100010A0LU, 0x4020002040LU,
                                                            0x20400040200LU, 0x50800080500LU, 0xA1100110A00LU, 0x142200221400LU, 0x284400442800LU, 0x508800885000LU, 0xA0100010A000LU, 0x402000204000LU,
                                                            0x2040004020000LU, 0x5080008050000LU, 0xA1100110A0000LU, 0x14220022140000LU, 0x28440044280000LU, 0x50880088500000LU, 0xA0100010A00000LU, 0x40200020400000LU,
                                                            0x204000402000000LU, 0x508000805000000LU, 0xA1100110A000000LU, 0x1422002214000000LU, 0x2844004428000000LU, 0x5088008850000000LU, 0xA0100010A0000000LU, 0x4020002040000000LU,
                                                            0x400040200000000LU, 0x80008050000LU, 0x1100110A00000000LU, 0x2200221400000000LU, 0x4400442800000000LU, 0x8800885000000000LU, 0x100010A000000000LU, 0x2000204000000000LU,
                                                            0x4020000000000LU, 0x8050000000000LU, 0x110A0000000000LU, 0x22140000000000, 0x44280000000000LU, 0x88500000000000LU, 0x10A00000000000, 0x20400000000000LU};

        static public UInt64[] MaskRank = new UInt64[] { 0xffLU, 0xff00LU, 0xff0000LU, 0xff000000LU, 0xff00000000LU, 0xff0000000000LU, 0xff000000000000LU, 0xff00000000000000LU};

        static public UInt64[] ClearRank = new UInt64[] { 0xffffffffffffff00LU, 0xffffffffffff00ffLU, 0xffffffffff00ffffLU, 0xffffffff00ffffffLU, 0xffffff00ffffffffLU, 0xffff00ffffffffffLU, 0xff00ffffffffffffLU, 0xffffffffffffffLU };

        static public UInt64[] ClearFile = new UInt64[] { 0xFEFEFEFEFEFEFEFELU, 0xFDFDFDFDFDFDFDFDLU, 0xFBFBFBFBFBFBFBFBLU, 0xF7F7F7F7F7F7F7F7LU, 
            0x7F7F7F7F7F7F7F7FLU, 0xBFBFBFBFBFBFBFBFLU, 0xDFDFDFDFDFDFDFDFLU, 0x7F7F7F7F7F7F7F7FU }; 
        
        static public UInt64[] MaskFile = new UInt64[] {0x101010101010101LU, 0x202020202020202LU, 0x404040404040404LU, 0x808080808080808LU, 
            0x1010101010101010LU, 0x2020202020202020LU, 0x4040404040404040LU, 0x8080808080808080LU};

#endregion

        static public int getIndexFromSquare(Square s)
        {
            int index = 8 * (int)s.rank + (int)s.file;
            return index;
        }

        static public Tuple<int, int> getSquareFromIndex(uint index)
        {
            if (index >= 64)
            {
                throw new System.Exception();
            }
            int i = (int)(index / 8);
            int j = (int)(index % 8);

            return new Tuple<int,int>(i, j);
        }

        static public ulong getBitboardFromSquare(Square s)
        {
            return ((ulong)1) << (int)getIndexFromSquare(s);
        }

        static public Tuple<int, int> getSquareFromBitboard(ulong bb)
        {
            uint index = bitScanForward(bb);
            var s = getSquareFromIndex(index);
            return s;
        }

        static public uint bitScanForward(ulong bb)
        {
            uint[] debruijn64Array = new uint[64]
            {63, 0, 58, 1, 59, 47, 53, 2,
                60, 39, 48, 27, 54, 33, 42, 3,
                61, 51, 37, 40, 49, 18, 28, 20,
                55, 30, 34, 11, 43, 14, 22, 4,
                62, 57, 46, 52, 38, 26, 32, 41,
                50, 36, 17, 19, 29, 10, 13, 21,
                56, 45, 25, 31, 35, 16, 9, 12,
                44, 24, 15, 8, 23, 7, 6, 5};

            const long debruijn64 = (long)(0x07EDD5E59A4E28C2);

            return debruijn64Array[(ulong)(getBitboardLSB(bb) * debruijn64) >> 58];
        }

        static public ulong getBitboardLSB(ulong bb)
        {
            return ((ulong)((long)bb & -(long)bb));
        }

        static public List<Tuple<int, int>> getSquareListFromBB(ulong bb)
        {
            List<Tuple<int, int>> sList = new List<Tuple<int, int>>();

            if (bb != 0)
            {
                do
                {
                    uint index = bitScanForward(bb);
                    var s = getSquareFromIndex(index);
                    sList.Add(s);
                }
                while ((bb &= bb - 1) != 0);

                return sList;
            }
            return null;
        }

    }
}

    