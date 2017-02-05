using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.EventSystems;
using System;

public interface INotePositions
{
	void PushNote(int idx, Vector2 note);
}

public class NotePositions : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, INotePositions
{
	public float AngleMultiplier = 1.0f;
	public float SpeedMultiplier = 1.0f;
	public float CurveScale = 1.0f;
	private TextMeshPro mTextMeshPro;
	bool hasTextChanged;
	bool m_isHoveringObject;
	private int m_selectedLink = -1;
	private int m_lastCharIndex = -1;
	private TMP_MeshInfo[] m_cachedMeshInfoVertexData;
	
	[SerializeField]private int GATE_NUMBER;

	Matrix4x4 m_matrix;

	public Camera m_Camera;

	private float SEMITONE_DIST;
	// private float SEMITONE_DIST = 0.9f / 2 * 0.5f;
	// private float TINY_OFFSET = 0.3f;
	private float TINY_OFFSET;

	private DKThrow dk;
	// We need an array containing the pitches
	public enum Pitch
	{
		/// <summary>C in octave -1.</summary>
		CNeg1 = 0,
		/// <summary>C# in octave -1.</summary>
		CSharpNeg1 = 1,
		/// <summary>D in octave -1.</summary>
		DNeg1 = 2,
		/// <summary>D# in octave -1.</summary>
		DSharpNeg1 = 3,
		/// <summary>E in octave -1.</summary>
		ENeg1 = 4,
		/// <summary>F in octave -1.</summary>
		FNeg1 = 5,
		/// <summary>F# in octave -1.</summary>
		FSharpNeg1 = 6,
		/// <summary>G in octave -1.</summary>
		GNeg1 = 7,
		/// <summary>G# in octave -1.</summary>
		GSharpNeg1 = 8,
		/// <summary>A in octave -1.</summary>
		ANeg1 = 9,
		/// <summary>A# in octave -1.</summary>
		ASharpNeg1 = 10,
		/// <summary>B in octave -1.</summary>
		BNeg1 = 11,

		/// <summary>C in octave 0.</summary>
		C0 = 12,
		/// <summary>C# in octave 0.</summary>
		CSharp0 = 13,
		/// <summary>D in octave 0.</summary>
		D0 = 14,
		/// <summary>D# in octave 0.</summary>
		DSharp0 = 15,
		/// <summary>E in octave 0.</summary>
		E0 = 16,
		/// <summary>F in octave 0.</summary>
		F0 = 17,
		/// <summary>F# in octave 0.</summary>
		FSharp0 = 18,
		/// <summary>G in octave 0.</summary>
		G0 = 19,
		/// <summary>G# in octave 0.</summary>
		GSharp0 = 20,
		/// <summary>A in octave 0.</summary>
		A0 = 21,
		/// <summary>A# in octave 0, usually the lowest key on an 88-key keyboard.</summary>
		ASharp0 = 22,
		/// <summary>B in octave 0.</summary>
		B0 = 23,

		/// <summary>C in octave 1.</summary>
		C1 = 24,
		/// <summary>C# in octave 1.</summary>
		CSharp1 = 25,
		/// <summary>D in octave 1.</summary>
		D1 = 26,
		/// <summary>D# in octave 1.</summary>
		DSharp1 = 27,
		/// <summary>E in octave 1.</summary>
		E1 = 28,
		/// <summary>F in octave 1.</summary>
		F1 = 29,
		/// <summary>F# in octave 1.</summary>
		FSharp1 = 30,
		/// <summary>G in octave 1.</summary>
		G1 = 31,
		/// <summary>G# in octave 1.</summary>
		GSharp1 = 32,
		/// <summary>A in octave 1.</summary>
		A1 = 33,
		/// <summary>A# in octave 1.</summary>
		ASharp1 = 34,
		/// <summary>B in octave 1.</summary>
		B1 = 35,

		/// <summary>C in octave 2.</summary>
		C2 = 36,
		/// <summary>C# in octave 2.</summary>
		CSharp2 = 37,
		/// <summary>D in octave 2.</summary>
		D2 = 38,
		/// <summary>D# in octave 2.</summary>
		DSharp2 = 39,
		/// <summary>E in octave 2.</summary>
		E2 = 40,
		/// <summary>F in octave 2.</summary>
		F2 = 41,
		/// <summary>F# in octave 2.</summary>
		FSharp2 = 42,
		/// <summary>G in octave 2.</summary>
		G2 = 43,
		/// <summary>G# in octave 2.</summary>
		GSharp2 = 44,
		/// <summary>A in octave 2.</summary>
		A2 = 45,
		/// <summary>A# in octave 2.</summary>
		ASharp2 = 46,
		/// <summary>B in octave 2.</summary>
		B2 = 47,

