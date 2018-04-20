#version 330 core

uniform mat4 modelmatrix;

layout(location=0) in vec3 vertexposition;

void main()  
  {
  gl_Position=vec4(vertexposition,1.0);
  }