uniform bool FogEnabled = false;
uniform vec3 FogColor;
uniform float FogMaxDist;
uniform float FogMinDist;

#include("Noise\SimplexNoise3D.glsl")

vec3 Fog(vec3 Color, vec3 Position, vec3 WorldPosition)
{
	float Distance = length(Position);
	float FogFactor = (FogMaxDist - Distance) / (FogMaxDist - FogMinDist);
	FogFactor += 
	snoise(WorldPosition / 10.0) * 0.2 - 0.1;
	return mix(FogColor, Color, clamp(FogFactor, 0.0, 1.0));
}