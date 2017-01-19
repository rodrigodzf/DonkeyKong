using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StringCreator : MonoBehaviour
{

	public Font font;
	private TextGenerator mGenerator;
	// public Material mMaterial;

	void Start()
	{
		TextGenerationSettings settings = new TextGenerationSettings();
		settings.color = Color.red;
		settings.generationExtents = new Vector2(100, 100);
		settings.pivot = new Vector2(0.5f, 0.5f); ;
		settings.richText = true;
		settings.font = font;
		settings.fontSize = 14;
		settings.fontStyle = FontStyle.Normal;
		settings.verticalOverflow = VerticalWrapMode.Overflow;
		settings.horizontalOverflow = HorizontalWrapMode.Wrap;
		settings.lineSpacing = 1;
		settings.generateOutOfBounds = true;
		settings.resizeTextForBestFit = false;
		settings.scaleFactor = 1f;

		mGenerator = new TextGenerator();
		mGenerator.Populate("xxxxxxxxxx", settings);


		// GetComponent<MeshFilter>().mesh = TextGenToMesh(mGenerator);
		// GetComponent<MeshRenderer>().sharedMaterial = settings.font.material;
		GetComponent<CanvasRenderer>().SetMaterial(settings.font.material, null);
		GetComponent<CanvasRenderer>().SetMesh(TextGenToMesh(mGenerator));

		Debug.Log("I generated: " + mGenerator.vertexCount + " verts!");
	}

	public Mesh TextGenToMesh(TextGenerator generator, Mesh mesh = null)
	{
		if (mesh == null) mesh = new Mesh();

		int vertSize = generator.vertexCount;
		Vector3[] tempVerts = new Vector3[vertSize];
		Color32[] tempColours = new Color32[vertSize];
		Vector2[] tempUvs = new Vector2[vertSize];
		IList<UIVertex> generatorVerts = generator.verts;
		for (int i = 0; i < vertSize; ++i)
		{
			tempVerts[i] = generatorVerts[i].position;
			tempColours[i] = generatorVerts[i].color;
			tempUvs[i] = generatorVerts[i].uv0;
		}
		mesh.vertices = tempVerts;
		mesh.colors32 = tempColours;
		mesh.uv = tempUvs;

		int characterCount = vertSize / 4;
		int[] tempIndices = new int[characterCount * 6];
		for (int i = 0; i < characterCount; ++i)
		{
			int vertIndexStart = i * 4;
			int trianglesIndexStart = i * 6;
			tempIndices[trianglesIndexStart++] = vertIndexStart;
			tempIndices[trianglesIndexStart++] = vertIndexStart + 1;
			tempIndices[trianglesIndexStart++] = vertIndexStart + 2;
			tempIndices[trianglesIndexStart++] = vertIndexStart;
			tempIndices[trianglesIndexStart++] = vertIndexStart + 2;
			tempIndices[trianglesIndexStart] = vertIndexStart + 3;
		}
		mesh.triangles = tempIndices;
		//TODO: setBounds manually
		mesh.RecalculateBounds();
		return mesh;
	}
}