		/// <summary>C in octave 3.</summary>
		C3 = 48,
		/// <summary>C# in octave 3.</summary>
		CSharp3 = 49,
		/// <summary>D in octave 3.</summary>
		D3 = 50,
		/// <summary>D# in octave 3.</summary>
		DSharp3 = 51,
		/// <summary>E in octave 3.</summary>
		E3 = 52,
		/// <summary>F in octave 3.</summary>
		F3 = 53,
		/// <summary>F# in octave 3.</summary>
		FSharp3 = 54,
		/// <summary>G in octave 3.</summary>
		G3 = 55,
		/// <summary>G# in octave 3.</summary>
		GSharp3 = 56,
		/// <summary>A in octave 3.</summary>
		A3 = 57,
		/// <summary>A# in octave 3.</summary>
		ASharp3 = 58,
		/// <summary>B in octave 3.</summary>
		B3 = 59,

		/// <summary>C in octave 4, also known as Middle C.</summary>
		C4 = 60,
		/// <summary>C# in octave 4.</summary>
		CSharp4 = 61,
		/// <summary>D in octave 4.</summary>
		D4 = 62,
		/// <summary>D# in octave 4.</summary>
		DSharp4 = 63,
		/// <summary>E in octave 4.</summary>
		E4 = 64,
		/// <summary>F in octave 4.</summary>
		F4 = 65,
		/// <summary>F# in octave 4.</summary>
		FSharp4 = 66,
		/// <summary>G in octave 4.</summary>
		G4 = 67,
		/// <summary>G# in octave 4.</summary>
		GSharp4 = 68,
		/// <summary>A in octave 4.</summary>
		A4 = 69,
		/// <summary>A# in octave 4.</summary>
		ASharp4 = 70,
		/// <summary>B in octave 4.</summary>
		B4 = 71,

		/// <summary>C in octave 5.</summary>
		C5 = 72,
		/// <summary>C# in octave 5.</summary>
		CSharp5 = 73,
		/// <summary>D in octave 5.</summary>
		D5 = 74,
		/// <summary>D# in octave 5.</summary>
		DSharp5 = 75,
		/// <summary>E in octave 5.</summary>
		E5 = 76,
		/// <summary>F in octave 5.</summary>
		F5 = 77,
		/// <summary>F# in octave 5.</summary>
		FSharp5 = 78,
		/// <summary>G in octave 5.</summary>
		G5 = 79,
		/// <summary>G# in octave 5.</summary>
		GSharp5 = 80,
		/// <summary>A in octave 5.</summary>
		A5 = 81,
		/// <summary>A# in octave 5.</summary>
		ASharp5 = 82,
		/// <summary>B in octave 5.</summary>
		B5 = 83,

		/// <summary>C in octave 6.</summary>
		C6 = 84,
		/// <summary>C# in octave 6.</summary>
		CSharp6 = 85,
		/// <summary>D in octave 6.</summary>
		D6 = 86,
		/// <summary>D# in octave 6.</summary>
		DSharp6 = 87,
		/// <summary>E in octave 6.</summary>
		E6 = 88,
		/// <summary>F in octave 6.</summary>
		F6 = 89,
		/// <summary>F# in octave 6.</summary>
		FSharp6 = 90,
		/// <summary>G in octave 6.</summary>
		G6 = 91,
		/// <summary>G# in octave 6.</summary>
		GSharp6 = 92,
		/// <summary>A in octave 6.</summary>
		A6 = 93,
		/// <summary>A# in octave 6.</summary>
		ASharp6 = 94,
		/// <summary>B in octave 6.</summary>
		B6 = 95,

