namespace NDS
{
    /// <summary>Represents a method of extracting individual bits for a fixed-length type.</summary>
    /// <typeparam name="T"></typeparam>
    public interface IBitAddressable<T>
    {
        /// <summary>Gets the number of bits in type <typeparamref name="T"/>.</summary>
        int NumBits { get; }

        /// <summary>Gets the bit at the given index from a value.</summary>
        /// <param name="item">The item to index.</param>
        /// <param name="bitIndex">The bit to index. Index 0 is the most significant bit and (NumBits - 1) the least significant.</param>
        /// <returns></returns>
        Bit GetBit(T item, int bitIndex);
    }

    public static class BitAddressable
    {
        private class Int32BitAddressable : IBitAddressable<int>
        {
            public int NumBits
            {
                get { return 32; }
            }

            public Bit GetBit(int item, int bitIndex)
            {
                return (Bit)((item >> (32 - bitIndex - 1)) & 0x00000001);
            }
        }


        /// <summary>Gets the bit indexer for Int32.</summary>
        public static IBitAddressable<int> Int32
        {
            get { return new Int32BitAddressable(); }
        }
    }
}
