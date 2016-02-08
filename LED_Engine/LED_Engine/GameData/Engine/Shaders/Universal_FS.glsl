#version 330
uniform sampler2D TextureUnit0; //Diffuse
uniform sampler2D TextureUnit1; //Normal
uniform sampler2D TextureUnit2; //Specular
uniform sampler2D TextureUnit3; //Emissive
uniform sampler2D TextureUnit4; //Height

in vec3 f_EyePosition;
in vec2 f_UV;
in mat3 f_TBN;

#include("FBO\MaterialInfo.glsl")
uniform MaterialInfo Material;

uniform float ParallaxScale = 0.04;
#include("POM.glsl")

layout(location = 0) out vec4 Output0;
layout(location = 1) out vec4 Output1;
layout(location = 2) out vec4 Output2;
layout(location = 3) out vec4 Output3;
layout(location = 4) out vec4 Output4;

void main()
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	vec2 newTexCoords = f_UV;
	if (TexUnits[4])
	{
		vec3 EyeVector = normalize(transpose(f_TBN) * (-f_EyePosition)); //Convert from Eye to Tangent Space
		newTexCoords = ParallaxOcclusionMapping(TextureUnit4, f_UV, EyeVector, ParallaxScale);
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	vec4 Diffuse = Material.Kd;
	vec2 NormalXY;
	vec4 SpecularShininess = vec4(Material.Ks, Material.S);
	vec3 Emissive = Material.Ke;
	
	if (TexUnits[0])
		Diffuse *= texture(TextureUnit0, newTexCoords);
		
	if (Diffuse.a <= 0.02)
		discard;
	
	if (TexUnits[1])
		NormalXY.xy = normalize(f_TBN * normalize(texture(TextureUnit1, newTexCoords).rgb * 2.0 - 1.0)).xy;
	else
		NormalXY.xy = normalize(f_TBN[2]).xy; //Take Column with Normals in Eye Space
	
	if (TexUnits[2])
		SpecularShininess.xyz *= texture(TextureUnit2, newTexCoords).rgb;
	
	if (TexUnits[3])
		Emissive *= texture(TextureUnit3, newTexCoords).rgb;
	
	Output0 = Diffuse;
	Output1.xy = NormalXY;
	Output1.zw = Emissive.xy;
	Output2.xyz = f_EyePosition;
	Output2.w = Emissive.z;
	Output3 = SpecularShininess;
	Output4.xyz = Material.Ka;
	Output4.w = 0.0; //NOTHING (Value not used, Free channel)
}