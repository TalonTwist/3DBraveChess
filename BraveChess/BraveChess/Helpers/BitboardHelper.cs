using System;
using System.Collections.Generic;
using System.Linq;
using BraveChess.Engines;
using BraveChess.Objects;

namespace BraveChess.Helpers
{
    public class BitboardHelper
    {
        #region Look up arrays

        static public UInt64[] KnightAttacks =
        {0x20400LU, 0x50800LU, 0xA1100LU, 0x142200LU, 284400LU, 0x508800, 0xA01000LU, 0x402000,
            0x2040004LU, 0x5080008LU, 0xA110011LU, 0x14220022LU, 0x28440044LU, 0x50880088LU, 0xA0100010LU, 0x40200020LU,
            0x204000402LU, 0x508000805LU, 0xA1100110ALU, 0x1422002214LU, 0x2844004428LU, 0x5088008850LU, 0xA0100010A0LU, 0x4020002040LU,
            0x20400040200LU, 0x50800080500LU, 0xA1100110A00LU, 0x142200221400LU, 0x284400442800LU, 0x508800885000LU, 0xA0100010A000LU, 0x402000204000LU,
            0x2040004020000LU, 0x5080008050000LU, 0xA1100110A0000LU, 0x14220022140000LU, 0x28440044280000LU, 0x50880088500000LU, 0xA0100010A00000LU, 0x40200020400000LU,
            0x204000402000000LU, 0x508000805000000LU, 0xA1100110A000000LU, 0x1422002214000000LU, 0x2844004428000000LU, 0x5088008850000000LU, 0xA0100010A0000000LU, 0x4020002040000000LU,
            0x400040200000000LU, 0x80008050000LU, 0x1100110A00000000LU, 0x2200221400000000LU, 0x4400442800000000LU, 0x8800885000000000LU, 0x100010A000000000LU, 0x2000204000000000LU,
            0x4020000000000LU, 0x8050000000000LU, 0x110A0000000000LU, 0x22140000000000, 0x44280000000000LU, 0x88500000000000LU, 0x10A00000000000, 0x20400000000000LU};

        public static UInt64[] WhitePawnAttacks =
        {0,0,0,0,0,0,0,0, 0x200LU, 0x500LU, 0xA00LU, 0x1400LU, 0x2800LU, 0x5000LU, 0xA000LU, 0x4000LU,
            0x20000LU, 0x50000LU, 0xA0000LU, 0x140000LU, 0x280000LU, 0x500000LU, 0xA00000LU, 0x400000LU,
            0x2000000LU, 0x5000000LU, 0xA000000LU, 0x14000000LU, 0x28000000LU, 0x50000000LU, 0xA0000000LU, 0x40000000LU, 
            0x200000000LU, 0x500000000LU, 0xA00000000LU, 0x1400000000LU, 0x2800000000LU, 0x5000000000LU, 0xA000000000LU, 0x4000000000LU, 
            0x20000000000LU, 0x50000000000LU, 0xA0000000000LU, 0x140000000000LU, 0x280000000000LU, 0x500000000000LU, 0xA00000000000LU, 0x400000000000LU,
            0x2000000000000LU, 0x5000000000000LU, 0xA000000000000LU, 0x14000000000000LU, 0x28000000000000LU, 0x50000000000000LU, 0xA0000000000000LU, 0x40000000000000LU,
            0,0,0,0,0,0,0,0};

        public static UInt64[] BlackPawnAttacks =
        {0, 0, 0, 0, 0, 0, 0, 0, 0x2LU, 0x5LU, 0xALU, 0x41LU, 0x82LU, 0x50LU, 0xA0LU, 0x40LU, 0x200LU, 0x500LU,0xA00LU, 0x4100LU, 0x8200LU, 0x5000LU, 0xA000LU, 0x4000LU,
            0x20000LU, 0x50000LU, 0xA0000LU, 0x410000LU, 0x820000LU, 0x500000LU, 0xA00000LU, 0x400000LU, 0x2000000LU, 0x5000000LU, 0xA000000LU, 0x41000000LU, 0x82000000LU, 0x50000000LU, 0xA0000000LU, 0x40000000LU,
            0x200000000LU, 0x500000000LU, 0xA00000000LU, 0x4100000000LU, 0x8200000000LU, 0x5000000000LU, 0xA000000000LU, 0x4000000000LU,
            0x20000000000LU, 0x50000000000LU, 0xA0000000000LU, 0x410000000000LU, 0x820000000000LU, 0x500000000000LU, 0xA00000000000LU, 0x400000000000LU, 
            0, 0, 0, 0, 0, 0, 0, 0};

