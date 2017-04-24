using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class ProfilerScope : IDisposable {

    public ProfilerScope(string name) {
        Profiler.BeginSample(name);
    }

    public void Dispose() {
        Profiler.EndSample();
    }
}
