#version 330
uniform sampler2D TextureUnit0;

#include("Other\MaterialInfo.glsl")
in MaterialInfo f_Material;
in vec2 f_UV;

out vec4 FragColor;

void main()
{
	FragColor = f_Material.Kd;
	
	if (TexUnits[0])
		FragColor *= texture(TextureUnit0, f_UV);
}