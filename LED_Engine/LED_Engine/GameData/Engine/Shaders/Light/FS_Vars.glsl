#include("LightInfo.glsl")

#define MaxLights 2
uniform LightInfo Light[MaxLights];
uniform vec3 LightAmbient; //Ambient light intensity

void PhongModel(LightInfo L, vec3 N, vec3 P, float Shininess, inout vec3 Diffuse, inout vec3 Specular)
{
	vec3 S = normalize(L.Pos - P); //Light direction vector (from surface to Light)
	vec3 Normal = normalize(N); //Normalized surface Normal
	float sDotN = max(dot(S, Normal), 0.0);
	
	if (sDotN > 0.0)
	{
		float AttenuationDistance = length(L.Pos - P); //Attenuation
		float Attenuation = L.Att.x +
							L.Att.y * AttenuationDistance +
							L.Att.z * AttenuationDistance * AttenuationDistance;
		
		//const float FalloffExponent = 1.0;
		//float Attenuation = pow(AttenuationDistance, FalloffExponent);
		
		Diffuse += sDotN * L.Ld / Attenuation; //Diffuse
		
		vec3 V = normalize(-P); //View Vector
		vec3 HalfWay = normalize(V + S); //HalfWay vector
		Specular += pow(max(dot(HalfWay, Normal), 0.0), Shininess) * L.Ls / Attenuation; //Specular
	}
}

vec3 CalcLight(vec3 Kd, vec3 Ks, float Shininess, vec3 Ka, vec3 Ke, vec3 Normal, vec3 Position)
{
	vec3 AccumDiffuse = vec3(0.0);
	vec3 AccumSpecular = vec3(0.0);
	
	for(int LightIndex = 0; LightIndex < Light.length(); LightIndex++)
		PhongModel(Light[LightIndex], Normal, Position, Shininess, AccumDiffuse, AccumSpecular);
	
	return	AccumDiffuse * Kd + //Diffuse
			AccumSpecular * Ks + //Specular
			LightAmbient * Ka + Ke; //Ambient + Emissive
}