struct MaterialInfo {
	vec3 Ka;		// Ambient reflectivity, RGB
	vec4 Kd;		// Diffuse reflectivity, RGBA
	vec3 Ks;		// Specular reflectivity, RGB
	vec3 Ke;		// Emissive Color, RGB
	float S;		// Specular shininess factor
};

// TexUnits[] is used for check, have Material some texture in TextureUnit or not.
//uniform bool TexUnits[min(min(gl_MaxTextureImageUnits, gl_MaxVertexTextureImageUnits), gl_MaxGeometryTextureImageUnits)];
uniform bool TexUnits[gl_MaxTextureImageUnits]; // Use Texture Units? (Check Fragment shader only for optimization)