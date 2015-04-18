using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS
{
    public class Require
    {
        public static void NotNull(object obj, string name)
        {
            if (obj == null) throw new ArgumentNullException(name);
        }

        public static void NotNull<T>(T? nullable, string name) where T : struct
        {
            if (!nullable.HasValue) throw new ArgumentNullException(name);
        }

        [Conditional("DEBUG")]
        public static void DebugImplies(bool ifTrue, bool thenTrue, string message = null)
        {
            Debug.Assert(!ifTrue || thenTrue, message);
        }
    }
}
