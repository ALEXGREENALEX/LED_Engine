uniform bool FogEnabled = false;
uniform vec3 FogColor;
uniform float FogMaxDist;
uniform float FogMinDist;

vec3 Fog(vec3 Color, vec3 Position)
{
	float Distance = length(Position);
	float FogFactor = (FogMaxDist - Distance) / (FogMaxDist - FogMinDist);
	return mix(FogColor, Color, clamp(FogFactor, 0.0, 1.0));
}