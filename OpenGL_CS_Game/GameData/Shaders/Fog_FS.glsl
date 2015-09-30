#version 430

in vec3 Position;
in vec3 Normal;

struct LightInfo {
    vec4 Position;
    vec3 Intensity;
};
uniform LightInfo Light;

struct FogInfo {
  float MaxDist;
  float MinDist;
  vec3 Color;
};
uniform FogInfo Fog;

struct MaterialInfo {
  vec3 Kd;            // Diffuse reflectivity
  vec3 Ka;            // Ambient reflectivity
  vec3 Ks;            // Specular reflectivity
  float Shininess;    // Specular shininess factor
};
uniform MaterialInfo Material;

uniform bool FogEnabled;

layout( location = 0 ) out vec4 FragColor;

vec3 ads( )
{
    vec3 s = normalize( Light.Position.xyz - Position.xyz );
    vec3 v = normalize(vec3(-Position));
    vec3 h = normalize( v + s );

    vec3 ambient = Material.Ka;
    vec3 diffuse = Material.Kd * max(0.0, dot(s, Normal) );
    vec3 spec = Material.Ks * pow( max( 0.0, dot( h, Normal) ), Material.Shininess );

    return Light.Intensity * (ambient + diffuse + spec);
}

void main() {
	if(FogEnabled)
	{
		//float dist = abs( Position.z );
		float dist = length(Position);
		float fogFactor = (Fog.MaxDist - dist) /
						(Fog.MaxDist - Fog.MinDist);
		fogFactor = clamp(fogFactor, 0.0, 1.0);
		vec3 shadeColor = ads();
		//vec3 color = mix(Fog.Color, shadeColor, fogFactor);
		vec3 color = mix(Fog.Color, vec3(0.3, 0.8, 0.1), fogFactor);
		FragColor = vec4(color, 1.0);
	}
	else
		FragColor = vec4(ads(), 1.0);
}
