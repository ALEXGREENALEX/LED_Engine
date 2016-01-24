uniform bool FogEnabled = false;
uniform vec3 FogColor;
uniform float FogMaxDist;
uniform float FogMinDist;

#include("Noise\PerlinNoise3D.glsl")

vec3 Fog(vec3 Color, vec3 Position, vec3 WorldPosition)
{
	float Distance = length(Position);
	float FogFactor = (FogMaxDist - Distance) / (FogMaxDist - FogMinDist);
	
	// Add noise for best result
	FogFactor +=
		pnoise(WorldPosition / 5.1) * 0.1 - 0.05 + 
		pnoise(WorldPosition / 7.2) * 0.1 - 0.05;
	
	return mix(FogColor, Color, clamp(FogFactor, 0.0, 1.0));
}