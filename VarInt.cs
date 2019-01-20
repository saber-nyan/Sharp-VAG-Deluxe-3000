using System.Collections.Generic;

namespace Sharp_VAG_Deluxe_3000 {
    public static class VarInt {
        public static IEnumerable<byte> Write(int value) {
            while (value != 0) {
                var current = value & 0x7F;
                value >>= 7;
                if (value != 0)
                    yield return (byte) (current | 0x80);
                else
                    yield return (byte) current;
            }
        }
    }
}