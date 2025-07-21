using System;
using System.Numerics;

namespace LearnGC.Scripts.Runtime
{
    struct MyStruct : IDisposable
    {
        public long a;
        public long b;
        public long c;
        public long d;
        public long e;
        public long f;
        public long aa;
        public long g;
        public long h;

        public void Dispose()
        {
        }
    }
}