#version 430

in vec3 ReflectDir;

layout(binding=0) uniform samplerCube CubeMapTex;

uniform float ReflectFactor;
uniform vec4 MaterialColor;

layout( location = 0 ) out vec4 FragColor;

void main()
{
    vec4 cubeMapColor = texture(CubeMapTex, ReflectDir);
    FragColor = mix( MaterialColor, cubeMapColor, ReflectFactor);
}
