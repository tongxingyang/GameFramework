using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


namespace I2.TextAnimation
{
    public struct SE_Character
    {
        public Vector2 Min, Max;
        public char Character;
        public float TopY, BaselineY, Height;
        public int iCharacterInText, iCharacterInLine, 
                   iLine, 
                   iWord, iWordInLine,
                   iParagraph;
    }

	public struct SEVertex
	{
		public Vector3 position;
		public Vector2 uv;
		public Color32 color;

		public Vector2 uv1;
		public Vector3 normal;
		public Vector4 tangent;

		public int characterID;

		public static SEVertex Get ( UIVertex vert, byte characterID )
		{
			seVertex.position 		= vert.position;
			seVertex.color    		= vert.color;
			seVertex.uv       		= vert.uv0;
			seVertex.characterID 	= characterID;
			return seVertex;
		}

		public static SEVertex seVertex = new SEVertex();
	}

}