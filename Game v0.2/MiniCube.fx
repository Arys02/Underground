/*float4 AmbientColor = float4(1, 1, 1, 1);
float4 DiffuseColor = float4(1, 1, 1, 1);
float AmbientIntensity = 1; // 0.1
float4 DiffuseLightDirection = float4(1, 0, 0, 1);
float3 ViewVector = float3(1, 0, 0);
bool Lumiere = true;
texture Textu;
sampler2D s_2D;
float4x4 worldViewProj; // viewprojection
float4 PositionPLight;*/

float4x4 World;
float4x4 View;
float4x4 Projection;

texture DiffuseTexture;
float4 CameraPos;
float4 LightPosition;
float4 LightDiffuseColor; // intensity multiplier
float4 LightSpecularColor; // intensity multiplier
float LightDistanceSquared;
float4 DiffuseColor;
float4 AmbientLightColor;
float4 EmissiveColor;
float4 SpecularColor;
float SpecularPower;
bool Sepia = true;

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
	float4 lightDir = normalize(input.WorldPos - LightPosition); // per pixel diffuse lighting
	float diffuseLighting = saturate(dot(input.Normal, -lightDir));
	diffuseLighting *= (LightDistanceSquared / dot(LightPosition - input.WorldPos, LightPosition - input.WorldPos));
	float4 h = normalize(normalize(CameraPos - input.WorldPos) - lightDir);
	float specLighting = pow(saturate(dot(h, input.Normal)), SpecularPower);
	float4 texel = tex2D(s_2D, input.tex);
	
	float4 abc = texel.xyzw * DiffuseColor * LightDiffuseColor * min(diffuseLighting,float4(10,10,10,1)) * 0.6;
	float4 abcd = (SpecularColor * LightSpecularColor * specLighting * 0.5);
	float4 outputColor = (saturate(AmbientLightColor + abc + abcd));
	float4 finalColor = outputColor;
	if (Sepia)
	{
		finalColor.r = (outputColor.r * 0.393) + (outputColor.g * 0.769) + (outputColor.b * 0.189);
		finalColor.g = (outputColor.r * 0.349) + (outputColor.g * 0.686) + (outputColor.b * 0.168);    
		finalColor.b = (outputColor.r * 0.272) + (outputColor.g * 0.534) + (outputColor.b * 0.131);
	}
	return finalColor;
}

technique Main {
	pass P0 {
        AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
		VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader  = compile ps_2_0 PixelShaderFunction();
	}
}
