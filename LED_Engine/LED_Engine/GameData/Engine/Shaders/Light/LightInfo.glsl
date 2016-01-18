struct LightInfo {
	vec3 Pos;		// Position in eye coords
	vec3 Ld;		// Diffuse intensity
	vec3 Ls;		// Specular intensity
	vec3 Att;		// Attenuation: Constant, Linear, Quadric
	vec3 Dir;		// Direction for SpotLight and DirectionalLight
	float CutOff;	// SpotLight: CutOff Angle in degrees [0..90]
	float Exp;		// SpotLight: Angular attenuation Exponent
};