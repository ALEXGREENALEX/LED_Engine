#version 330

in vec4 eyeCordFs;
in vec4 eyeNormalFs;

uniform vec4 lightPosition;

layout (location = 0) out vec3 outputColor;

void main()
{
    vec4 s = normalize(lightPosition - eyeCordFs);
    vec4 r = reflect(-s, eyeNormalFs);
    vec4 v = normalize(-eyeCordFs);
    float spec = max(dot(v, r), 0.0);
    float diff = max(dot(eyeNormalFs, s), 0.0);

    vec3 diffColor = diff * vec3(1, 0, 0);
    vec3 specColor = pow(spec, 3) * vec3(1, 1, 1);
    vec3 ambientColor = vec3(0.1, 0.1, 0.1);

    outputColor = diffColor + 0.5 * specColor + ambientColor; 
}
/*
#version 330

in vec2 f_texcoord;
in vec3 f_color;
uniform sampler2D MainTexture;

out vec4 FragColor;

void main()
{
    vec4 Color = texture(MainTexture, f_texcoord);
    FragColor = Color * vec4(f_color, 0.0) * 4.0;
}
*/