		/// <summary>C in octave 7.</summary>
		C7 = 96,
		/// <summary>C# in octave 7.</summary>
		CSharp7 = 97,
		/// <summary>D in octave 7.</summary>
		D7 = 98,
		/// <summary>D# in octave 7.</summary>
		DSharp7 = 99,
		/// <summary>E in octave 7.</summary>
		E7 = 100,
		/// <summary>F in octave 7.</summary>
		F7 = 101,
		/// <summary>F# in octave 7.</summary>
		FSharp7 = 102,
		/// <summary>G in octave 7.</summary>
		G7 = 103,
		/// <summary>G# in octave 7.</summary>
		GSharp7 = 104,
		/// <summary>A in octave 7.</summary>
		A7 = 105,
		/// <summary>A# in octave 7.</summary>
		ASharp7 = 106,
		/// <summary>B in octave 7.</summary>
		B7 = 107,

		/// <summary>C in octave 8, usually the highest key on an 88-key keyboard.</summary>
		C8 = 108,
		/// <summary>C# in octave 8.</summary>
		CSharp8 = 109,
		/// <summary>D in octave 8.</summary>
		D8 = 110,
		/// <summary>D# in octave 8.</summary>
		DSharp8 = 111,
		/// <summary>E in octave 8.</summary>
		E8 = 112,
		/// <summary>F in octave 8.</summary>
		F8 = 113,
		/// <summary>F# in octave 8.</summary>
		FSharp8 = 114,
		/// <summary>G in octave 8.</summary>
		G8 = 115,
		/// <summary>G# in octave 8.</summary>
		GSharp8 = 116,
		/// <summary>A in octave 8.</summary>
		A8 = 117,
		/// <summary>A# in octave 8.</summary>
		ASharp8 = 118,
		/// <summary>B in octave 8.</summary>
		B8 = 119,

		/// <summary>C in octave 9.</summary>
		C9 = 120,
		/// <summary>C# in octave 9.</summary>
		CSharp9 = 121,
		/// <summary>D in octave 9.</summary>
		D9 = 122,
		/// <summary>D# in octave 9.</summary>
		DSharp9 = 123,
		/// <summary>E in octave 9.</summary>
		E9 = 124,
		/// <summary>F in octave 9.</summary>
		F9 = 125,
		/// <summary>F# in octave 9.</summary>
		FSharp9 = 126,
		/// <summary>G in octave 9.</summary>
		G9 = 127
	}

	struct Duration
	{
		public const string Quarter = "q";
		public const string Half = "h";
		public const string Whole = "w";
		public const string Eight = "e";
		public const string Six = "x";
	}

	float[] semitonelist;
	private int count;

	/// <summary>
	/// Structure to hold pre-computed animation data.
	/// </summary>
	private struct VertexAnim
	{
		public float angleRange;
		public float angle;
		public float speed;
	}
	// 


	string ParseNote(Pitch pitch, string dur, out float vert)
	{
		bool isacc = false;
		vert = ParsePitch2((int)pitch, out isacc);
		if (dur.Equals(Duration.Whole)){
			vert += TINY_OFFSET;
		}

		if (!isacc)
		{
			// vert += TINY_OFFSET;
			return "#" + dur;
		}
		else
		{
			return dur;
		}
	}

	void BuildTextAndArray(Pitch pitch, string dur, List<float> array, System.Text.StringBuilder builder)
	{
		float vert;
		string text = ParseNote(pitch, dur, out vert);
		builder.Append(text);
		for (int i = 0; i < text.Length; i++)
		{
			array.Add(vert);
		}
	}
	float ParsePitch2(int input, out bool accidental)
	{
		// 60 - 65		

		int n = input % 12; //0
		int oct = (int)input / 12 * 7; //35
		int offset = 38;
		accidental = false;

		if (n < 5)
		{
			accidental = System.Math.Abs(n / 2f % 1) <= (System.Double.Epsilon * 100);
			//Debug.Log(accidental);
			n = Mathf.FloorToInt(n / 2); // 0
			n += oct; //35
			n -= offset;
			Debug.Log(n);
			return n * SEMITONE_DIST;
		}

		n++;
		accidental = System.Math.Abs(n / 2f % 1) <= (System.Double.Epsilon * 100);
		n /= 2;
		n += oct;
		int d = n - offset;
		Debug.Log(d);
		return d * SEMITONE_DIST;
	}

