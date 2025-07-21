using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using Object = System.Object;

namespace LearnGC.Scripts.Runtime
{
    internal class LearnGCMain : MonoBehaviour
    {
        private int _intValue;
        private readonly List<int> _actionPointers = new();
        private MyStruct _myStruct = new MyStruct();
        private MyClass _myClass = new MyClass();

        private void Awake()
        {
            Debug.Log($"sizeof:{Marshal.SizeOf(_myStruct)}");
        }

        private IEnumerator Pause()
        {
            for (int i = 0; i < 20; i++)
            {
                yield return null;
            }

            Time.timeScale = 0;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPaused = true;
#endif
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Test GC"))
            {
                Profiler.BeginSample("Exam GC");
                for (int i = 0; i < 2; i++)
                {
                    SingleLoop();
                }

                Profiler.EndSample();
                GC.Collect();
                GC.WaitForPendingFinalizers();
#if UNITY_WEBGL
                Application.ExternalEval("if(window.gc) window.gc();");
#endif
                Debug.Log(string.Join(" ", _actionPointers));
                StartCoroutine(Pause());
            }
        }

        private void SingleLoop()
        {
            //测试结构体
            Profiler.BeginSample("Struct GC GetType");
            var type = _myStruct.GetType();
            Profiler.EndSample();

            Profiler.BeginSample("Struct GC GetHashCode");
            var hashCode = _myStruct.GetHashCode();
            Profiler.EndSample();

            Profiler.BeginSample("Struct GC Interface");
            var interfaceValue = _myStruct as IDisposable;
            Profiler.EndSample();

            Profiler.BeginSample("Struct GC Class Field");
            var myClass = new MyClass();
            Profiler.EndSample();

            Profiler.BeginSample("Struct Performance");
            MyStruct myStruct = new MyStruct();
            myStruct.a = 12345;
            // Debug.Log($"Test1 {myStruct.a}");
            Test2(myStruct);
            Profiler.EndSample();

            //测试简单值类型
            Profiler.BeginSample("IntValue GC GetType");
            var type1 = _myStruct.GetType();
            Profiler.EndSample();

            Profiler.BeginSample("IntValue GC GetHashCode");
            var hashCode1 = _myStruct.GetHashCode();
            Profiler.EndSample();

            //测试匿名方法
            Profiler.BeginSample("Anonymous GC Global Relation");
            Test1(() => { _myStruct.a++; });
            Profiler.EndSample();

            Profiler.BeginSample("Anonymous GC Local Relation");
            MyStruct internalStruct = new MyStruct();
            Test1(() => { internalStruct.a++; });
            Profiler.EndSample();

            Profiler.BeginSample("Anonymous GC Non Relation");
            Test1(() => { });
            Profiler.EndSample();
        }

        private void Test1(Action action)
        {
            action?.Invoke();
        }

        private void Test2(in MyStruct myStruct)
        {
            // Debug.Log($"Test2 {myStruct.a}");
            Test3(myStruct);
        }

        private void Test3(in MyStruct myStruct)
        {
            // Debug.Log($"Test3 {myStruct.a}");
            Test4(in myStruct);
        }

        private void Test4(in MyStruct myStruct)
        {
            // Debug.Log($"Test4 {myStruct.a}");
        }
    }
}