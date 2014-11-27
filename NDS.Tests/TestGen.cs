using System;
using System.Collections.Generic;

namespace NDS.Tests
{
    /// <summary>Utility class for generating random values.</summary>
    public static class TestGen
    {
        /// <summary>Generates a sequence of random ints from the given generator.</summary>
        /// <param name="random">The generator.</param>
        /// <returns>An infinite sequences of random ints generated from <paramref name="random"/>.</returns>
        public static IEnumerable<int> RandomInts(Random random)
        {
            return Generate(random, r => r.Next());
        }

        /// <summary>Generates an infinite sequence of items generated using the given generator and generator transform.</summary>
        /// <typeparam name="T">The type of generated items.</typeparam>
        /// <param name="r">Random generator for the sequence.</param>
        /// <param name="gen">Used to generate the next item in the sequence using the random source.</param>
        /// <returns>An inifinite sequence of items generated from <paramref name="gen"/>.</returns>
        public static IEnumerable<T> Generate<T>(Random r, Func<Random, T> gen)
        {
            while (true)
            {
                yield return gen(r);
            }
        }
    }
}
