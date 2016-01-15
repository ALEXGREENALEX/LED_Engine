#include("LightInfo.glsl")
#include("MaterialInfo.glsl")

#define MaxLights 2
uniform LightInfo Light[MaxLights];
uniform vec3 LightAmbient; //Ambient light intensity
uniform MaterialInfo Material;

/*
L - Light
M - Material
N - Surface Normal
P - Surface position in Eye coords
S - Light direction
V - View Vector
*/

void PhongModel(LightInfo L, MaterialInfo M, vec3 N, vec3 P, vec3 S, vec3 V, inout vec3 Diffuse, inout vec3 Specular)
{
	//S = normalize(L.Pos - P);  (for default PhongModel)
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
		
		//V = normalize(-P); (for default PhongModel)
		vec3 HalfWay = normalize(V + S); //HalfWay vector
		Specular += pow(max(dot(HalfWay, Normal), 0.0), M.S) * L.Ls / Attenuation; //Specular
	}
}

void PhongModel(LightInfo L, MaterialInfo M, vec3 N, vec3 P, inout vec3 AccumDiffuse, inout vec3 AccumSpecular)
{
	vec3 S = normalize(L.Pos - P); //Light direction vector (from surface to Light)
	vec3 V = normalize(-P); //View Vector
	PhongModel(L, M, N, P, S, V, AccumDiffuse, AccumSpecular);
}

vec3 CalcLight(MaterialInfo M, vec3 N, vec3 P, vec3 S, vec3 V)
{
	vec3 AccumDiffuse = vec3(0.0);
	vec3 AccumSpecular = vec3(0.0);
	
	for(int LightIndex = 0; LightIndex < Light.length(); LightIndex++)
		PhongModel(Light[LightIndex], M, N, P, S, V, AccumDiffuse, AccumSpecular);
	
	return 	AccumDiffuse * M.Kd.rgb + //Diffuse
			AccumSpecular * M.Ks + //Specular
			LightAmbient * M.Ka + M.Ke; //Ambient + Emissive
}

vec3 CalcLight(MaterialInfo M, vec3 N, vec3 P)
{
	vec3 AccumDiffuse = vec3(0.0);
	vec3 AccumSpecular = vec3(0.0);
	
	for(int LightIndex = 0; LightIndex < Light.length(); LightIndex++)
		PhongModel(Light[LightIndex], M, N, P, AccumDiffuse, AccumSpecular);
	
	return 	AccumDiffuse * M.Kd.rgb + //Diffuse
			AccumSpecular * M.Ks + //Specular
			LightAmbient * M.Ka + M.Ke; //Ambient + Emissive
}