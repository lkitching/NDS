namespace NDS.Algorithms.Sorting
{
    /// <summary>Represents a method of extracting individual bytes from fixed-length elements.</summary>
    /// <typeparam name="T">The item to address.</typeparam>
    public interface IByteAddressable<T>
    {
        /// <summary>The number of bytes in each item.</summary>
        int NumBytes { get; }

        /// <summary>Gets the nth byte within an input value. Byte 0 is the most significant and byte (NumBytes - 1) the least.</summary>
        /// <param name="item">The item to index.</param>
        /// <param name="msbIndex">The index of the byte to retrieve.</param>
        /// <returns></returns>
        byte GetByte(T item, int msbIndex);
    }

    /// <summary>Indexer for the bytes within an int. </summary>
    public class Int32Radix : IByteAddressable<int>
    {
        public int NumBytes
        {
            get { return 4; }
        }

        public byte GetByte(int item, int msbIndex)
        {
            return (byte)((item >> ((3 - msbIndex) * 8)) & 0x000000FF);
        }
    }
}
