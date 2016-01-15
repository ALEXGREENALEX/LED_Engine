struct LightInfo {
	vec3 Pos;	// Light position in eye coords.
	vec3 Ld;	// Diffuse light intensity
	vec3 Ls;	// Specular light intensity
	vec3 Att;	// Attenuation Constant, Linear, Quadric
};