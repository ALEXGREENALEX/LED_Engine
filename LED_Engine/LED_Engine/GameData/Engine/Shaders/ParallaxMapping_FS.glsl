#version 330
uniform sampler2D TextureUnit0; //Diffuse
uniform sampler2D TextureUnit1; //Normal
uniform sampler2D TextureUnit2; //Specular
uniform sampler2D TextureUnit3; //Emissive
uniform sampler2D TextureUnit4; //AO
uniform sampler2D TextureUnit5; //Height

in vec3 f_EyePosition;
in vec2 f_UV;
in mat3 f_TBN;

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
	const float MinSteps  = 15.0;
	const float MaxSteps  = 30.0;
	
	float NumSteps = MaxSteps + ViewVector.z * (MinSteps - MaxSteps);
	float Step = 1.0 / NumSteps;
	vec2 dTexCoords = -Step * Scale * ViewVector.xy / ViewVector.z; //Offset for one layer
	float Height = 1.0; //Height of the layer
	vec2 NewTexCoords = UV;
	float TextureHeight = texture(HeightTexture, NewTexCoords).r;
	
	while(TextureHeight < Height)
	{
		Height -= Step;
		NewTexCoords += dTexCoords;
		TextureHeight = texture(HeightTexture, NewTexCoords).r;
	}
	
	vec2 PrevPoint = NewTexCoords - dTexCoords;
	float hPrev = texture(HeightTexture, PrevPoint).r - (Height + Step); // < 0
	float hCur = TextureHeight - Height; // > 0
	
	return mix(NewTexCoords, PrevPoint, hCur / (hCur - hPrev));
}

void main()
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	vec2 newTexCoords = f_UV;
	vec3 EyeVector = normalize(transpose(f_TBN) * (-f_EyePosition)); //Convert from Eye to Tangent Space
	
	//newTexCoords = ParallaxMapping(TextureUnit5, f_UV, EyeVector, ParallaxScale);
	newTexCoords = ParallaxOcclusionMapping(TextureUnit5, f_UV, EyeVector, ParallaxScale);
	////////////////////////////////////////////////////////////////////////////////////////////////////

	vec4 Diffuse = Material.Kd;
	vec2 NormalXY;
	vec4 SpecularShininess = vec4(Material.Ks, Material.S);
	vec3 Emissive = Material.Ke;
	vec3 Ambient = Material.Ka;
	
	if (TexUnits[0])
		Diffuse *= texture(TextureUnit0, newTexCoords);
	
	if (TexUnits[1])
		NormalXY.xy = normalize(f_TBN * normalize(texture(TextureUnit1, newTexCoords).rgb * 2.0 - 1.0)).xy;
	else
		NormalXY.xy = normalize(f_TBN[2]).xy; //Take Column with Normals in Eye Space
	
	if (TexUnits[2])
		SpecularShininess.xyz *= texture(TextureUnit2, newTexCoords).rgb;
	
	if (TexUnits[3])
		Emissive *= texture(TextureUnit3, newTexCoords).rgb;
	
	if (TexUnits[4])
		Ambient *= texture(TextureUnit4, newTexCoords).r;
	
	Output0 = Diffuse;
	Output1.xy = NormalXY;
	Output1.zw = Emissive.xy;
	Output2.xyz = f_EyePosition;
	Output2.w = Emissive.z;
	Output3 = SpecularShininess;
	Output4.xyz = Ambient;
	Output4.w = 0.0; //NOTHING (Value not used, Free channel)
}