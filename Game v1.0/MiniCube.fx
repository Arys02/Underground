float4x4 World;
float4x4 View;
float4x4 Projection;
//const static int nblights = 1;

texture DiffuseTexture;
float4 CameraPos;
float4 AmbientLightColor;

float4 LightPosition[nblights];
float4 LightDiffuseColor[nblights]; // intensity multiplier
float LightDistanceSquared[nblights];
float luminosity;

bool Sepia = false;
sampler2D s_2D;

sampler S0 = sampler_state
{
    Texture = (DiffuseTexture);
    MinFilter = ANISOTROPIC; //LINEAR;
    MagFilter = ANISOTROPIC; //LINEAR;
    MipFilter = LINEAR;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;  
	float2 tex : TEXCOORD0;
	float4 col : COLOR0;  
    float4 Normal : NORMAL0;
};
struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 tex : TEXCOORD0;
    float4 Color : COLOR0;
	float4 Normal : TEXCOORD1;
	float4 WorldPos : TEXCOORD2;
};


VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	// "Multiplication will be done in the pre-shader - so no cost per vertex"
	float4 posWorld = mul(input.Position, World);
	float4x4 viewprojection = mul(View, Projection);
	output.Position = mul(posWorld, viewprojection);
	output.tex = input.tex;

	// Passing information on to do both specular AND diffuse calculation in pixel shader
	output.Normal = mul(input.Normal, (float4x4)World);
	output.WorldPos = posWorld;
	output.Color = input.col;
	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 texel = tex2D(s_2D, input.tex);
	float4 lightDir[nblights];
	float4 finalLights[nblights];
	int i;

	for (i=0;i<nblights;i++)
	{
		lightDir[i] = normalize(input.WorldPos - LightPosition[i]);
		finalLights[i] = LightDiffuseColor[i] * saturate(dot(input.Normal, -lightDir[i])) *
			LightDistanceSquared[i] / max(dot(LightPosition[i] - input.WorldPos, LightPosition[i] - input.WorldPos),1) * 0.1;
	}
	for (i=1;i<nblights;i++)
	{
		finalLights[0] += finalLights[i];
	}
	//finalLights[0] = min(finalLights[0],float4(0.6,0.6,0.6,1));
	float4 finalPixel = saturate(texel.xyzw * (AmbientLightColor + finalLights[0]) * input.Color) * luminosity;
	finalPixel.w = texel.w;
	return min(finalPixel,saturate(texel+input.Color));

}

technique Main {
	pass P0 {
        AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
		AlphaTestEnable = True;
		AlphaFunc = Greater;
		VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader  = compile ps_2_0 PixelShaderFunction();
	}
}
