#version 430

layout (location = 0) in vec3 VertexPosition;
layout (location = 1) in vec3 VertexNormal;
layout (location = 2) in vec2 VertexTexCoord;
layout (location = 3) in vec3 VertexTangent;
layout (location = 4) in vec3 VertexBitangent;

out vec3 LightDir;
out vec2 TexCoord;
out vec3 ViewDir;

uniform vec4 EyeLightPosition; // Light position in eye coords.

uniform mat4 ModelViewMatrix;
uniform mat3 NormalMatrix;
uniform mat4 MVP;

#include("Fog\VarsVS.glsl")

void main()
{
    // Transform normal and tangent to eye space
    vec3 N = normalize(NormalMatrix * VertexNormal);
    vec3 T = normalize(NormalMatrix * VertexTangent);
    vec3 B = normalize(NormalMatrix * VertexBitangent);

    // Matrix for transformation to tangent space
    mat3 toObjectLocal = mat3(
        T.x, B.x, N.x,
        T.y, B.y, N.y,
        T.z, B.z, N.z);

    // Transform light direction and view direction to tangent space
    vec3 pos = vec3(ModelViewMatrix * vec4(VertexPosition, 1.0));
    LightDir = normalize(toObjectLocal * (EyeLightPosition.xyz - pos));

    ViewDir = toObjectLocal * normalize(-pos);
    TexCoord = VertexTexCoord;
    gl_Position = MVP * vec4(VertexPosition, 1.0);
	#include("Fog\VS.glsl")
}