        public static UInt64[] KingAttacks =
        {0x302LU, 0x705LU, 0xE0ALU, 0x1C14LU, 0x3828LU, 0x7050LU, 0xE0A0LU, 0xC040LU, 0x30203LU, 0x70507LU, 0xE0A0ELU, 0x1C141CLU, 0x382838LU, 0x705070LU, 0xE0A0E0LU, 0xC040C0,
            0x3020300LU, 0x7050700LU, 0xE0A0E00LU, 0x1C141C00LU, 0x38283800LU, 0x70507000LU, 0xE0A0E000LU, 0xC040C000LU,
            0x302030000LU, 0x705070000LU, 0xE0A0E0000LU, 0x1C141C0000LU, 0x3828380000LU, 0x7050700000LU, 0xE0A0E00000LU, 0xC040C00000LU,
            0x30203000000LU, 0x70507000000LU, 0xE0A0E000000LU, 0x1C141C000000LU, 0x382838000000LU, 0x705070000000LU, 0xE0A0E0000000LU, 0xC040C0000000LU,
            0x3020300000000LU, 0x7050700000000LU, 0xE0A0E00000000LU, 0x1C141C00000000LU, 0x38283800000000LU, 0x70507000000000LU, 0xE0A0E000000000LU, 0xC040C000000000LU,
            0x302030000000000LU, 0x705070000000000LU, 0xE0A0E0000000000LU, 0x1C141C0000000000LU, 0x3828380000000000LU, 0x7050700000000000LU, 0xE0A0E00000000000LU, 0xC040C00000000000LU,
            0x203000000000000LU, 0x507000000000000LU, 0xA0E000000000000LU, 0x141C000000000000LU, 0x2838000000000000LU, 0x5070000000000000LU, 0xA0E0000000000000LU, 0x40C0000000000000LU};

        public static UInt64 MaskWhiteCastleShort = 0x60LU;
        public static UInt64 MaskWhiteCastleLong = 0xELU;
        public static UInt64 MaskBlackCastleShort = 0x6000000000000000LU;
        public static UInt64 MaskBlackCastleLong = 0xE00000000000000LU;


        static public UInt64[] MaskRank = { 0xffLU, 0xff00LU, 0xff0000LU, 0xff000000LU, 0xff00000000LU, 0xff0000000000LU, 0xff000000000000LU, 0xff00000000000000LU};

        static public UInt64[] ClearRank = { 0xffffffffffffff00LU, 0xffffffffffff00ffLU, 0xffffffffff00ffffLU, 0xffffffff00ffffffLU, 0xffffff00ffffffffLU, 0xffff00ffffffffffLU, 0xff00ffffffffffffLU, 0xffffffffffffffLU };

        static public UInt64[] ClearFile =
        { 0xFEFEFEFEFEFEFEFELU, 0xFDFDFDFDFDFDFDFDLU, 0xFBFBFBFBFBFBFBFBLU, 0xF7F7F7F7F7F7F7F7LU, 
            0x7F7F7F7F7F7F7F7FLU, 0xBFBFBFBFBFBFBFBFLU, 0xDFDFDFDFDFDFDFDFLU, 0x7F7F7F7F7F7F7F7FU }; 
        
        static public UInt64[] MaskFile =
        {0x101010101010101LU, 0x202020202020202LU, 0x404040404040404LU, 0x808080808080808LU, 
            0x1010101010101010LU, 0x2020202020202020LU, 0x4040404040404040LU, 0x8080808080808080LU};

        public static int[] MagicNumberShiftsRook =
        { 52,53,53,53,53,53,53,52,
            53,54,54,54,54,54,54,53,
            53,54,54,54,54,54,54,53,
            53,54,54,54,54,54,54,53,
            53,54,54,54,54,54,54,53,
            53,54,54,54,54,54,54,53,
            53,54,54,54,54,54,54,53,
            52,53,53,53,53,53,53,52 };

