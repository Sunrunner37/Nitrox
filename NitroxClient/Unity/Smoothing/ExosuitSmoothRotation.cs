﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NitroxClient.Unity.Smoothing
{
    class ExosuitSmoothRotation : SmoothRotation
    {
        private const float SMOOTHING_SPEED = 10f;
        private float timeCount = 0;
        private Quaternion target;
        public new Quaternion Target
        {
            get { return target; }
            set
            {
                NitroxModel.Logger.Log.Debug("set rotation target");
                timeCount = 0;
                target = value;
            }
        }

        public ExosuitSmoothRotation(Quaternion initial)
        {
            Target = SmoothValue = initial;
        }

        public ExosuitSmoothRotation()
        {
        }

        public new void FixedUpdate()
        {
            SmoothValue = Quaternion.Slerp(SmoothValue, Target, timeCount + SMOOTHING_SPEED * Time.fixedDeltaTime);
            timeCount += SMOOTHING_SPEED * Time.fixedDeltaTime;
        }
    }
}