	float ParsePitch(int pitchmidi)
	{
		int rest = (int)(pitchmidi - 65);
		//  if (rest == 0) return rest * SEMITONE_DIST; // fundamental 

		int halfoctave = rest / 7;// half octave 
		int octave = rest / 14;//  octave 

		//  if (rest % 7 == 0) {
		// rest++;
		// return rest / 2 * SEMITONE_DIST;
		//  } 

		//  else if (rest % 12 == 0 )
		//  {
		// rest += 2;
		// return rest / 2 * SEMITONE_DIST;
		//  }

		//  if (rest % 14 == 0)
		//  {

		//  }

		if (rest % 12 == 0) rest++;

		rest = (rest % 7) + (halfoctave * 8);

		// if (rest % 16)
		Debug.Log("ho " + halfoctave + " rest " + rest);

		// rest = (rest % 14) + (octave * 15);
		if (rest < 0) rest -= 2;

		rest /= 2;

		return rest * SEMITONE_DIST;
	}
	// Use this for initialization
	void Start()
	{
		mTextMeshPro = GetComponent<TextMeshPro>();
		SEMITONE_DIST = mTextMeshPro.fontSize / 40.0f / 2;
		TINY_OFFSET = mTextMeshPro.fontSize / 40.0f / 3;

		// semitonelist = new float[] {ParsePitch2((int)Pitch.F4, out acc)};
		// StartCoroutine(WarpText());

		mTextMeshPro.text = " q";
		// dk = FindObjectOfType<DKThrow>();
		// bool acc;
		// semitonelist[0] = ;
/*
		semitonelist = new float[]
		{

			// ParsePitch2((int)Pitch.C4),
			// ParsePitch2((int)Pitch.D4),
			// ParsePitch2((int)Pitch.E4),
			// ParsePitch2((int)Pitch.F4),
			// ParsePitch2((int)Pitch.G4),
			// ParsePitch2((int)Pitch.A4),
			// ParsePitch2((int)Pitch.B4),
			// ParsePitch2((int)Pitch.C5),
			// ParsePitch2((int)Pitch.D5),
			// ParsePitch2((int)Pitch.E5),
			// ParsePitch2((int)Pitch.F5),
			// ParsePitch2((int)Pitch.G5),
			// ParsePitch2((int)Pitch.A5),
			// ParsePitch2((int)Pitch.B5),
		};

		List<float> temp = new List<float>();

		
		// mTextMeshPro.font = Resources.Load("../Fonts/Maestro SDF", typeof(TMP_FontAsset)) as TMP_FontAsset;
		// mTextMeshPro.text = "qqqqqq";
		System.Text.StringBuilder mBuilder = new System.Text.StringBuilder();
		BuildTextAndArray(Pitch.CSharp4, Duration.Half, temp, mBuilder);
		BuildTextAndArray(Pitch.DSharp4, Duration.Whole, temp, mBuilder);
		BuildTextAndArray(Pitch.GSharp4, Duration.Eight, temp, mBuilder);
		BuildTextAndArray(Pitch.FSharp4, Duration.Quarter, temp, mBuilder);

		semitonelist = temp.ToArray();
		foreach (var item in semitonelist)
		{
			//Debug.Log(item);
		}
		mTextMeshPro.text = mBuilder.ToString();
		Debug.Log(mTextMeshPro.text);

*/
	}

	void Update()
	{
		// if (Input.GetKeyUp(KeyCode.Q))
		// {
		// 	PushNoteVoid(70);
		// }
		// 	//BuildTextAndArray(count++, Duration.Quarter, mBuilder);
		// 	// mTextMeshPro.text = mBuilder.ToString();
		// 	bool acc;
		// 	semitonelist[0] = ParsePitch2((int)Pitch.C4, out acc);
			
		// 	// count++;
		// }
		// if (Input.GetKey(KeyCode.W))
		// {
		// 	//BuildTextAndArray(count++, Duration.Quarter, mBuilder);
		// 	// mTextMeshPro.text = mBuilder.ToString();
		// 	bool acc;
		// 	semitonelist[0] = ParsePitch2((int)Pitch.D4, out acc);
			
		// 	// count++;
		// }

		// if (Input.GetKey(KeyCode.E))
		// {
		// 	//BuildTextAndArray(count++, Duration.Quarter, mBuilder);
		// 	// mTextMeshPro.text = mBuilder.ToString();
		// 	bool acc;
		// 	semitonelist[0] = ParsePitch2((int)Pitch.E4, out acc);
			
		// 	// count++;
		// }

		// WarpText();


		// poll the most recent note


	}


