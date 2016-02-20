uniform vec3 FogColor;
uniform vec2 FogMinMaxDistance;

//#include("Noise\PerlinNoise3D.glsl")
#include("Noise\SimplexNoise3D.glsl")

vec3 Fog(vec3 Color, vec3 Position, vec3 WorldPosition)
{
	if (FogMinMaxDistance.x < 0.0) //Fog Disabled
		return Color;
	
	float Distance = length(Position);
	float FogFactor = (FogMinMaxDistance.y - Distance) / (FogMinMaxDistance.y - FogMinMaxDistance.x);
	
	// Add noise for best result
	//FogFactor += pnoise(WorldPosition / 5.1) * 0.1 - 0.05;
	FogFactor += snoise(WorldPosition / 5.1) * 0.1 - 0.05;
	
	return mix(FogColor, Color, clamp(FogFactor, 0.0, 1.0));
}