/***********************    MATRICES    ***********************/
float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
//const static int nblights = 1;

/***********************     LIGHTS     ***********************/
float4 AmbientLightColor;
float3 LightPosition[nblights];
float4 LightDiffuseColor[nblights]; // intensity multiplier
float LightDistanceSquared[nblights];
float luminosity;

/***********************    FILTERS     ***********************/
//bool Sepia = false;
bool bump_mapping = true;
float percent_Negatif = 0;

/***********************    Textures     ***********************/
texture colorMapTexture;
sampler2D colorMap = sampler_state {
    Texture = (colorMapTexture);
    MinFilter = ANISOTROPIC; //LINEAR;
    MagFilter = ANISOTROPIC; //LINEAR;
    MipFilter = LINEAR;
	MaxAnisotropy = 16;
};
texture normalMapTexture;
sampler2D normalMap = sampler_state {
    Texture = (normalMapTexture);
    MinFilter = ANISOTROPIC; //LINEAR;
    MagFilter = ANISOTROPIC; //LINEAR;
    MipFilter = LINEAR;
	MaxAnisotropy = 16;
};

/***********************   Structures    ***********************/
struct VertexShaderInput
{
    float4 Position : POSITION0;  
	float2 tex : TEXCOORD0;
	float4 col : COLOR0;  
    float4 Normal : NORMAL0;
	float4 Tangent : TANGENT;
};
struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 tex : TEXCOORD0;
    float4 Color : COLOR0;
	float3 Normal : TEXCOORD1;
	float3 worldPosition : TEXCOORD2;
	float3 t : TEXCOORD3;
	float3 b : TEXCOORD4;
};


VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	//CameraPos = LightPosition[0];
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.Normal = mul(input.Normal.xyz, (float3x3)WorldInverseTranspose);
	float3 t = mul(input.Tangent.xyz, (float3x3)WorldInverseTranspose);
	float3 b = cross(output.Normal, t) * input.Tangent.w;
	output.tex = input.tex;
	output.t = t;
	output.b = b;
	output.worldPosition = worldPosition.xyz;
	output.Color = input.col;
	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3x3 TangentBinomialNormalMatrix = float3x3(input.t.x, input.b.x, input.Normal.x,
													input.t.y, input.b.y, input.Normal.y,
													input.t.z, input.b.z, input.Normal.z);
	float atten, nDotL, nDotH, power;
	float4 color = float4(0,0,0,0);
	float4 texel = tex2D(colorMap, input.tex);
	//return texel;
	float3 lightDir, l, h;
	float3 v = mul(normalize(LightPosition[0] - input.worldPosition), TangentBinomialNormalMatrix); // LightPosition[0] représente ici la position du joueur
	float3 n;
	n = normalize(tex2D(normalMap, input.tex).rgb * 2.0f - 1.0f);
	int i;

	for (i = 0; i < nblights; i++)
	{
		lightDir = mul(((LightPosition[i] - input.worldPosition) / LightDistanceSquared[i]).xyz, TangentBinomialNormalMatrix);
		atten = saturate(1.0f - dot(lightDir, lightDir));
		l = normalize(lightDir);
		nDotL = saturate(dot(n, l));
		h = normalize(l + v);
		nDotH = saturate(dot(n, h));
		power = (nDotL == 0.0f) ? 0.0f : pow(nDotH, 90);
		color += LightDiffuseColor[i] * nDotL * atten + input.Color * power * atten; // diffuse , specular
	}
	color =
		saturate(color) // éclairage
		* ((float4(1,1,1,1) - 2 * texel * input.Color) * percent_Negatif + texel * input.Color)
		* luminosity; // blink
	color.w = texel.w;
	return color;

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
