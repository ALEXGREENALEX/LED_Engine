struct MaterialInfo {
	vec4 Kd;		// Diffuse reflectivity, RGBA
	vec3 Ka;		// Ambient reflectivity, RGB
	vec3 Ks;		// Specular reflectivity, RGB
	vec3 Ke;		// Emissive Color, RGB
	float S;		// Specular shininess factor
};

uniform bool TexUnits[gl_MaxTextureImageUnits];	// Use Texture Units?
// TexUnits[] is used for check, have Material some texture in TextureUnit or not.