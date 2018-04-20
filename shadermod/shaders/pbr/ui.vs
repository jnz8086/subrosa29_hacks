#version 330 core
uniform mat4 modelviewprojectionmatrix;

layout(location=0) in vec3 vertexposition;
layout(location=1) in vec2 vertextexcoord;
layout(location=2) in vec4 vertexcolor;

out vec2 texcoord;
out vec4 color;

void main()
  {
  color=vertexcolor;
  texcoord=vertextexcoord;
  gl_Position=modelviewprojectionmatrix*vec4(vertexposition,1.0);
  }