	public void PushNoteVoid(int n)
	{
		bool acc;
		float note = ParsePitch2(n, out acc);
		if (acc) {
			mTextMeshPro.text = " q";
			semitonelist = new float[]{note, note};
		} else {
			mTextMeshPro.text = "#q";
			semitonelist = new float[]{note, note};
		}
		WarpText();
		// StartCoroutine(WarpText());
	}

	
	void OnEnable()
	{
		// Subscribe to event fired when text object has been regenerated.
		//TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
	}

	void OnDisable()
	{
		//TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
	}
	void RestoreCachedVertexAttributes(int index)
	{
		if (index == -1 || index > mTextMeshPro.textInfo.characterCount - 1) return;

		// Get the index of the material / sub text object used by this character.
		int materialIndex = mTextMeshPro.textInfo.characterInfo[index].materialReferenceIndex;

		// Get the index of the first vertex of the selected character.
		int vertexIndex = mTextMeshPro.textInfo.characterInfo[index].vertexIndex;

		// Restore Vertices
		// Get a reference to the cached / original vertices.
		Vector3[] src_vertices = m_cachedMeshInfoVertexData[materialIndex].vertices;

		// Get a reference to the vertices that we need to replace.
		Vector3[] dst_vertices = mTextMeshPro.textInfo.meshInfo[materialIndex].vertices;

		// Restore / Copy vertices from source to destination
		dst_vertices[vertexIndex + 0] = src_vertices[vertexIndex + 0];
		dst_vertices[vertexIndex + 1] = src_vertices[vertexIndex + 1];
		dst_vertices[vertexIndex + 2] = src_vertices[vertexIndex + 2];
		dst_vertices[vertexIndex + 3] = src_vertices[vertexIndex + 3];

		// Restore Vertex Colors
		// Get a reference to the vertex colors we need to replace.
		Color32[] dst_colors = mTextMeshPro.textInfo.meshInfo[materialIndex].colors32;

		// Get a reference to the cached / original vertex colors.
		Color32[] src_colors = m_cachedMeshInfoVertexData[materialIndex].colors32;

		// Copy the vertex colors from source to destination.
		dst_colors[vertexIndex + 0] = src_colors[vertexIndex + 0];
		dst_colors[vertexIndex + 1] = src_colors[vertexIndex + 1];
		dst_colors[vertexIndex + 2] = src_colors[vertexIndex + 2];
		dst_colors[vertexIndex + 3] = src_colors[vertexIndex + 3];

		// Restore UV0S
		// UVS0
		Vector2[] src_uv0s = m_cachedMeshInfoVertexData[materialIndex].uvs0;
		Vector2[] dst_uv0s = mTextMeshPro.textInfo.meshInfo[materialIndex].uvs0;
		dst_uv0s[vertexIndex + 0] = src_uv0s[vertexIndex + 0];
		dst_uv0s[vertexIndex + 1] = src_uv0s[vertexIndex + 1];
		dst_uv0s[vertexIndex + 2] = src_uv0s[vertexIndex + 2];
		dst_uv0s[vertexIndex + 3] = src_uv0s[vertexIndex + 3];

		// UVS2
		Vector2[] src_uv2s = m_cachedMeshInfoVertexData[materialIndex].uvs2;
		Vector2[] dst_uv2s = mTextMeshPro.textInfo.meshInfo[materialIndex].uvs2;
		dst_uv2s[vertexIndex + 0] = src_uv2s[vertexIndex + 0];
		dst_uv2s[vertexIndex + 1] = src_uv2s[vertexIndex + 1];
		dst_uv2s[vertexIndex + 2] = src_uv2s[vertexIndex + 2];
		dst_uv2s[vertexIndex + 3] = src_uv2s[vertexIndex + 3];


		// Restore last vertex attribute as we swapped it as well
		int lastIndex = (src_vertices.Length / 4 - 1) * 4;

		// Vertices
		dst_vertices[lastIndex + 0] = src_vertices[lastIndex + 0];
		dst_vertices[lastIndex + 1] = src_vertices[lastIndex + 1];
		dst_vertices[lastIndex + 2] = src_vertices[lastIndex + 2];
		dst_vertices[lastIndex + 3] = src_vertices[lastIndex + 3];

		// Vertex Colors
		src_colors = m_cachedMeshInfoVertexData[materialIndex].colors32;
		dst_colors = mTextMeshPro.textInfo.meshInfo[materialIndex].colors32;
		dst_colors[lastIndex + 0] = src_colors[lastIndex + 0];
		dst_colors[lastIndex + 1] = src_colors[lastIndex + 1];
		dst_colors[lastIndex + 2] = src_colors[lastIndex + 2];
		dst_colors[lastIndex + 3] = src_colors[lastIndex + 3];

		// UVS0
		src_uv0s = m_cachedMeshInfoVertexData[materialIndex].uvs0;
		dst_uv0s = mTextMeshPro.textInfo.meshInfo[materialIndex].uvs0;
		dst_uv0s[lastIndex + 0] = src_uv0s[lastIndex + 0];
		dst_uv0s[lastIndex + 1] = src_uv0s[lastIndex + 1];
		dst_uv0s[lastIndex + 2] = src_uv0s[lastIndex + 2];
		dst_uv0s[lastIndex + 3] = src_uv0s[lastIndex + 3];

		// UVS2
		src_uv2s = m_cachedMeshInfoVertexData[materialIndex].uvs2;
		dst_uv2s = mTextMeshPro.textInfo.meshInfo[materialIndex].uvs2;
		dst_uv2s[lastIndex + 0] = src_uv2s[lastIndex + 0];
		dst_uv2s[lastIndex + 1] = src_uv2s[lastIndex + 1];
		dst_uv2s[lastIndex + 2] = src_uv2s[lastIndex + 2];
		dst_uv2s[lastIndex + 3] = src_uv2s[lastIndex + 3];

		// Need to update the appropriate 
		mTextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
	}
	void OnTriggerEnter2D(Collider2D other) {
		// Debug.Log("collide");
	}
	void OnCollisionEnter2D(Collision2D coll) {
			Debug.Log("collide and send");
			OSCSender.SendMessage(OSCSender.PDClient, OSCSender.hitCmd + GATE_NUMBER, new List<float>(){Init.gateNotes[GATE_NUMBER].x, Init.gateNotes[GATE_NUMBER].y} );
	}
	
