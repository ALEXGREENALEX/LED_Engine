#version 330

in vec3 f_VertexPosition;
in vec2 f_TexCoord;
in vec4 f_Normal;
in mat4 f_ModelMatrixT;

uniform sampler2D MainTexture;

uniform vec3 LightPosition;
uniform vec3 LightDiffuseColor;

out vec4 FragColor;

#include("Fog\VarsFS.glsl")

//Коэфициенты затухания света
const float k_ConstAttenuation = 0.5;
const float k_LinearAttenuation = 0.05;
const float k_QuadricAttenuation = 0.001;

void main()
{
	vec4 worldpos = vec4(f_VertexPosition, 1.0) * f_ModelMatrixT;
	vec4 LightVector = normalize(worldpos - vec4(LightPosition, 1.0));
	
	vec3 LightColor = LightDiffuseColor * dot(-LightVector, f_Normal);
	
	float distance = length(LightVector);
	float LightIntensity = 2.0;
	// Расчет коэф. затухания
    float attenuation = LightIntensity / (k_ConstAttenuation + k_LinearAttenuation * distance + k_QuadricAttenuation * distance * distance);
	
    vec4 Color = texture(MainTexture, f_TexCoord);
	Color.rgb *= attenuation;
    FragColor = Color * vec4(LightColor, 0.0);
	
	#include("Fog\FS.glsl")
}
/*
    vec3  L = LightPosition - v_FragmentPosition;

    // ** Используется при расчете затухания света
    float distance = length(L);

    // ** Расчет коэф. затухания
    float attenuation = 1.0 / (k_ConstAttenuation + k_LinearAttenuation * distance + k_QuadricAttenuation * distance * distance);
  
    vec4 diffuseColor  *= attenuation;
    vec4 specularColor *= attenuation;
*/