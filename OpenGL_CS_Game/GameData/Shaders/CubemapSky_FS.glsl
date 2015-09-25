#version 430

in vec3 ReflectDir;

layout(binding=0) uniform samplerCube CubemapTex;
layout( location = 0 ) out vec4 FragColor;

void main() {
    // Access the cube map texture
    FragColor = texture(CubemapTex, ReflectDir);
}
