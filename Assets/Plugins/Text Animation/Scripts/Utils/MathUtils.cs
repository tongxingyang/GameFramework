using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace I2.TextAnimation
{
	public static class MathUtils
	{
		public static Vector3 ExtractTranslationFromMatrix(ref Matrix4x4 matrix) 
		{
			Vector3 translate;
			translate.x = matrix.m03;
			translate.y = matrix.m13;
			translate.z = matrix.m23;
			return translate;
		}
		
		public static Quaternion ExtractRotationFromMatrix(ref Matrix4x4 matrix) 
		{
			Vector3 forward;
			forward.x = matrix.m02;
			forward.y = matrix.m12;
			forward.z = matrix.m22;
			
			Vector3 upwards;
			upwards.x = matrix.m01;
			upwards.y = matrix.m11;
			upwards.z = matrix.m21;
			
			return Quaternion.LookRotation(forward, upwards);
		}
		
		public static Vector3 ExtractScaleFromMatrix(ref Matrix4x4 matrix) 
		{
			Vector3 scale;
			scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
			scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
			scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
			return scale;
		}
		
		public static void DecomposeMatrix(ref Matrix4x4 matrix, out Vector3 localPosition, out Quaternion localRotation, out Vector3 localScale) 
		{
			localPosition = ExtractTranslationFromMatrix(ref matrix);
			localRotation = ExtractRotationFromMatrix(ref matrix);
			localScale = ExtractScaleFromMatrix(ref matrix);
		}
		
		public static void SetTransformFromMatrix(Transform transform, ref Matrix4x4 matrix) 
		{
			transform.localPosition = ExtractTranslationFromMatrix(ref matrix);
			transform.localRotation = ExtractRotationFromMatrix(ref matrix);
			transform.localScale = ExtractScaleFromMatrix(ref matrix);
		}
		
		public static bool IsRotationAndScaleIdentity( Transform tr )
		{
			Matrix4x4 matrix = tr.localToWorldMatrix;
			matrix.m03 = 0;
			matrix.m13 = 0;
			matrix.m23 = 0;

			//Debug.Log ( (matrix==Matrix4x4.identity) + " " + matrix.isIdentity);
			//return matrix.isIdentity;
			return (matrix==Matrix4x4.identity);
		}

        public static float LerpUnclamped( float a, float b, float t)
        {
            return (b - a) * t + a;
        }

        public static byte LerpByte(byte a, byte b, float t)    { return (byte)(a + (b - a) * t); }
        public static byte Float2Byte( float value )            { return (byte)(255 * Mathf.Clamp01(value)); }

		// It is faster to access this variation than Quaternion.identity
		public static readonly Quaternion IdentityQuaternion = Quaternion.identity;

		// It is faster to access this variation than Matrix4x4.identity
		public static readonly Matrix4x4 IdentityMatrix = Matrix4x4.identity;
        public static readonly Vector2 v2one = Vector2.one;
        public static readonly Vector2 v2zero = Vector2.zero;
        public static readonly Vector2 v2down = -Vector2.up;
        public static readonly Vector2 v2max = new Vector2(float.MaxValue, float.MaxValue);
        public static readonly Vector2 v2min = new Vector2(float.MinValue, float.MinValue);



        public static readonly Vector3 v3one = Vector3.one;
		public static readonly Vector3 v3zero = Vector3.zero;
		public static readonly Vector3 v3half = Vector3.one*0.5f;

		public static readonly Vector4 v4one = Vector4.one;
		public static readonly Vector4 v4zero = Vector4.zero;

		public static readonly Rect defaultRect = new Rect();

        public static readonly Color   white = Color.white;
        public static readonly Color32 white32 = Color.white;
        public static readonly Color transparentWhite = new Color(1, 1, 1, 0);

        public static readonly Color black = Color.black;
        public static readonly Color transparentBlack = new Color(0, 0, 0, 0);

        public static Color32 tempColor;
		public static Vector2 tempV2;
		public static Vector3 tempV3;
	}

    public static class ListExtension
    {
        public static void ReserveExtra<T>(this List<T> list, int newElements)
        {
            ReserveTotal(list, list.Count + newElements);
        }
        public static void ReserveTotal<T>(this List<T> list, int newSize)
        {
            if (list.Capacity < newSize)
            {
                list.Capacity = newSize;
            }
        }
        public static void Reset<T>(this List<T> list, int Capacity)
        {
            list.Clear();
            list.ReserveTotal(Capacity);
        }
    }
}