#version 330
uniform sampler2D TextureUnit0;

in vec2 f_UV;

#include("FBO\MaterialInfo.glsl")
uniform MaterialInfo Material;

out vec4 FragColor;

void main()
{
	FragColor = Material.Kd;
	
	if (TexUnits[0])
		FragColor *= texture(TextureUnit0, f_UV);
}