	// void LateUpdate()
	// {
	// 	m_isHoveringObject = false;

	// 	//Debug.Log("position " + dk.barrel1.transform.position);

	// 	if (TMP_TextUtilities.IsIntersectingRectTransform(mTextMeshPro.rectTransform, Input.mousePosition, Camera.main))
	// 	{

	// 			m_isHoveringObject = true;
	// 	}

	// 	if (m_isHoveringObject)
	// 	{
	// 		// Check if Mouse Intersects any of the characters. If so, assign a random color.
	// 		#region Handle Character Selection
	// 		int charIndex = TMP_TextUtilities.FindIntersectingCharacter(mTextMeshPro, Input.mousePosition, Camera.main, true);

	// 		Debug.Log("isHovering " + charIndex);

	// 		// Undo Swap and Vertex Attribute changes.
	// 		if (charIndex == -1 || charIndex != m_lastCharIndex)
	// 		{
	// 			RestoreCachedVertexAttributes(m_lastCharIndex);
	// 			m_lastCharIndex = -1;
	// 		}

	// 		if (charIndex != -1 && charIndex != m_lastCharIndex && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
	// 		{
				
				
	// 			// OSCSender.SendMessage(OSCSender.PDClient, OSCSender.hitCmd, new List<float>{60.0f, 100.0f} );
	// 			// OSCSender.SendMessage(OSCSender.PDClient, OSCSender.hitCmd, new List<float>{Init.receivedPitch, Init.receivedDur} );

	// 			m_lastCharIndex = charIndex;

	// 			// Get the index of the material / sub text object used by this character.
	// 			int materialIndex = mTextMeshPro.textInfo.characterInfo[charIndex].materialReferenceIndex;

	// 			// Get the index of the first vertex of the selected character.
	// 			int vertexIndex = mTextMeshPro.textInfo.characterInfo[charIndex].vertexIndex;

	// 			// Get a reference to the vertices array.
	// 			Vector3[] vertices = mTextMeshPro.textInfo.meshInfo[materialIndex].vertices;

