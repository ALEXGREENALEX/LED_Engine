#version 330
uniform sampler2D TextureUnit0; // Depth

in vec2 f_UV;

uniform vec2 ClipPlanes; // zNear, zFar

layout(location = 0) out vec4 FragColor;

float LinearDepth(float NonLinearDepth, float zNear, float zFar)
{
	return (2.0 * zNear) / (zFar + zNear - NonLinearDepth * (zFar - zNear));
}

void main()
{
	float Depth = LinearDepth(texture(TextureUnit0, f_UV).x, ClipPlanes.x, ClipPlanes.y);
	FragColor = vec4(Depth, Depth, Depth, 1.0);
}