#version 330
uniform sampler2D TextureUnit4;
uniform sampler2D TextureUnit5;

in vec2 f_UV;

layout(location = 0) out vec4 FragColor;

void main()
{
	float S = texture(TextureUnit4, f_UV).a / 100.0;  //Just for convert Specular Highlight Exponent in visible range [0..1]
	float R = texture(TextureUnit5, f_UV).a;
	FragColor = vec4(S, R, 0.0, 1.0);
}