#version 330
uniform sampler2D TextureUnit0; //Diffuse
uniform sampler2D TextureUnit1; //Normal
uniform sampler2D TextureUnit2; //Specular
uniform sampler2D TextureUnit3; //Emissive
uniform sampler2D TextureUnit4; //Height
uniform samplerCube	TextureUnit5; //CubeMap (for CubeMap Reflection)

in vec3 f_EyePosition;
in vec2 f_UV;
in mat3 f_TBN;

#include("FBO\MaterialInfo.glsl")
uniform MaterialInfo Material;

////////////////////////////
// Cubemap Reflection
////////////////////////////
in vec3 ReflectionDir;
in vec3 ReflectionNormal;
uniform float Reflection; //Reflection factor
////////////////////////////

layout(location = 0) out vec4 Output0;
layout(location = 1) out vec4 Output1;
layout(location = 2) out vec4 Output2;
layout(location = 3) out vec4 Output3;
layout(location = 4) out vec4 Output4;

////////////////////////////////////////////////////////////////////////////////////////////////////////
// Parallax Occlusion Mapping, based on: http://steps3d.narod.ru/tutorials/parallax-mapping-tutorial.html
////////////////////////////////////////////////////////////////////////////////////////////////////////
uniform float ParallaxScale = 0.04;

vec2 ParallaxOcclusionMapping(sampler2D HeightTexture, vec2 UV, vec3 ViewVector, float Scale)
{
	const float MinSteps  = 10.0;
	const float MaxSteps  = 20.0;
	
	float NumSteps = MaxSteps + ViewVector.z * (MinSteps - MaxSteps);
	float Step = 1.0 / NumSteps;
	vec2 dTexCoords = -Step * Scale * ViewVector.xy / ViewVector.z; //Offset for one layer
	float Height = 1.0; //Height of the layer
	vec2 NewTexCoords = UV;
	float TextureHeight = texture(HeightTexture, NewTexCoords).r;
	
	float StepCount = 0.0;
	while(TextureHeight < Height)
	{
		Height -= Step;
		NewTexCoords += dTexCoords;
		TextureHeight = texture(HeightTexture, NewTexCoords).r;
		StepCount++;
		if (StepCount > MaxSteps)
			break;
	}
	
	vec2 PrevPoint = NewTexCoords - dTexCoords;
	float hPrev = texture(HeightTexture, PrevPoint).r - (Height + Step); // < 0
	float hCur = TextureHeight - Height; // > 0
	
	return mix(NewTexCoords, PrevPoint, hCur / (hCur - hPrev));
}
////////////////////////////////////////////////////////////////////////////////////////////////////////

void main()
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	// Parallax Occlusion Mapping
	////////////////////////////////////////////////////////////////////////////////////////////////////
	vec2 NewTexCoords = f_UV;
	if (TexUnits[4])
	{
		vec3 EyeVector = normalize(transpose(f_TBN) * (-f_EyePosition)); //Convert from Eye to Tangent Space
		NewTexCoords = ParallaxOcclusionMapping(TextureUnit4, f_UV, EyeVector, ParallaxScale);
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	vec4 Diffuse = Material.Kd;
	vec2 NormalXY;
	vec4 SpecularShininess = vec4(Material.Ks, Material.S);
	vec3 Emissive = Material.Ke;
	
	if (TexUnits[0])
		Diffuse *= texture(TextureUnit0, NewTexCoords);
		
	if (Diffuse.a <= 0.02)
		discard;
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	// Cubemap Reflection
	////////////////////////////////////////////////////////////////////////////////////////////////////
	float ReflectionFromSpecular = 1.0;
	
	if (TexUnits[2])
	{
		SpecularShininess.xyz *= texture(TextureUnit2, NewTexCoords).rgb;
		
		//Или использовать отдельно карту отражений, можно ее запихнуть в альфа канал какой-то из карт...
		ReflectionFromSpecular = texture(TextureUnit2, NewTexCoords).g;
	}
	
	if (TexUnits[5])
	{
		vec3 ReflectionCoords = reflect(normalize(ReflectionDir), ReflectionNormal);
		vec3 ReflectionCubemap = texture(TextureUnit5, ReflectionCoords).rgb;
		Diffuse.rgb *= mix(Diffuse.rgb, ReflectionCubemap * ReflectionFromSpecular, Reflection);
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	if (TexUnits[1])
		NormalXY.xy = normalize(f_TBN * normalize(texture(TextureUnit1, NewTexCoords).rgb * 2.0 - 1.0)).xy;
	else
		NormalXY.xy = normalize(f_TBN[2]).xy; //Take Column with Normals in Eye Space
	
	if (TexUnits[3])
		Emissive *= texture(TextureUnit3, NewTexCoords).rgb;
	
	Output0 = Diffuse;
	Output1.xy = NormalXY;
	Output1.zw = Emissive.xy;
	Output2.xyz = f_EyePosition;
	Output2.w = Emissive.z;
	Output3 = SpecularShininess;
	Output4 = vec4(Material.Ka, Reflection);
}