        public static ulong[] OccupancyMaskRook =
        {
            0x101010101017eL, 0x202020202027cL, 0x404040404047aL, 0x8080808080876L, 0x1010101010106eL, 0x2020202020205eL, 0x4040404040403eL, 0x8080808080807eL, 0x1010101017e00L, 0x2020202027c00L, 0x4040404047a00L, 0x8080808087600L, 0x10101010106e00L, 0x20202020205e00L, 0x40404040403e00L, 0x80808080807e00L, 0x10101017e0100L, 0x20202027c0200L, 0x40404047a0400L, 0x8080808760800L, 0x101010106e1000L, 0x202020205e2000L, 0x404040403e4000L, 0x808080807e8000L, 0x101017e010100L, 0x202027c020200L, 0x404047a040400L, 0x8080876080800L, 0x1010106e101000L, 0x2020205e202000L, 0x4040403e404000L, 0x8080807e808000L, 0x1017e01010100L, 0x2027c02020200L, 0x4047a04040400L, 0x8087608080800L, 0x10106e10101000L, 0x20205e20202000L, 0x40403e40404000L, 0x80807e80808000L, 0x17e0101010100L, 0x27c0202020200L, 0x47a0404040400L, 0x8760808080800L, 0x106e1010101000L, 0x205e2020202000L, 0x403e4040404000L, 0x807e8080808000L, 0x7e010101010100L, 0x7c020202020200L, 0x7a040404040400L, 0x76080808080800L, 0x6e101010101000L, 0x5e202020202000L, 0x3e404040404000L, 0x7e808080808000L, 0x7e01010101010100L, 0x7c02020202020200L, 0x7a04040404040400L, 0x7608080808080800L, 0x6e10101010101000L, 0x5e20202020202000L, 0x3e40404040404000L, 0x7e80808080808000L 
        };
        public static ulong[] OccupancyMaskBishop =
        {
            0x40201008040200L, 0x402010080400L, 0x4020100a00L, 0x40221400L, 0x2442800L, 0x204085000L, 0x20408102000L, 0x2040810204000L, 0x20100804020000L, 0x40201008040000L, 0x4020100a0000L, 0x4022140000L, 0x244280000L, 0x20408500000L, 0x2040810200000L, 0x4081020400000L, 0x10080402000200L, 0x20100804000400L, 0x4020100a000a00L, 0x402214001400L, 0x24428002800L, 0x2040850005000L, 0x4081020002000L, 0x8102040004000L, 0x8040200020400L, 0x10080400040800L, 0x20100a000a1000L, 0x40221400142200L, 0x2442800284400L, 0x4085000500800L, 0x8102000201000L, 0x10204000402000L, 0x4020002040800L, 0x8040004081000L, 0x100a000a102000L, 0x22140014224000L, 0x44280028440200L, 0x8500050080400L, 0x10200020100800L, 0x20400040201000L, 0x2000204081000L, 0x4000408102000L, 0xa000a10204000L, 0x14001422400000L, 0x28002844020000L, 0x50005008040200L, 0x20002010080400L, 0x40004020100800L, 0x20408102000L, 0x40810204000L, 0xa1020400000L, 0x142240000000L, 0x284402000000L, 0x500804020000L, 0x201008040200L, 0x402010080400L, 0x2040810204000L, 0x4081020400000L, 0xa102040000000L, 0x14224000000000L, 0x28440200000000L, 0x50080402000000L, 0x20100804020000L, 0x40201008040200L     
        };

        public static ulong[] MagicNumberRook =
        {
            0xa180022080400230L, 0x40100040022000L, 0x80088020001002L, 0x80080280841000L, 0x4200042010460008L, 0x4800a0003040080L, 0x400110082041008L, 0x8000a041000880L, 0x10138001a080c010L, 0x804008200480L, 0x10011012000c0L, 0x22004128102200L, 0x200081201200cL, 0x202a001048460004L, 0x81000100420004L, 0x4000800380004500L, 0x208002904001L, 0x90004040026008L, 0x208808010002001L, 0x2002020020704940L, 0x8048010008110005L, 0x6820808004002200L, 0xa80040008023011L, 0xb1460000811044L, 0x4204400080008ea0L, 0xb002400180200184L, 0x2020200080100380L, 0x10080080100080L, 0x2204080080800400L, 0xa40080360080L, 0x2040604002810b1L, 0x8c218600004104L, 0x8180004000402000L, 0x488c402000401001L, 0x4018a00080801004L, 0x1230002105001008L, 0x8904800800800400L, 0x42000c42003810L, 0x8408110400b012L, 0x18086182000401L, 0x2240088020c28000L, 0x1001201040c004L, 0xa02008010420020L, 0x10003009010060L, 0x4008008008014L, 0x80020004008080L, 0x282020001008080L, 0x50000181204a0004L, 0x102042111804200L, 0x40002010004001c0L, 0x19220045508200L, 0x20030010060a900L, 0x8018028040080L, 0x88240002008080L, 0x10301802830400L, 0x332a4081140200L, 0x8080010a601241L, 0x1008010400021L, 0x4082001007241L, 0x211009001200509L, 0x8015001002441801L, 0x801000804000603L, 0xc0900220024a401L, 0x1000200608243L
        };

