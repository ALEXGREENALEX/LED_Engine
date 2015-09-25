#version 430

in vec3 ReflectDir;
in vec3 RefractDir;
in float reflectionFactor; // Percentage of reflected light

layout(binding=0) uniform samplerCube CubeMapTex;

layout( location = 0 ) out vec4 FragColor;

void main()
{
    vec4 reflectColor = texture(CubeMapTex, ReflectDir);
    vec4 refractColor = texture(CubeMapTex, RefractDir);

    FragColor = mix(refractColor, reflectColor, reflectionFactor);
}