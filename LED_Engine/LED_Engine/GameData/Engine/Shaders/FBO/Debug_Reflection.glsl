#version 330
uniform sampler2D TextureUnit5;

in vec2 f_UV;

layout(location = 0) out vec4 FragColor;

void main()
{
	float R = texture2D(TextureUnit5, f_UV).a;
	FragColor = vec4(R, R, R, 1.0);
}