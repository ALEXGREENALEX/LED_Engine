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

vec2 ParallaxOcclusionMapping(sampler2D HeightTexture, vec2 UV, vec3 ViewVector, float Scale)
{
	const float	minSteps  = 15.0;
	const float maxSteps  = 40.0;

	float numSteps  = maxSteps + ViewVector.z * (minSteps - maxSteps);
	float Step   = 1.0 / numSteps;
	vec2 dtex   = -Step * Scale * ViewVector.xy / ViewVector.z;	// adjustment for one layer
	float height = 0.99;								// height of the layer
	vec2 newTexCoords = UV;				// our initial guess
	float h = texture2D(HeightTexture, newTexCoords).r;

	while ( h < height )
	{
		height -= Step;
		newTexCoords    += dtex;
		h = texture2D (HeightTexture, newTexCoords).r;
	}
									// now find point via linear interpolation
	vec2 prev = newTexCoords - dtex; // previous point
	float hPrev = texture2D(HeightTexture, prev).r - (height + Step); // < 0
	float hCur = h - height;	// > 0
	
	return mix(newTexCoords, prev, hCur / (hCur - hPrev));
}

void main()
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	vec2 newTexCoords = f_UV;
	
	//Parallax Mapping setion:
	vec3 EyeVector = normalize(transpose(f_InvTBN) * (-f_EyePosition));
	
	// Simple Parallax mapping
	//float ParallaxOffset = (texture(TextureUnit5, f_UV).r) * ParallaxScale;
	//newTexCoords = f_UV + ParallaxOffset * EyeVector.xy;
	
	// POM (Parallax Occlusion Mapping)
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