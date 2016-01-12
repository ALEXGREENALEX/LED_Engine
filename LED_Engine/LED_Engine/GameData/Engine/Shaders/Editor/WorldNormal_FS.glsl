#version 330

in vec3 f_Color;

out vec4 FragColor;

void main()
{
	FragColor = vec4(f_Color, 1.0) * 0.5 + 0.5;
}