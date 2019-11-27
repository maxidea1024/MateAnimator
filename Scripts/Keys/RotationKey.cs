﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.CustomPlugins;

namespace M8.Animator {
    [System.Serializable]
    public class RotationKey : Key {
        public override SerializeType serializeType { get { return SerializeType.Rotation; } }

        //public int type = 0; // 0 = Rotate To, 1 = Look At
        public Quaternion rotation;

        public int endFrame;

        // copy properties from key
        public override void CopyTo(Key key) {
            base.CopyTo(key);

            RotationKey a = key as RotationKey;

            //a.type = type;
            a.rotation = rotation;
        }

        #region action
        public override int getNumberOfFrames(int frameRate) {
            if(!canTween && (endFrame == -1 || endFrame == frame))
                return 1;
            else if(endFrame == -1)
                return -1;
            return endFrame - frame;
        }
        public override void build(SequenceControl seq, Track track, int index, UnityEngine.Object obj) {
            Transform trans = obj as Transform;
            var transParent = trans.parent;

            Rigidbody body = trans.GetComponent<Rigidbody>();
            Rigidbody2D body2D = !body ? trans.GetComponent<Rigidbody2D>() : null;

            int frameRate = seq.take.frameRate;

            //allow tracks with just one key
            if(track.keys.Count == 1)
                interp = Interpolation.None;

            if(!canTween) {
                TweenerCore<Quaternion, Quaternion, TWeenPlugNoneOptions> tween;

                if(body2D)
                    tween = DOTween.To(new TweenPlugValueSet<Quaternion>(), () => trans.localRotation, (x) => body2D.rotation = (x * transParent.rotation).eulerAngles.z, rotation, getTime(frameRate));
                else if(body)
                    tween = DOTween.To(new TweenPlugValueSet<Quaternion>(), () => trans.localRotation, (x) => body.rotation = x * transParent.rotation, rotation, getTime(frameRate));
                else
                    tween = DOTween.To(new TweenPlugValueSet<Quaternion>(), () => trans.localRotation, (x) => trans.localRotation = x, rotation, getTime(frameRate));

                seq.Insert(this, tween);
            }
            else if(endFrame == -1) return;
            else {
                Quaternion endRotation = (track.keys[index + 1] as RotationKey).rotation;

                TweenerCore<Quaternion, Quaternion, DG.Tweening.Plugins.Options.NoOptions> tween;

                if(body2D)
                    tween = DOTween.To(new PureQuaternionPlugin(), () => trans.localRotation, (x) => body2D.MoveRotation((x * transParent.rotation).eulerAngles.z), endRotation, getTime(frameRate));
                else if(body)
                    tween = DOTween.To(new PureQuaternionPlugin(), () => trans.localRotation, (x) => body.MoveRotation(x * transParent.rotation), endRotation, getTime(frameRate));
                else
                    tween = DOTween.To(new PureQuaternionPlugin(), () => trans.localRotation, (x) => trans.localRotation = x, endRotation, getTime(frameRate));

                if(hasCustomEase())
                    tween.SetEase(easeCurve);
                else
                    tween.SetEase((Ease)easeType, amplitude, period);

                seq.Insert(this, tween);
            }
        }
        #endregion
    }
}