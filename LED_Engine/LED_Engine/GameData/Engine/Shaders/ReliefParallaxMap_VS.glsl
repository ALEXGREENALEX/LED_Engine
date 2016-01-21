#version 330
layout(location = 0) in vec3 v_Position;
layout(location = 1) in vec3 v_Normal;
layout(location = 2) in vec2 v_UV;
layout(location = 3) in vec3 v_Tan;

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
	vec3 Norm = normalize(NormalMatrix * v_Normal);
	vec3 Tan = normalize(NormalMatrix * v_Tan);
	vec3 BiTan = normalize(cross(Norm, Tan));
	
	vec3 Pos = vec3(ModelView * vec4(v_Position, 1.0));
	
	vec3 temp = LightPos - Pos;
	LightVector.x = dot(temp, Tan);
	LightVector.y = dot(temp, BiTan);
	LightVector.z = dot(temp, Norm);
	
	temp = CameraPos - Pos;
	EyeVector.x = dot(temp, Tan);
	EyeVector.y = dot(temp, BiTan);
	EyeVector.z = dot(temp, Norm);
  
  	f_UV = v_UV;
	gl_Position = MVP * vec4(v_Position, 1.0);
}