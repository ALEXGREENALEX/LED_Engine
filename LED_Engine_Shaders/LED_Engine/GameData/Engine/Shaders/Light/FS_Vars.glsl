in vec3 v_PosEye;

#include("LightInfo.glsl")
#include("MaterialInfo.glsl")

uniform LightInfo Light;
uniform MaterialInfo Material;

vec3 PhongModel(vec3 LightDirection, vec3 ViewDirection, vec3 Normal, LightInfo L, MaterialInfo M)
{
	vec3 Refl = reflect(-LightDirection, Normal);
	float sDotN = max(dot(LightDirection, Normal), 0.0);
	
	float Spec = 0;
	if (sDotN > 0)
		Spec = pow(max(dot(Refl, ViewDirection), 0.0), M.S);
	
	// Diffuse
	vec3 Result = sDotN * L.Ld * M.Kd.rgb; // DiffuseMap.rgb
	
	// Specular
	Result += Spec * L.Ls * M.Ks;
	
	// Attenuation
	float AttenDistance = length(L.Pos - v_PosEye);
	Result = Result / (L.Att.x + L.Att.y * AttenDistance + L.Att.z * AttenDistance * AttenDistance);
	
	// Ambient
	Result += L.La * M.Ka;
	return Result;
}