        public static ulong[] MagicNumberBishop =
        {
            0x2910054208004104L, 0x2100630a7020180L, 0x5822022042000000L, 0x2ca804a100200020L, 0x204042200000900L, 0x2002121024000002L, 0x80404104202000e8L, 0x812a020205010840L, 0x8005181184080048L, 0x1001c20208010101L, 0x1001080204002100L, 0x1810080489021800L, 0x62040420010a00L, 0x5028043004300020L, 0xc0080a4402605002L, 0x8a00a0104220200L, 0x940000410821212L, 0x1808024a280210L, 0x40c0422080a0598L, 0x4228020082004050L, 0x200800400e00100L, 0x20b001230021040L, 0x90a0201900c00L, 0x4940120a0a0108L, 0x20208050a42180L, 0x1004804b280200L, 0x2048020024040010L, 0x102c04004010200L, 0x20408204c002010L, 0x2411100020080c1L, 0x102a008084042100L, 0x941030000a09846L, 0x244100800400200L, 0x4000901010080696L, 0x280404180020L, 0x800042008240100L, 0x220008400088020L, 0x4020182000904c9L, 0x23010400020600L, 0x41040020110302L, 0x412101004020818L, 0x8022080a09404208L, 0x1401210240484800L, 0x22244208010080L, 0x1105040104000210L, 0x2040088800c40081L, 0x8184810252000400L, 0x4004610041002200L, 0x40201a444400810L, 0x4611010802020008L, 0x80000b0401040402L, 0x20004821880a00L, 0x8200002022440100L, 0x9431801010068L, 0x1040c20806108040L, 0x804901403022a40L, 0x2400202602104000L, 0x208520209440204L, 0x40c000022013020L, 0x2000104000420600L, 0x400000260142410L, 0x800633408100500L, 0x2404080a1410L, 0x138200122002900L    
        };
        
        public static int[] MagicNumberShiftsBishop =
        { 58,59,59,59,59,59,59,58,
            59,59,59,59,59,59,59,59,
            59,59,57,57,57,57,59,59,
            59,59,57,55,55,57,59,59,
            59,59,57,55,55,57,59,59,
            59,59,57,57,57,57,59,59,
            59,59,59,59,59,59,59,59,
            58,59,59,59,59,59,59,58 };

        public static ulong[][] OccupancyVariation;
        public static ulong[][] OccupancyAttackSet;

        public static ulong[][] MagicMovesRook;
        public static ulong[][] MagicMovesBishop;

#endregion

        #region public methods

        static public  void Initialize()
        {

            OccupancyAttackSet = new ulong[64][];
            for(int i = 0; i < 64; i++)
            {
                OccupancyAttackSet[i] = new ulong[4096];
            }
            OccupancyVariation = new ulong[64][];
            for (int i = 0; i < 64; i++)
            {
                OccupancyVariation[i] = new ulong[4096];
            }

            MagicMovesRook = new ulong[64][];
            for (int i = 0; i < 64; i++)
            {
                MagicMovesRook[i] = new ulong[4096];
            }

           GenerateOccupancyVariations(true);
           GenerateMoveDatabase(true);

            MagicMovesBishop = new ulong[64][];
            for (int i = 0; i < 64; i++)
            {
                MagicMovesBishop[i] = new ulong[4096];
            }
            GenerateOccupancyVariations(false);
            GenerateMoveDatabase(false);

            OccupancyAttackSet = null;
            OccupancyVariation = null;
        }

        static public int GetIndexFromSquare(Square s)
        {
            int index = 8 * (int)s.Rank + (int)s.File;
            return index;
        }

