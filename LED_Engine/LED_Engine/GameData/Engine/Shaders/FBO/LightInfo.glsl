struct DirectionalLightInfo {
	vec3 Dir;		// Direction
	vec3 Ld;		// Diffuse intensity
	vec3 Ls;		// Specular intensity
};

struct PointLightInfo {
	vec3 Pos;		// Position in eye coords
	vec3 Ld;		// Diffuse intensity
	vec3 Ls;		// Specular intensity
	vec3 Att;		// Attenuation: Constant, Linear, Quadric
};

struct SpotLightInfo {
	vec3 Pos;		// Position in eye coords
	vec3 Dir;		// Direction for SpotLight
	vec3 Ld;		// Diffuse intensity
	vec3 Ls;		// Specular intensity
	vec3 Att;		// Attenuation: Constant, Linear, Quadric
	float Cut;		// CutOFF Angle in degrees [0..90]
	float Exp;		// Angular attenuation Exponent
};