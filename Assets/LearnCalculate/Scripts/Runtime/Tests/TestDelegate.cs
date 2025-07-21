using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace LearnCalculate.Scripts.Runtime.Tests
{
    public class TestDelegate
    {
        public event Action UpdateLoopHandle;

        [TestCase()]
        public void TestDelegateReset()
        {
            UpdateLoopHandle -= OnUpdate;
            UpdateLoopHandle += OnUpdate;
        }

        private void OnUpdate()
        {
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TestUnityQueueTimer()
        {
            yield break;
        }
    }
}