#version 330
layout (location = 0) in vec3 v_Position;
layout (location = 1) in vec3 v_Normal;
layout (location = 2) in vec2 v_UV;
layout (location = 3) in vec3 v_Tan;

uniform mat4 ModelView;
uniform mat4 MVP;
uniform vec3 LightPos;
uniform vec3 CameraPos;

out vec3 LightVector;
out vec3 EyeVector;
out vec2 f_UV;

void main()
{
	mat3 NormalMatrix = transpose(inverse(mat3(ModelView)));
	vec3 norm = normalize(NormalMatrix * v_Normal);
	vec3 tang = normalize(NormalMatrix * v_Tan);
	vec3 bitan = normalize(cross(norm, tang));
	
	vec3 vertex = vec3(ModelView * vec4(v_Position, 1.0));
	
	vec3 temp = LightPos - vertex;
	LightVector.x = dot(temp, tang);
	LightVector.y = dot(temp, bitan);
	LightVector.z = dot(temp, norm);
	
	temp = CameraPos - vertex;
	EyeVector.x = dot(temp, tang);
	EyeVector.y = dot(temp, bitan);
	EyeVector.z = dot(temp, norm);
  
  	f_UV = v_UV;
	gl_Position = MVP * vec4(v_Position, 1.0);
}
/*
#version 440

layout (location = 0) in vec3 v_Position;
layout (location = 1) in vec3 v_Normal;

out vec4 eyeCordFs;
out vec4 eyeNormalFs;

uniform mat4 M_Matrix;
uniform mat4 V_Matrix;
uniform mat4 NormalMatrix;
uniform mat4 MVP;

void main()
{
    mat4 normalMatrix = transpose(inverse(modelView));
    vec4 eyeNorm = normalize(normalMatrix * vec4(v_Normal, 0.0));
    vec4 eyeCord = ModelView * vec4(v_Position, 1.0);

    eyeCordFs = eyeCord;
    eyeNormalFs = eyeNorm;

    gl_Position = MVP * vec4(v_Position, 1.0);
}*/