        static public Tuple<int, int> GetSquareFromIndex(uint index)
        {
            int i, j;
            if (index / 8 > 0)
            {
                i = (int)(index / 8);
                j = (int)(index % 8);
            }
            else
            {
                i = 0;
                j = (int)index;
            }

            return new Tuple<int,int>(i, j);
        }

        static public ulong GetBitboardFromSquare(Square s)
        {
            return ((ulong)1) << GetIndexFromSquare(s);
        }

        static public Tuple<int, int> GetSquareFromBitboard(ulong bb)
        {
            uint index = BitScanForward(bb);
            var s = GetSquareFromIndex(index);
            return s;
        }

        // De Bruijn Multiplication, see http://chessprogramming.wikispaces.com/BitScan
        static public uint BitScanForward(ulong bb)
        {
            uint[] debruijn64Array =
            {63, 0, 58, 1, 59, 47, 53, 2,
                60, 39, 48, 27, 54, 33, 42, 3,
                61, 51, 37, 40, 49, 18, 28, 20,
                55, 30, 34, 11, 43, 14, 22, 4,
                62, 57, 46, 52, 38, 26, 32, 41,
                50, 36, 17, 19, 29, 10, 13, 21,
                56, 45, 25, 31, 35, 16, 9, 12,
                44, 24, 15, 8, 23, 7, 6, 5};

            const long debruijn64 = 0x07EDD5E59A4E28C2;

            return debruijn64Array[GetBitboardLsb(bb) * debruijn64 >> 58];
        }

        static public ulong GetBitboardLsb(ulong bb)
        {
            return ((ulong)((long)bb & -(long)bb));
        }

        static public List<Tuple<int, int>> GetSquareListFromBB(ulong bb)
        {
            List<Tuple<int, int>> sList = new List<Tuple<int, int>>();

            if (bb != 0)
            {
                do
                {
                    uint index = BitScanForward(bb);
                    var s = GetSquareFromIndex(index);
                    sList.Add(s);
                }
                while ((bb &= bb - 1) != 0);

                return sList;
            }
            return null;
        }


        static int CountSetBits(UInt64 x) //ref:http://en.wikipedia.org/wiki/Hamming_weight
        {
            int count;
            for (count = 0; x != 0; count++)
                x &= x - 1;
            return count;
        }

        static int[] GetSetBits(UInt64 x) //return array holding index of each bit set in UInt64 x
        {
            List<int> result = new List<int>();

            for(int i = 0; x!=0; i++)
            {
                if ((x & 0x01) == 1) //checks if lsb is set
                {
                    result.Add(i);
                }

                x = x >> 1; //shifts x 1 bit to the right
            }
            result.Add(-1); // appends -1 to mark the end of the list
            return result.ToArray<int>();
        }

        public static void GenerateOccupancyVariations(bool isRook) 
    {
            int bitRef;
            int[] bitCount = new int[64];
        
        for (bitRef=0; bitRef<=63; bitRef++)
        {
            ulong mask = isRook ? OccupancyMaskRook[bitRef] : OccupancyMaskBishop[bitRef];
            int[] setBitsInMask = GetSetBits(mask); 
            bitCount[bitRef] = CountSetBits(mask);
            int variationCount = (int)(1LU << bitCount[bitRef]);
            int i;
            for (i=0; i<variationCount; i++)
            {
                OccupancyVariation[bitRef][i] = 0; 

                // find bits set in index "i" and map them to bits in the 64 bit "occupancyVariation"

                int[] setBitsInIndex = GetSetBits((uint)i);
                int j;
                for (j = 0; setBitsInIndex[j] != -1; j++)
                {
                    OccupancyVariation[bitRef][i] |= (1LU << setBitsInMask[setBitsInIndex[j]]);
                }
                
                if (isRook)
                {
                    for (j=bitRef+8; j<=55 && (OccupancyVariation[bitRef][i] & (1LU << j)) == 0; j+=8)
                    if (j>=0 && j<=63) OccupancyAttackSet[bitRef][i] |= (1LU << j);
                    for (j=bitRef-8; j>=8 && (OccupancyVariation[bitRef][i] & (1LU << j)) == 0; j-=8)
                    if (j>=0 && j<=63) OccupancyAttackSet[bitRef][i] |= (1LU << j);
                    for (j=bitRef+1; j%8!=7 && j%8!=0 && (OccupancyVariation[bitRef][i] & (1LU << j)) == 0; j++);
                    if (j>=0 && j<=63) OccupancyAttackSet[bitRef][i] |= (1LU << j);
                    for (j=bitRef-1; j%8!=7 && j%8!=0 && j>=0 && (OccupancyVariation[bitRef][i] & (1LU << j)) == 0; j--);
                    if (j>=0 && j<=63) OccupancyAttackSet[bitRef][i] |= (1LU << j);
                }
                else
                {
                    for (j=bitRef+9; j%8!=7 && j%8!=0 && j<=55 && (OccupancyVariation[bitRef][i] & (1LU << j)) == 0; j+=9);
                    if (j>=0 && j<=63) OccupancyAttackSet[bitRef][i] |= (1LU << j);
                    for (j=bitRef-9; j%8!=7 && j%8!=0 && j>=8 && (OccupancyVariation[bitRef][i] & (1LU << j)) == 0; j-=9);
                    if (j>=0 && j<=63) OccupancyAttackSet[bitRef][i] |= (1LU << j);
                    for (j=bitRef+7; j%8!=7 && j%8!=0 && j<=55 && (OccupancyVariation[bitRef][i] & (1LU << j)) == 0; j+=7);
                    if (j>=0 && j<=63) OccupancyAttackSet[bitRef][i] |= (1LU << j);
                    for (j=bitRef-7; j%8!=7 && j%8!=0 && j>=8 && (OccupancyVariation[bitRef][i] & (1LU << j)) == 0; j-=7);
                    if (j>=0 && j<=63) OccupancyAttackSet[bitRef][i] |= (1LU << j);
                }
            }
        }
    }


