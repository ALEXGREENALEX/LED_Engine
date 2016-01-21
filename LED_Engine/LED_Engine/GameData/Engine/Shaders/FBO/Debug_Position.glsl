#version 330
uniform sampler2D TextureUnit2;
uniform sampler2D TextureUnit4;

in vec2 f_UV;

layout(location = 0) out vec4 FragColor;

void main()
{
	vec3 Position = vec3(
		texture2D(TextureUnit2, f_UV).ba,
		texture2D(TextureUnit4, f_UV).a);
	
	FragColor = vec4(Position, 1.0);
}