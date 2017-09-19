using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class OutlinePrePass : MonoBehaviour
{
	private static RenderTexture PrePass;
	private static RenderTexture Blurred;

	private Material _blurMat;
	private Material _angleMat;

	private float Angle = 0.01f;

	public int numIterations = 5;
	public float blurLength = 1.5f;

	public bool isAngleEffect = false;
	Camera c;

	void OnEnable()
	{
        PrePass = new RenderTexture(Screen.width, Screen.height, 24);
		PrePass.antiAliasing = QualitySettings.antiAliasing;
		Blurred = new RenderTexture(Screen.width >> 1, Screen.height >> 1, 0);

		c = GetComponent<Camera>();
		SetReplacementShaders();

		Shader.SetGlobalTexture("_OutlineBlurredTex", Blurred);

		_blurMat = new Material(Shader.Find("Hidden/BlockBlur"));
		_blurMat.SetVector("_BlurSize", new Vector2(Blurred.texelSize.x * blurLength, Blurred.texelSize.y * blurLength));
	}

	public void SetReplacementShaders() {
		if(isAngleEffect) {
			c.SetReplacementShader(Shader.Find("Hidden/AngleReplace"), "Outlinable");
		}
		else {
			c.SetReplacementShader(Shader.Find("Hidden/OutlineReplace"), "Outlinable");
			Shader.SetGlobalTexture("_OutlinePrePassTex", PrePass);
		}
		c.targetTexture = PrePass;
	}

	private Vector2 AngleToVector(float angle)
	{
		return new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
	}

	private void PrintFloatArray(float[] a)
	{
		Debug.Log(string.Format("X: {0}   Y: {1}", a[0], a[1]));
	}

	private void Update()
	{
		c.orthographicSize = Camera.main.orthographicSize;
	}

	void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		Graphics.Blit(src, dst);

		Graphics.SetRenderTarget(Blurred);
		GL.Clear(false, true, Color.clear);

		Graphics.Blit(src, Blurred);
		
		for (int i = 0; i < numIterations; i++)
		{
			var temp = RenderTexture.GetTemporary(Blurred.width, Blurred.height);
			Graphics.Blit(Blurred, temp, _blurMat, 0);
			Graphics.Blit(temp, Blurred, _blurMat, 1);
			RenderTexture.ReleaseTemporary(temp);
		}
	}
}