        public static void GenerateMoveDatabase(bool isRook)
    {
            int bitRef;

            for (bitRef=0; bitRef<=63; bitRef++)
        {
            int bitCount = isRook ? CountSetBits(OccupancyMaskRook[bitRef]) : CountSetBits(OccupancyMaskBishop[bitRef]);
            int variations = (int)(1L << bitCount);

            int i;
            for (i=0; i<variations; i++)
            {
                ulong validMoves = 0;
                int magicIndex;
                int j;
                if (isRook)
                {
                    magicIndex = (int)((OccupancyVariation[bitRef][i] * MagicNumberRook[bitRef]) >> MagicNumberShiftsRook[bitRef]);

                    for (j=bitRef+8; j<=63; j+=8) { validMoves |= (1LU << j); if ((OccupancyVariation[bitRef][i] & (1LU << j)) != 0) break; }
                    for (j=bitRef-8; j>=0; j-=8) { validMoves |= (1LU << j); if ((OccupancyVariation[bitRef][i] & (1LU << j)) != 0) break; }
                    for (j=bitRef+1; j%8!=0; j++) { validMoves |= (1LU << j); if ((OccupancyVariation[bitRef][i] & (1LU << j)) != 0) break; }
                    for (j=bitRef-1; j%8!=7 && j>=0; j--) { validMoves |= (1LU << j); if ((OccupancyVariation[bitRef][i] & (1LU << j)) != 0) break; }
                    
                    MagicMovesRook[bitRef][magicIndex] = validMoves;
                }
                else
                {
                    magicIndex = (int)((OccupancyVariation[bitRef][i] * MagicNumberBishop[bitRef]) >> MagicNumberShiftsBishop[bitRef]);

                    for (j=bitRef+9; j%8!=0 && j<=63; j+=9) { validMoves |= (1LU << j); if ((OccupancyVariation[bitRef][i] & (1LU << j)) != 0) break; }
                    for (j=bitRef-9; j%8!=7 && j>=0; j-=9) { validMoves |= (1LU << j); if ((OccupancyVariation[bitRef][i] & (1LU << j)) != 0) break; }
                    for (j=bitRef+7; j%8!=7 && j<=63; j+=7) { 
                        validMoves |= (1LU << j); 
                        if ((OccupancyVariation[bitRef][i] & (1LU << j)) != 0) 
                            break; 
                    }
                    for (j=bitRef-7; j%8!=0 && j>=0; j-=7) { 
                        validMoves |= (1LU << j); 
                        if ((OccupancyVariation[bitRef][i] & (1LU << j)) != 0) 
                            break; 
                    }

                    MagicMovesBishop[bitRef][magicIndex] = validMoves;
                }
            }
        }
    }

        #endregion

    }
}

    