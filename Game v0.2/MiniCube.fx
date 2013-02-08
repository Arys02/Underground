// Copyright (c) 2010-2012 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.


float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 1; // 0.1
float3 DiffuseLightDirection = float3(1, 0, 0);
float3 ViewVector = float3(1, 0, 0);

struct VS_IN
{
	float4 pos : POSITION;
	float2 tex : TEXCOORD0;
	float4 col : COLOR0;
	float4 normal : NORMAL;
};

struct PS_IN
{
	float4 pos : POSITION;
	float2 tex : TEXCOORD0;
	float4 col : COLOR0;
};

texture Textu;
sampler2D s_2D;
float4x4 worldViewProj;

sampler S0 = sampler_state
{
    Texture = (Textu);
    MinFilter = ANISOTROPIC; //LINEAR;
    MagFilter = ANISOTROPIC; //LINEAR;
    MipFilter = LINEAR;
};

PS_IN VS( VS_IN input )
{
	PS_IN output = (PS_IN)0;
	
	output.pos = mul(input.pos, worldViewProj);
	output.col = input.col;
	output.tex = input.tex;
	
	return output;
}

float4 PS( PS_IN input ) : COLOR
{
	//float3 light = normalize(DiffuseLightDirection);
	//float3 normal = normalize(input.Normal);
    //float3 r = normalize(2 * dot(light, normal) * normal - light);
    //float3 v = normalize(mul(normalize(ViewVector), World));
	
	/*float3 n = normalize(input.normal);
	float3 h = normalize(input.halfVector);
	float3 l = normalize(input.lightDir);
	
	float nDotL = saturate(dot(n, l));
	float nDotH = saturate(dot(n, h));
	float power = (nDotL == 0.0f) ? 0.0f : pow(nDotH, material.shininess);   

	float4 color = (material.ambient * (globalAmbient + light.ambient)) +
		(IN.diffuse * nDotL) + (IN.specular * power);*/

	//return tex2D(s_2D, input.tex);
	return AmbientColor * AmbientIntensity * tex2D(s_2D, input.tex);
	//return input.col;
}


technique Main {
	pass P0 {
        AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
		VertexShader = compile vs_2_0 VS();
        PixelShader  = compile ps_2_0 PS();
	}
}
