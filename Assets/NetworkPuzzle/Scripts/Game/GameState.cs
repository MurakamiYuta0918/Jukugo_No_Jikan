using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkPuzzle {
    
    public abstract class GameState {
        Coroutine coroutine;
        Coroutine timeCountCoroutine;
        Timer timer;
        public bool IsRunning { get; private set; }

        public GameState (Timer timer ) {
            this.timer = timer;
        }

        public void Start () {
            if (IsRunning) return;

            IsRunning = true;
            OnStart();
            coroutine = timer.StartCoroutine(ManageRoutine());
        }

        protected virtual void OnStart () {} 

        IEnumerator ManageRoutine () {
            yield return timer.StartCoroutine(Routine());
            Stop();
        }

        protected abstract IEnumerator Routine(); 

        protected Coroutine StartTimeCount(float timeLimit, System.Action<float> writeMethod) {
            return timeCountCoroutine = timer.StartCoroutine(TimeCount(timeLimit, writeMethod));
        }

        IEnumerator TimeCount(float timeLimit, System.Action<float> writeMethod) {
            float currentTime = timeLimit;
            while (currentTime >= 0f) {
                yield return null;
                if (!timer.IsPlay) continue;
                currentTime -= Time.deltaTime;
                writeMethod(currentTime);
            }
            writeMethod(0f);
            timeCountCoroutine = null;
        }

        public void Stop() {
            if (!IsRunning) return;

            if (timeCountCoroutine != null) {
                timer.StopCoroutine(timeCountCoroutine);
                timeCountCoroutine = null;
            }

            timer.StopCoroutine(coroutine);
            coroutine = null;
            OnStop();
            IsRunning = false;
        }

        protected virtual void OnStop() {}
    }

}
