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
bool bump_mapping = true;
float4x4 transformMatrix;
float percent_Negatif = 0;
sampler2D s_2D[2];

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
	float4 texel = tex2D(s_2D[0], input.tex);
	float coef_bump;
	if (bump_mapping)
	{
		float4 normaltex = tex2D(s_2D[1], input.tex);
		float4 bump = normalize(normalize(normaltex * 2.0 - 1.0));
		bump.w = 1;
		coef_bump = max(saturate(dot(input.Normal, bump)), 0.0);
	}
	else coef_bump = 1;
	
	float4 lightDir[nblights];
	float4 finalLights[nblights];
	int i;

	for (i=0;i<nblights;i++)
	{
		lightDir[i] = normalize(input.WorldPos - LightPosition[i]);
		finalLights[i] = ((LightDiffuseColor[i] * 2 - float4(1,1,1,1)) * percent_Negatif / 2
			+ LightDiffuseColor[i] * saturate(dot(input.Normal, -lightDir[i]))) * coef_bump *
			LightDistanceSquared[i] / pow(max(dot(LightPosition[i] - input.WorldPos, LightPosition[i] - input.WorldPos),1),1) * 0.1;
	}
	for (i=1;i<nblights;i++)
	{
		finalLights[0] += finalLights[i];
	}
	//finalLights[0] = min(finalLights[0],float4(0.6,0.6,0.6,1));

	float4 pixel = saturate(texel * input.Color);
	pixel = (pixel * 2 - float4(1,1,1,1)) * percent_Negatif / 2 + pixel;
	pixel *= (AmbientLightColor + finalLights[0]);
	float4 finalPixel = pixel * luminosity;
	finalPixel.w = texel.w;

	return min(finalPixel,texel*input.Color);

}

technique Main {
	pass P0 {
        AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
		AlphaTestEnable = True;
		AlphaFunc = Greater;
		VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader  = compile ps_3_0 PixelShaderFunction();
	}
}
