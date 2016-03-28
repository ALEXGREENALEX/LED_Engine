#include("LightInfo.glsl")

#define Max_DirectionalLights 4
#define Max_PointLights 100
#define Max_SpotLights 50

uniform DirectionalLightInfo	LightD[Max_DirectionalLights];
uniform PointLightInfo			LightP[Max_PointLights];
uniform SpotLightInfo			LightS[Max_SpotLights];
uniform vec3 LightA;			//Ambient light intensity
uniform int LDcount;			//Directional Light Count
uniform int LPcount;			//Directional Light Count
uniform int LScount;			//Directional Light Count

float Attenuation(vec3 ConstLinearQuad, vec3 Distance)
{
	float AttDistance = length(Distance);
	return 	ConstLinearQuad.x +
			ConstLinearQuad.y * AttDistance +
			ConstLinearQuad.z * AttDistance * AttDistance;
	
	//const float FalloffExponent = 1.0;
	//return pow(Distance, FalloffExponent);
}

void PhongModel(DirectionalLightInfo L, vec3 N, vec3 Pos, float Shininess, inout vec3 Diffuse, inout vec3 Specular)
{
	vec3 S = normalize(L.Dir); //Light direction vector
	float sDotN = max(dot(S, N), 0.0);
	
	if (sDotN > 0.0)
	{
		Diffuse += sDotN * L.Ld; //Diffuse
		
		vec3 V = normalize(-Pos); //View Vector
		vec3 H = normalize(V + S); //HalfWay vector
		Specular += pow(max(dot(H, N), 0.0), Shininess) * L.Ls; //Specular
	}
}

void PhongModel(PointLightInfo L, vec3 N, vec3 Pos, float Shininess, inout vec3 Diffuse, inout vec3 Specular)
{
	vec3 P = L.Pos - Pos;
	vec3 S = normalize(P); //Light direction vector
	float sDotN = max(dot(S, N), 0.0);
	
	if (sDotN > 0.0)
	{
		float A = Attenuation(L.Att, P);
		Diffuse += sDotN * L.Ld / A; //Diffuse
		
		vec3 V = normalize(-Pos); //View Vector
		vec3 H = normalize(V + S); //HalfWay vector
		Specular += pow(max(dot(H, N), 0.0), Shininess) * L.Ls / A; //Specular
	}
}

void PhongModel(SpotLightInfo L, vec3 N, vec3 Pos, float Shininess, inout vec3 Diffuse, inout vec3 Specular)
{
	vec3 P = L.Pos - Pos;
	vec3 S = normalize(P); //Light direction vector
	
	float SpotLightFactor = 0.0;
	float sDotDir = dot(-S, L.Dir); //Need to calc Angle and SpotFactor
	float Angle = acos(sDotDir);
	float CutOff = radians(L.Cut);
	if(Angle < CutOff) //In the cone area
		SpotLightFactor = pow(sDotDir, L.Exp);
	
	float sDotN = max(dot(S, N), 0.0);
	
	if (sDotN > 0.0)
	{
		float A = Attenuation(L.Att, P);
		Diffuse += sDotN * L.Ld * SpotLightFactor / A; //Diffuse
		
		vec3 V = normalize(-Pos); //View Vector
		vec3 H = normalize(V + S); //HalfWay vector
		Specular += pow(max(dot(H, N), 0.0), Shininess) * L.Ls * SpotLightFactor / A; //Specular
	}
}

vec3 CalcLight(vec3 Position, vec3 Normal, vec3 Kd, vec3 Ks, vec3 Ka, vec3 Ke, float Shininess)
{
	vec3 AccumDiffuse = vec3(0.0);
	vec3 AccumSpecular = vec3(0.0);
	
	// Directional Lights
	for(int i = 0; i < Max_DirectionalLights; i++)
	{
		if (i < LDcount)
			PhongModel(LightD[i], Normal, Position, Shininess, AccumDiffuse, AccumSpecular);
		else
			i = Max_DirectionalLights;
	}
	
	// Point Lights
	for(int i = 0; i < Max_PointLights; i++)
	{
		if (i < LPcount)
			PhongModel(LightP[i], Normal, Position, Shininess, AccumDiffuse, AccumSpecular);
		else
			i = Max_PointLights;
	}
	
	// Spot Lights
	for(int i = 0; i < Max_SpotLights; i++)
	{
		if (i < LScount)
			PhongModel(LightS[i], Normal, Position, Shininess, AccumDiffuse, AccumSpecular);
		else
			i = Max_SpotLights;
	}
	
	return	LightA * Ka + //Ambient
			AccumDiffuse * Kd + //Diffuse
			AccumSpecular * Ks + //Specular
			Ke; //Emissive
}