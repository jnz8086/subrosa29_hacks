#version 330 core

uniform mat4 modelviewprojectionmatrix;
uniform mat4 modelviewmatrix;

layout(location=0) in vec3 position_in;

out vec3 viewvec;

void main()
  {
  vec4 position;

  position=modelviewmatrix*vec4(position_in,1.0);
  viewvec=position.xyz;

  gl_Position=modelviewprojectionmatrix*vec4(position_in,1.0);
  }