float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
//const static int nblights = 1;

texture DiffuseTexture;
float4 CameraPos;
float4 AmbientLightColor;

float4 LightPosition[nblights];
float4 LightDiffuseColor[nblights]; // intensity multiplier
float4 lightDir[nblights];
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
	float4 worldPosition : TEXCOORD2;
};


VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(mul(mul(input.Position,World), View), Projection);

	output.Normal = (mul(float4(input.Normal.xyz, 0), WorldInverseTranspose));
	output.Normal.w = 1;

	output.tex = input.tex;

	output.worldPosition = worldPosition;
	output.Color = input.col; //saturate(input.col * dot(output.Normal, LightPosition[0] - worldPosition));
	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	int i;
	float4 texel = tex2D(s_2D[0], input.tex);
	float4 finalLight = float4(0,0,0,1);
	float4 currentLight;
	float coef_bump;
	if (bump_mapping)
	{
		float4 normaltex = tex2D(s_2D[1], input.tex);
		float4 bump = normalize(normalize(normaltex * 2.0 - 1.0));
		bump.w = 1;
		coef_bump = max(saturate(dot(input.Normal, bump)), 0.0);
	}
	else
		coef_bump = 1;
	for (i = 0; i < nblights; i++)
	{
		currentLight =
			pow(LightDistanceSquared[i] / max(dot(LightPosition[i] - input.worldPosition, LightPosition[i] - input.worldPosition), LightDistanceSquared[i]),2)
			* LightDiffuseColor[i]
			* coef_bump;
		//if (i != 1) currentLight *= saturate(dot(input.Normal, normalize(LightPosition[i] - input.worldPosition)));
		finalLight += currentLight;
	}
	float4 pixel = saturate((texel * input.Color * 2 - float4(1,1,1,1)) * percent_Negatif / 2 + texel * input.Color) * saturate(AmbientLightColor + finalLight) * luminosity;
	pixel.w = texel.w;
	return min(pixel,texel*input.Color);

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
