if(FogEnabled)
{
	FragColor = mix(vec4(FogColor, 1.0), FragColor, FogFactor);
}