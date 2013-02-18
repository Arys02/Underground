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
float4 DiffuseColor = float4(1, 1, 1, 1);
float AmbientIntensity = 1; // 0.1
float3 DiffuseLightDirection = float3(1, 0, 0);
float3 ViewVector = float3(1, 0, 0);
bool Lumiere = true;
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
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	
    VertexShaderOutput output;

    output.Position = mul(input.Position, worldViewProj);

    float4 normal = input.Normal;
    float lightIntensity = dot(normal, DiffuseLightDirection);
	output.tex = input.tex;
	if (Lumiere)
	{
		output.Color = saturate ( AmbientColor * input.col * DiffuseColor * lightIntensity);
	}
	else
	{
		output.Color = saturate (AmbientColor * input.col * DiffuseColor);
	}
	output.Color[3] = 1;

    return output;
}


float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 finalColor = input.Color * tex2D(s_2D, input.tex);
	float4 outputColor = finalColor;
    /* FILTRE SEPIA
	outputColor.r = (finalColor.r * 0.393) + (finalColor.g * 0.769) + (finalColor.b * 0.189);
    outputColor.g = (finalColor.r * 0.349) + (finalColor.g * 0.686) + (finalColor.b * 0.168);    
    outputColor.b = (finalColor.r * 0.272) + (finalColor.g * 0.534) + (finalColor.b * 0.131);*/
	return outputColor;
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
