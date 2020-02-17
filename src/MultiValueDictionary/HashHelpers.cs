using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AIlins.Collections.Generic
{
    internal interface IWellKnownStringEqualityComparer
    {
        System.Collections.IEqualityComparer GetRandomizedEqualityComparer();

        System.Collections.IEqualityComparer GetEqualityComparerForSerialization();
    }
    internal sealed class RandomizedObjectEqualityComparer : System.Collections.IEqualityComparer, IWellKnownStringEqualityComparer
    {
        private long _entropy;
        [MethodImpl(MethodImplOptions.InternalCall)]
        [SecurityCritical]
        internal static extern int InternalMarvin32HashString(string s, int strLen, long additionalEntropy);
        public RandomizedObjectEqualityComparer()
        {
            _entropy = HashHelpers.GetEntropy();
        }

        public new bool Equals(object x, object y)
        {
            if (x != null)
            {
                if (y != null)
                {
                    return x.Equals(y);
                }
                return false;
            }
            if (y != null)
            {
                return false;
            }
            return true;
        }

        [SecuritySafeCritical]
        public int GetHashCode(object obj)
        {
            if (obj == null)
            {
                return 0;
            }
            string text = obj as string;
            if (text != null)
            {
                return InternalMarvin32HashString(text, text.Length, _entropy);
            }
            return obj.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            RandomizedObjectEqualityComparer randomizedObjectEqualityComparer = obj as RandomizedObjectEqualityComparer;
            if (randomizedObjectEqualityComparer != null)
            {
                return _entropy == randomizedObjectEqualityComparer._entropy;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return GetType().Name.GetHashCode() ^ (int)(_entropy & 0x7FFFFFFF);
        }

        System.Collections.IEqualityComparer IWellKnownStringEqualityComparer.GetRandomizedEqualityComparer()
        {
            return new RandomizedObjectEqualityComparer();
        }

        System.Collections.IEqualityComparer IWellKnownStringEqualityComparer.GetEqualityComparerForSerialization()
        {
            return null;
        }
    }
    internal sealed class RandomizedStringEqualityComparer : IEqualityComparer<string>, System.Collections.IEqualityComparer, IWellKnownStringEqualityComparer
    {
        private long _entropy;
        [MethodImpl(MethodImplOptions.InternalCall)]
        [SecurityCritical]
        internal static extern int InternalMarvin32HashString(string s, int strLen, long additionalEntropy);
        public RandomizedStringEqualityComparer()
        {
            _entropy = HashHelpers.GetEntropy();
        }

        public new bool Equals(object x, object y)
        {
            if (x == y)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            if (x is string && y is string)
            {
                return Equals((string)x, (string)y);
            }
            ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArgumentForComparison);
            return false;
        }

        public bool Equals(string x, string y)
        {
            if (x != null)
            {
                if (y != null)
                {
                    return x.Equals(y);
                }
                return false;
            }
            if (y != null)
            {
                return false;
            }
            return true;
        }

        [SecuritySafeCritical]
        public int GetHashCode(string obj)
        {
            if (obj == null)
            {
                return 0;
            }
            return InternalMarvin32HashString(obj, obj.Length, _entropy);
        }

        [SecuritySafeCritical]
        public int GetHashCode(object obj)
        {
            if (obj == null)
            {
                return 0;
            }
            string text = obj as string;
            if (text != null)
            {
                return InternalMarvin32HashString(text, text.Length, _entropy);
            }
            return obj.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            RandomizedStringEqualityComparer randomizedStringEqualityComparer = obj as RandomizedStringEqualityComparer;
            if (randomizedStringEqualityComparer != null)
            {
                return _entropy == randomizedStringEqualityComparer._entropy;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return GetType().Name.GetHashCode() ^ (int)(_entropy & 0x7FFFFFFF);
        }

        System.Collections.IEqualityComparer IWellKnownStringEqualityComparer.GetRandomizedEqualityComparer()
        {
            return new RandomizedStringEqualityComparer();
        }

        System.Collections.IEqualityComparer IWellKnownStringEqualityComparer.GetEqualityComparerForSerialization()
        {
            return EqualityComparer<string>.Default;
        }
    }
    internal static class HashHelpers
    {
        public const int HashCollisionThreshold = 100;

        
        public static readonly int[] primes = new int[72]
        {
        3,
        7,
        11,
        17,
        23,
        29,
        37,
        47,
        59,
        71,
        89,
        107,
        131,
        163,
        197,
        239,
        293,
        353,
        431,
        521,
        631,
        761,
        919,
        1103,
        1327,
        1597,
        1931,
        2333,
        2801,
        3371,
        4049,
        4861,
        5839,
        7013,
        8419,
        10103,
        12143,
        14591,
        17519,
        21023,
        25229,
        30293,
        36353,
        43627,
        52361,
        62851,
        75431,
        90523,
        108631,
        130363,
        156437,
        187751,
        225307,
        270371,
        324449,
        389357,
        467237,
        560689,
        672827,
        807403,
        968897,
        1162687,
        1395263,
        1674319,
        2009191,
        2411033,
        2893249,
        3471899,
        4166287,
        4999559,
        5999471,
        7199369
        };

        private static ConditionalWeakTable<object, SerializationInfo> s_SerializationInfoTable;

        public const int MaxPrimeArrayLength = 2146435069;

        private const int bufferSize = 1024;

        private static RandomNumberGenerator rng;

        private static byte[] data;

        private static int currentIndex = 1024;

        private static readonly object lockObj = new object();

        internal static ConditionalWeakTable<object, SerializationInfo> SerializationInfoTable
        {
            get
            {
                if (s_SerializationInfoTable == null)
                {
                    ConditionalWeakTable<object, SerializationInfo> value = new ConditionalWeakTable<object, SerializationInfo>();
                    Interlocked.CompareExchange(ref s_SerializationInfoTable, value, null);
                }
                return s_SerializationInfoTable;
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static bool IsPrime(int candidate)
        {
            if ((candidate & 1) != 0)
            {
                int num = (int)Math.Sqrt((double)candidate);
                for (int i = 3; i <= num; i += 2)
                {
                    if (candidate % i == 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            return candidate == 2;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static int GetPrime(int min)
        {
            if (min < 0)
            {
                throw new ArgumentException(ThrowHelper.GetResourceString("Arg_HTCapacityOverflow"));
            }
            for (int i = 0; i < primes.Length; i++)
            {
                int num = primes[i];
                if (num >= min)
                {
                    return num;
                }
            }
            for (int j = min | 1; j < 2147483647; j += 2)
            {
                if (IsPrime(j) && (j - 1) % 101 != 0)
                {
                    return j;
                }
            }
            return min;
        }

        public static int GetMinPrime()
        {
            return primes[0];
        }

        public static int ExpandPrime(int oldSize)
        {
            int num = 2 * oldSize;
            if ((uint)num > 2146435069u && 2146435069 > oldSize)
            {
                return 2146435069;
            }
            return GetPrime(num);
        }

        public static bool IsWellKnownEqualityComparer(object comparer)
        {
            if (comparer != null && comparer != EqualityComparer<string>.Default)
            {
                return comparer is IWellKnownStringEqualityComparer;
            }
            return true;
        }

        public static System.Collections.IEqualityComparer GetRandomizedEqualityComparer(object comparer)
        {
            if (comparer == null)
            {
                return new RandomizedObjectEqualityComparer();
            }
            if (comparer == EqualityComparer<string>.Default)
            {
                return new RandomizedStringEqualityComparer();
            }
            return (comparer as IWellKnownStringEqualityComparer)?.GetRandomizedEqualityComparer();
        }

        public static object GetEqualityComparerForSerialization(object comparer)
        {
            if (comparer == null)
            {
                return null;
            }
            IWellKnownStringEqualityComparer wellKnownStringEqualityComparer = comparer as IWellKnownStringEqualityComparer;
            if (wellKnownStringEqualityComparer != null)
            {
                return wellKnownStringEqualityComparer.GetEqualityComparerForSerialization();
            }
            return comparer;
        }

        internal static long GetEntropy()
        {
            lock (lockObj)
            {
                if (currentIndex == 1024)
                {
                    if (rng == null)
                    {
                        rng = RandomNumberGenerator.Create();
                        data = new byte[1024];
                    }
                    rng.GetBytes(data);
                    currentIndex = 0;
                }
                long result = BitConverter.ToInt64(data, currentIndex);
                currentIndex += 8;
                return result;
            }
        }
    }
}
