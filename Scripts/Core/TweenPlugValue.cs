﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;

namespace M8.Animator {
    public struct TweenPlugValueSetOptions : IPlugOptions {
        private SequenceControl mSeq;
        private int mLoopCount;

        public void Reset() {
            SetSequence(null);
        }

        public void SetSequence(SequenceControl seq) {
            if(mSeq != seq) {
                if(mSeq != null)
                    mSeq.stepCompleteCallback -= OnStepComplete;

                mSeq = seq;

                if(mSeq != null)
                    mSeq.stepCompleteCallback += OnStepComplete;
            }

            mLoopCount = 0;
        }

        public bool Refresh(ref int counter) {
            if(counter != mLoopCount) {
                counter = mLoopCount;

                return true;
            }
            return false;
        }

        void OnStepComplete() {
            mLoopCount++;
        }
    }

    public struct TWeenPlugNoneOptions : IPlugOptions {
        void IPlugOptions.Reset() { }
    }

    /// <summary>
    /// Use to apply a single value within a tween.
    /// Note: Set getter as the current value to be compaired to the target value. setter is used to apply the target value to current value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TweenPlugValueSet<T> : ABSTweenPlugin<T, T, TWeenPlugNoneOptions> {
        public override T ConvertToStartValue(TweenerCore<T, T, TWeenPlugNoneOptions> t, T value) {
            return t.endValue; //start value is the same as the end value
        }

        public override void EvaluateAndApply(TWeenPlugNoneOptions options, Tween t, bool isRelative, DOGetter<T> getter, DOSetter<T> setter, float elapsed, T startValue, T changeValue, float duration, bool usingInversePosition, UpdateNotice updateNotice) {            
            var curVal = getter();
            if(!curVal.Equals(startValue)) {
                setter(startValue);
            }
        }

        public override float GetSpeedBasedDuration(TWeenPlugNoneOptions options, float unitsXSecond, T changeValue) {
            return 1.0f / unitsXSecond;
        }

        public override void Reset(TweenerCore<T, T, TWeenPlugNoneOptions> t) { }
        public override void SetChangeValue(TweenerCore<T, T, TWeenPlugNoneOptions> t) { }
        public override void SetFrom(TweenerCore<T, T, TWeenPlugNoneOptions> t, bool isRelative) { }
        public override void SetFrom(TweenerCore<T, T, TWeenPlugNoneOptions> t, T fromValue, bool setImmediately) { }
        public override void SetRelativeEndValue(TweenerCore<T, T, TWeenPlugNoneOptions> t) { }
    }

    /// <summary>
    /// setter is passed the elapsed time, getter is not used
    /// </summary>
    public class TweenPlugValueSetElapsed : ABSTweenPlugin<float, float, TweenPlugValueSetOptions> {
        private int mCounter = -1;

        public override float ConvertToStartValue(TweenerCore<float, float, TweenPlugValueSetOptions> t, float value) {
            return value;
        }

        public override void EvaluateAndApply(TweenPlugValueSetOptions options, Tween t, bool isRelative, DOGetter<float> getter, DOSetter<float> setter, float elapsed, float startValue, float changeValue, float duration, bool usingInversePosition, UpdateNotice updateNotice) {
            if(updateNotice == UpdateNotice.RewindStep)
                mCounter = -1;
            else if(options.Refresh(ref mCounter))
                setter(elapsed);
        }

        public override float GetSpeedBasedDuration(TweenPlugValueSetOptions options, float unitsXSecond, float changeValue) {
            return 1.0f / unitsXSecond;
        }

        public override void Reset(TweenerCore<float, float, TweenPlugValueSetOptions> t) {
            mCounter = -1;
        }

        public override void SetChangeValue(TweenerCore<float, float, TweenPlugValueSetOptions> t) { }

        public override void SetFrom(TweenerCore<float, float, TweenPlugValueSetOptions> t, bool isRelative) { }
        public override void SetFrom(TweenerCore<float, float, TweenPlugValueSetOptions> t, float fromValue, bool setImmediately) { }

        public override void SetRelativeEndValue(TweenerCore<float, float, TweenPlugValueSetOptions> t) { }
    }
}
