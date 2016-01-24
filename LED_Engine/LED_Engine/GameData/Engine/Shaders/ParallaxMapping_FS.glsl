#version 330
uniform sampler2D TextureUnit0; //Diffuse
uniform sampler2D TextureUnit1; //Normal
uniform sampler2D TextureUnit2; //Specular
uniform sampler2D TextureUnit3; //Emissive
uniform sampler2D TextureUnit4; //AO
uniform sampler2D TextureUnit5; //Height

in vec3 f_EyePosition;
in vec2 f_UV;
in mat3 f_InvTBN;

uniform float ParallaxScale = 0.04;

#include("FBO\MaterialInfo.glsl")
uniform MaterialInfo Material;

layout (location = 0) out vec4 Output0;
layout (location = 1) out vec4 Output1;
layout (location = 2) out vec4 Output2;
layout (location = 3) out vec4 Output3;
layout (location = 4) out vec4 Output4;

// Simple Parallax Mapping
vec2 ParallaxMapping(sampler2D HeightTexture, vec2 UV, vec3 ViewVector, float Scale)
{
	float ParallaxOffset = (texture(HeightTexture, UV).r) * Scale;
	return UV + ParallaxOffset * ViewVector.xy;
}

//POM, based on: http://steps3d.narod.ru/tutorials/parallax-mapping-tutorial.html
vec2 ParallaxOcclusionMapping(sampler2D HeightTexture, vec2 UV, vec3 ViewVector, float Scale)
{
	const float MinSteps  = 20.0;
	const float MaxSteps  = 30.0;
	
	float NumSteps = MaxSteps + ViewVector.z * (MinSteps - MaxSteps);
	float Step = 1.0 / NumSteps;
	vec2 dTexCoords = -Step * Scale * ViewVector.xy / ViewVector.z; //Offset for one layer
	float Height = 0.99; //Height of the layer
	vec2 NewTexCoords = UV;
	float TextureHeight = texture2D(HeightTexture, NewTexCoords).r;
	
	while(TextureHeight < Height)
	{
		Height -= Step;
		NewTexCoords += dTexCoords;
		TextureHeight = texture2D (HeightTexture, NewTexCoords).r;
	}
	
	vec2 PrevPoint = NewTexCoords - dTexCoords;
	float hPrev = texture2D(HeightTexture, PrevPoint).r - (Height + Step); // < 0
	float hCur = TextureHeight - Height; // > 0
	
	return mix(NewTexCoords, PrevPoint, hCur / (hCur - hPrev));
}

void main()
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	vec2 newTexCoords = f_UV;
	vec3 EyeVector = normalize(transpose(f_InvTBN) * (-f_EyePosition));
	
	//newTexCoords = ParallaxMapping(TextureUnit5, f_UV, EyeVector, ParallaxScale);
	newTexCoords = ParallaxOcclusionMapping(TextureUnit5, f_UV, EyeVector, ParallaxScale);
	////////////////////////////////////////////////////////////////////////////////////////////////////

	if (TexUnits[0])
		Output0 = Material.Kd * texture(TextureUnit0, newTexCoords);
	else
		Output0 = Material.Kd;
	
	if (TexUnits[1])
		Output1.xy = normalize(f_InvTBN * normalize(texture(TextureUnit1, newTexCoords).rgb * 2.0 - 1.0)).xy;
	else
		Output1.xy = normalize(f_InvTBN[2]).xy;
	Output1.zw = f_EyePosition.xy;
	
	if (TexUnits[2])
		Output2.xyz = Material.Ks * texture(TextureUnit2, newTexCoords).rgb;
	else
		Output2.xyz = Material.Ks;
	Output2.w = Material.S;
	
	if (TexUnits[3])
		Output3.xyz = Material.Ke * texture(TextureUnit3, newTexCoords).rgb;
	else
		Output3.xyz = Material.Ke;
	Output3.w = f_EyePosition.z;
	
	if (TexUnits[4])
		Output4.xyz = Material.Ka * texture(TextureUnit4, newTexCoords).r;
	else
		Output4.xyz = Material.Ka;
	Output4.w = 0.0; //NOTHING (Value not used, Free channel)
}