	// 			// Determine the center point of the character.
	// 			Vector2 charMidBasline = (vertices[vertexIndex + 0] + vertices[vertexIndex + 2]) / 2;

	// 			// Need to translate all 4 vertices of the character to aligned with middle of character / baseline.
	// 			// This is needed so the matrix TRS is applied at the origin for each character.
	// 			Vector3 offset = charMidBasline;

	// 			// Translate the character to the middle baseline.
	// 			vertices[vertexIndex + 0] = vertices[vertexIndex + 0] - offset;
	// 			vertices[vertexIndex + 1] = vertices[vertexIndex + 1] - offset;
	// 			vertices[vertexIndex + 2] = vertices[vertexIndex + 2] - offset;
	// 			vertices[vertexIndex + 3] = vertices[vertexIndex + 3] - offset;

	// 			float zoomFactor = 1.5f;

	// 			// Setup the Matrix for the scale change.
	// 			m_matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * zoomFactor);

	// 			// Apply Matrix operation on the given character.
	// 			vertices[vertexIndex + 0] = m_matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
	// 			vertices[vertexIndex + 1] = m_matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
	// 			vertices[vertexIndex + 2] = m_matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
	// 			vertices[vertexIndex + 3] = m_matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);

	// 			// Translate the character back to its original position.
	// 			vertices[vertexIndex + 0] = vertices[vertexIndex + 0] + offset;
	// 			vertices[vertexIndex + 1] = vertices[vertexIndex + 1] + offset;
	// 			vertices[vertexIndex + 2] = vertices[vertexIndex + 2] + offset;
	// 			vertices[vertexIndex + 3] = vertices[vertexIndex + 3] + offset;

	// 			// Change Vertex Colors of the highlighted character
	// 			Color32 c = new Color32(255, 255, 192, 255);

	// 			// Get a reference to the vertex color
	// 			Color32[] vertexColors = mTextMeshPro.textInfo.meshInfo[materialIndex].colors32;

	// 			vertexColors[vertexIndex + 0] = c;
	// 			vertexColors[vertexIndex + 1] = c;
	// 			vertexColors[vertexIndex + 2] = c;
	// 			vertexColors[vertexIndex + 3] = c;


	// 			// Get a reference to the meshInfo of the selected character.
	// 			TMP_MeshInfo meshInfo = mTextMeshPro.textInfo.meshInfo[materialIndex];

	// 			// Get the index of the last character's vertex attributes.
	// 			int lastVertexIndex = vertices.Length - 4;

	// 			// Swap the current character's vertex attributes with those of the last element in the vertex attribute arrays.
	// 			// We do this to make sure this character is rendered last and over other characters.
	// 			meshInfo.SwapVertexData(vertexIndex, lastVertexIndex);

	// 			// Need to update the appropriate 
	// 			mTextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
	// 		}
	// 		#endregion

	// 	}
	// 	else
	// 	{
	// 		// Restore any character that may have been modified
	// 		if (m_lastCharIndex != -1)
	// 		{
	// 			RestoreCachedVertexAttributes(m_lastCharIndex);
	// 			m_lastCharIndex = -1;
	// 		}
	// 	}

	// }




