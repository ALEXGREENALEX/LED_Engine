if(FogEnabled)
{
	float FogDist = length(f_FogPosition);
	float FogFactor =	(FogMaxDist - FogDist) /
						(FogMaxDist - FogMinDist);
	FogFactor = clamp(FogFactor, 0.0, 1.0);
	FragColor = mix(vec4(FogColor, 1.0), FragColor, FogFactor);
}