#version 330
uniform sampler2D TextureUnit0;
uniform sampler2D TextureUnit1;
uniform sampler2D TextureUnit2;
uniform sampler2D TextureUnit3;
uniform sampler2D TextureUnit4;

in vec3 f_EyePosition;
in vec2 f_UV;
in mat3 f_TBN;

#include("FBO\MaterialInfo.glsl")
uniform MaterialInfo Material;

layout (location = 0) out vec4 Output0;
layout (location = 1) out vec4 Output1;
layout (location = 2) out vec4 Output2;
layout (location = 3) out vec4 Output3;
layout (location = 4) out vec4 Output4;

void main()
{
	if (TexUnits[0])
		Output0 = Material.Kd * texture(TextureUnit0, f_UV);
	else
		Output0 = Material.Kd;
	
	if (TexUnits[1])
		Output1.xy = normalize(f_TBN * normalize(texture(TextureUnit1, f_UV).rgb * 2.0 - 1.0)).xy;
	else
		Output1.xy = normalize(f_TBN[2]).xy;
	Output1.zw = f_EyePosition.xy;
	
	if (TexUnits[2])
		Output2.xyz = Material.Ks * texture(TextureUnit2, f_UV).rgb;
	else
		Output2.xyz = Material.Ks;
	Output2.w = Material.S;
	
	if (TexUnits[3])
		Output3.xyz = Material.Ke * texture(TextureUnit3, f_UV).rgb;
	else
		Output3.xyz = Material.Ke;
	Output3.w = f_EyePosition.z;
	
	if (TexUnits[4])
		Output4.xyz = Material.Ka * texture(TextureUnit4, f_UV).r;
	else
		Output4.xyz = Material.Ka;
	Output4.w = 0.0; //NOTHING (Value not used, Free channel)
}