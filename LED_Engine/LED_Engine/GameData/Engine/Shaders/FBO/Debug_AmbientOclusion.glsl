#version 330
uniform sampler2D TextureUnit5;

in vec2 f_UV;

layout(location = 0) out vec4 FragColor;

void main()
{
	vec3 Ka = texture2D(TextureUnit5, f_UV).rgb;
	FragColor = vec4(Ka, 1.0);
}