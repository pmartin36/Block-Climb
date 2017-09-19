using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class OutlineComposite : MonoBehaviour
{
	[Range (0, 10)]
	public float Intensity = 2;

	private Material _compositeMat;

	void OnEnable()
	{
		_compositeMat = new Material(Shader.Find("Hidden/OutlineComposite"));
    }

	void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		_compositeMat.SetFloat("_Intensity", Intensity);
        Graphics.Blit(src, dst, _compositeMat);
	}
}
