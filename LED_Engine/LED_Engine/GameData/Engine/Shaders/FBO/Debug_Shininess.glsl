#version 330
uniform sampler2D TextureUnit3;

in vec2 f_UV;

layout(location = 0) out vec4 FragColor;

void main()
{
	float S = texture2D(TextureUnit3, f_UV).a;
	S /= 100.0; //Just for convert Specular Highlight Exponent in visible range [0..1]
	FragColor = vec4(S, S, S, 1.0);
}