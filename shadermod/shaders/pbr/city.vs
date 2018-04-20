#version 330 core

uniform vec3 viewposition;
uniform vec3 lightdirection;

uniform mat4 modelviewprojectionmatrix;

layout(location=0) in vec3 invertex;
layout(location=1) in vec3 intexcoord;
layout(location=2) in vec3 innormal;

out vec2 texcoord;
out vec3 lightvec;
out vec3 viewvec;
out vec3 normalvec;
out vec3 wsposition;

void main()
  {
  wsposition=invertex;
  
  viewvec=invertex-viewposition;
  
  normalvec=innormal;
  lightvec=lightdirection;

  texcoord=intexcoord.xy;
  gl_Position=modelviewprojectionmatrix*vec4(invertex,1.0);
  }