	void ON_TEXT_CHANGED(UnityEngine.Object obj)
	{
		if (obj == mTextMeshPro)
			hasTextChanged = true;
			m_cachedMeshInfoVertexData = mTextMeshPro.textInfo.CopyMeshInfoVertexData();
	}
	void WarpText()
	{
		// We force an update of the text object since it would only be updated at the end of the frame. Ie. before this code is executed on the first frame.
		// Alternatively, we could yield and wait until the end of the frame when the text object will be generated.
		mTextMeshPro.ForceMeshUpdate();

		TMP_TextInfo textInfo = mTextMeshPro.textInfo;

		Matrix4x4 matrix;

		int loopCount = 0;
		hasTextChanged = true;

		// Cache the vertex data of the text object as the Jitter FX is applied to the original position of the characters.
		TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

		m_cachedMeshInfoVertexData = mTextMeshPro.textInfo.CopyMeshInfoVertexData();

		// while (true)
		// {
		// Get new copy of vertex data if the text has changed.
		if (hasTextChanged)
		{
			// Update the copy of the vertex data for the text object.
			cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

			hasTextChanged = false;
		}

		int characterCount = textInfo.characterCount;

		// If No Characters then just yield and wait for some text to be added
		if (characterCount == 0)
		{
			Debug.Log("no chars?");
			return;
			// yield return new WaitForSeconds(0.25f);
			// continue;
		}


		for (int i = 0; i < characterCount; i++)
		{
			TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

			// Skip characters that are not visible and thus have no geometry to manipulate.
			if (!charInfo.isVisible){
				Debug.Log("not visible?");
				continue;
			}

			// Retrieve the pre-computed animation data for the given character.

			// Get the index of the material used by the current character.
			int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

			// Get the index of the first vertex used by this text element.
			int vertexIndex = textInfo.characterInfo[i].vertexIndex;

			// textInfo.characterInfo[i].bottomLeft = textInfo.characterInfo[i].bottomLeft * 10;


			// Get the cached vertices of the mesh used by this text element (character or sprite).
			Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;

			// Determine the center point of each character at the baseline.
			//Vector2 charMidBasline = new Vector2((sourceVertices[vertexIndex + 0].x + sourceVertices[vertexIndex + 2].x) / 2, charInfo.baseLine);
			// Determine the center point of each character.
			Vector2 charMidBasline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

			// Need to translate all 4 vertices of each quad to aligned with middle of character / baseline.
			// This is needed so the matrix TRS is applied at the origin for each character.
			Vector3 offset = charMidBasline;

			Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

			destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
			destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
			destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
			destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;

			// vertAnim.angle = Mathf.SmoothStep(-vertAnim.angleRange, vertAnim.angleRange, Mathf.PingPong(loopCount / 25f * vertAnim.speed, 1f));
			// Vector3 jitterOffset = new Vector3(Random.Range(-.25f, .25f), Random.Range(-.25f, .25f), 0);

			// matrix = Matrix4x4.TRS(jitterOffset * CurveScale, Quaternion.Euler(0, 0, Random.Range(-5f, 5f) * AngleMultiplier), Vector3.one);

			// matrix for tranlation of x coord only
			matrix = Matrix4x4.TRS(new Vector3(0, semitonelist[i], 0), Quaternion.identity, Vector3.one);

			destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
			destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
			destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
			destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);


			
			// destinationVertices[vertexIndex + 0] = destinationVertices[vertexIndex + 0];
			// destinationVertices[vertexIndex + 1] = destinationVertices[vertexIndex + 1];
			// destinationVertices[vertexIndex + 2] = destinationVertices[vertexIndex + 2];
			// destinationVertices[vertexIndex + 3] = destinationVertices[vertexIndex + 3];

/*
			charInfo.bottomLeft = charInfo.vertex_BL.position = destinationVertices[vertexIndex + 0];
			charInfo.bottomRight = charInfo.vertex_BR.position = destinationVertices[vertexIndex + 2];
			charInfo.topLeft = charInfo.vertex_TL.position = destinationVertices[vertexIndex + 1];
			charInfo.topRight = charInfo.vertex_TR.position = destinationVertices[vertexIndex + 3];
*/
			destinationVertices[vertexIndex + 0] += offset;
			destinationVertices[vertexIndex + 1] += offset;
			destinationVertices[vertexIndex + 2] += offset;
			destinationVertices[vertexIndex + 3] += offset;




		}

		// Push changes into meshes
		for (int i = 0; i < textInfo.meshInfo.Length; i++)
		{
			textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
			mTextMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
		}

		loopCount += 1;

		// yield return new WaitForSeconds(0.1f);
		// }
	}


	public void OnPointerEnter(PointerEventData eventData)
	{
		Debug.Log("OnPointerEnter()");
		m_isHoveringObject = true;
	}


	public void OnPointerExit(PointerEventData eventData)
	{
		Debug.Log("OnPointerExit()");
		m_isHoveringObject = false;
	}

	public void PushNote(int idx, Vector2 note)
	{
		Debug.Log("PushNote() " + idx);
		if (GATE_NUMBER == idx){
			PushNoteVoid((int)note.x);
		}
		// bool acc;
		// float note = ParsePitch2(n, out acc);
		// if (acc) {
		// 	mTextMeshPro.text = " q";
		// 	semitonelist = new float[]{note, note};
		// } else {
		// 	mTextMeshPro.text = "#q";
		// 	semitonelist = new float[]{note, note};
		// }
		// WarpText();
	}
}
