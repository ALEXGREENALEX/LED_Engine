#include_String_Once("uniform vec2 ClipPlanes;") // zNear, zFar

//depth buffer Z-value remapped to a linear [0..1] range (eye to far plane)
float LinearDepth(float NonLinearDepth, float zNear, float zFar)
{
	return (2.0 * zNear) / (zFar + zNear - NonLinearDepth * (zFar - zNear));
}

float LinearDepth(sampler2D DepthTexture, vec2 UV, float zNear, float zFar)
{
	float NonLinearDepth = texture(DepthTexture, UV).z;
	return LinearDepth(NonLinearDepth, zNear, zFar);
}

float LinearDepth(sampler2D DepthTexture, vec2 UV)
{
	float NonLinearDepth = texture(DepthTexture, UV).z;
	return LinearDepth(NonLinearDepth, ClipPlanes.x, ClipPlanes.y);
}