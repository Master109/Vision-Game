using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
	public static class QuaternionExtensions
	{
		public static Quaternion NULL = new Quaternion(MathfExtensions.NULL_FLOAT, MathfExtensions.NULL_FLOAT, MathfExtensions.NULL_FLOAT, MathfExtensions.NULL_FLOAT);

		public static Vector3 GetAngularVelocity (Quaternion fromRotation, Quaternion toRotation)
		{
			Quaternion q = toRotation * Quaternion.Inverse(fromRotation);
			if(Mathf.Abs(q.w) > 1023.5f / 1024.0f)
				return new Vector3(0 ,0, 0);
			float gain;
			if (q.w < 0.0f)
			{
				float angle = Mathf.Acos(-q.w);
				gain = -2.0f * angle / (Mathf.Sin(angle) * Time.deltaTime);
			}
			else
			{
				float angle = Mathf.Acos(q.w);
				gain = 2.0f * angle / (Mathf.Sin(angle) * Time.deltaTime);
			}
			return new Vector3(q.x * gain, q.y * gain, q.z * gain);
		}
	}
}