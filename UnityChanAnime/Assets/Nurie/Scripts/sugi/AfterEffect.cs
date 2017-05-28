using UnityEngine;
using System.Collections;

public class AfterEffect : MonoBehaviour {
	public Shader blurShader;
	[Range(1,4)]
	public int
		downsample = 2,
		blurInterations = 2;
	public float
		blurSize = 3f;
	
	public Material
		effect;
	
	Material blurMaterial;
	RenderTexture blurTex;
	
	void OnRenderImage(RenderTexture source, RenderTexture destination){
		if(blurMaterial == null)
			blurMaterial = new Material(blurShader);
		
		float widthMod = 1f / (1f * (1<<downsample));
		int
			rtW = source.width >> downsample,
			rtH = source.height >> downsample;
		
		if(blurTex == null)
			blurTex = new RenderTexture(rtW, rtH, source.depth, source.format);
		
		RenderTexture rt = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
		rt.filterMode = FilterMode.Bilinear;
		
		blurMaterial.SetVector("_Parameter", new Vector4(blurSize * widthMod, -blurSize * widthMod, 0.0f, 0.0f));
		Graphics.Blit(source, rt, blurMaterial, 0);
		
		for(int i = 0; i < blurInterations; i++){
			float iterationOffs = (float)i;
			blurMaterial.SetVector("_Parameter", new Vector4(blurSize * widthMod + iterationOffs, -blurSize * widthMod - iterationOffs, 0, 0));
			
			RenderTexture rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
			rt2.filterMode = FilterMode.Bilinear;
			Graphics.Blit(rt, rt2, blurMaterial, 1);
			RenderTexture.ReleaseTemporary(rt);
			rt = rt2;
			
			rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
			rt2.filterMode = FilterMode.Bilinear;
			Graphics.Blit(rt, rt2, blurMaterial, 2);
			RenderTexture.ReleaseTemporary(rt);
			rt = rt2;
		}
		
		Graphics.Blit(rt, blurTex);
		effect.SetTexture("_BlurTex", blurTex);
		Graphics.Blit(source, destination, effect);
		
		RenderTexture.ReleaseTemporary(rt);
	}
}