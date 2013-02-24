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
float4 EmissiveColor;
float4 AmbientLightColor;

float4 LightPosition0;
float4 LightDiffuseColor0; // intensity multiplier
float4 LightSpecularColor0; // intensity multiplier
float LightDistanceSquared0;
float4 DiffuseColor0;
float4 SpecularColor0;
float SpecularPower0;


float4 LightPosition1;
float4 LightDiffuseColor1;
float4 LightSpecularColor1;
float LightDistanceSquared1;
float4 DiffuseColor1;
float4 SpecularColor1;
float SpecularPower1;

float4 LightPosition2;
float4 LightDiffuseColor2;
float4 LightSpecularColor2;
float LightDistanceSquared2;
float4 DiffuseColor2;
float4 SpecularColor2;
float SpecularPower2;



bool Sepia = true;
float4x4 essai;
sampler2D s_2D;
float nblights = 3;

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


	
	float4 lightDir0 = normalize(input.WorldPos - LightPosition0); // per pixel diffuse lighting
	float diffuseLighting0 = saturate(dot(input.Normal, -lightDir0));
	diffuseLighting0 *= (LightDistanceSquared0 / dot(LightPosition0 - input.WorldPos, LightPosition0 - input.WorldPos));
	float4 h0 = normalize(normalize(CameraPos - input.WorldPos) - lightDir0);
	float specLighting0 = pow(saturate(dot(h0, input.Normal)), SpecularPower0);
	if (nblights == 1)
	{
		float4 abc = texel.xyzw * DiffuseColor0 * LightDiffuseColor0 * min(diffuseLighting0,float4(10,10,10,1)) * 0.6;
		float4 abcd = (SpecularColor0 * LightSpecularColor0 * specLighting0 * 0.5);
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
	else
	{
		float4 lightDir1 = normalize(input.WorldPos - LightPosition1); // per pixel diffuse lighting
		float diffuseLighting1 = saturate(dot(input.Normal, -lightDir1));
		diffuseLighting1 *= (LightDistanceSquared1 / dot(LightPosition1 - input.WorldPos, LightPosition1 - input.WorldPos));
		float4 h1 = normalize(normalize(CameraPos - input.WorldPos) - lightDir1);
		float specLighting1 = pow(saturate(dot(h1, input.Normal)), SpecularPower1);
		if (nblights == 2)
		{
			float4 abc = texel.xyzw * 0.6 * (DiffuseColor0 * LightDiffuseColor0 * min(diffuseLighting0,float4(10,10,10,1))+ DiffuseColor1 * LightDiffuseColor1 * min(diffuseLighting1,float4(10,10,10,1)));
			float4 abcd = 0.5 * ((SpecularColor1 * LightSpecularColor1 * specLighting1) + (SpecularColor0 * LightSpecularColor0 * specLighting0));
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
		else
		{
			float4 lightDir2 = normalize(input.WorldPos - LightPosition2); // per pixel diffuse lighting
			float diffuseLighting2 = saturate(dot(input.Normal, -lightDir2));
			diffuseLighting2 *= (LightDistanceSquared2 / dot(LightPosition2 - input.WorldPos, LightPosition2 - input.WorldPos));
			float4 h2 = normalize(normalize(CameraPos - input.WorldPos) - lightDir2);
			float specLighting2 = pow(saturate(dot(h2, input.Normal)), SpecularPower2);
			if (nblights == 3)
			{
				float4 abc = texel.xyzw * 0.6 *
					( DiffuseColor0 * LightDiffuseColor0 * min(diffuseLighting0,float4(10,10,10,1))
					+ DiffuseColor1 * LightDiffuseColor1 * min(diffuseLighting1,float4(10,10,10,1))
					+ DiffuseColor2 * LightDiffuseColor2 * min(diffuseLighting2,float4(10,10,10,1)));
				float4 abcd = 0.5 * 
					((SpecularColor0 * LightSpecularColor0 * specLighting0)
					+ (SpecularColor1 * LightSpecularColor1 * specLighting1)
					+ (SpecularColor2 * LightSpecularColor2 * specLighting2));
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
			else { return float4(1,1,1,1); }
		}
	}

}

technique Main {
	pass P0 {
        AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
		VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader  = compile ps_3_0 PixelShaderFunction();
	}
}
