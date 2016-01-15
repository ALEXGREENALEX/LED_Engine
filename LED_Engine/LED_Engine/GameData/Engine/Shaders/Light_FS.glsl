#version 330
uniform sampler2D TextureUnit0;

in vec3 f_Position;
in vec2 f_UV;
in vec3 f_Normal; // Нормализированная нормаль поверхности

out vec4 FragColor;

#include("Light\FS_Vars.glsl")
#include("Fog\FS_Vars.glsl")

void main()
{
	MaterialInfo M = Material;
	if (TexUnits[0])
		M.Kd *= texture(TextureUnit0, f_UV);
	
	FragColor = vec4(CalcLight(M, f_Normal, f_Position), 1.0);
}