uniform vec3 FogColor;
uniform vec2 FogMinMax;

//#include("Noise\PerlinNoise3D.glsl")
#include("Noise\SimplexNoise3D.glsl")

vec3 Fog(vec3 Color, vec3 Position, vec3 WorldPosition)
{
	if (FogMinMax.x < 0.0) //Fog Disabled
		return Color;
	
	float Distance = length(Position);
	float FogFactor = (FogMinMax.y - Distance) / (FogMinMax.y - FogMinMax.x);
	
	// Add noise for best result
	//FogFactor += pnoise(WorldPosition / 5.1) * 0.1 - 0.05;
	FogFactor += snoise(WorldPosition / 5.1) * 0.1 - 0.05;
	
	return mix(FogColor, Color, clamp(FogFactor, 0.0, 1.0));
}