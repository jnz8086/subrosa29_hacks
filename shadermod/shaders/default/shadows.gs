#version 330 core

layout(triangles_adjacency) in;
layout(triangle_strip,max_vertices=18) out;

uniform mat4 viewprojectionmatrix;
uniform mat4 projectionmatrix;
uniform mat4 viewmatrix;
uniform mat4 modelmatrix;
uniform vec3 lightdirection;
uniform float shadowlength;

void main()  
  {
  vec4 position;
  vec3 normal[3];
  vec3 edgenormal[3];
  vec4 shadowvert[6];
  bool faceslight,edgefaceslight; 
  vec3 lightvec;
  
  normal[0]=cross(gl_in[4].gl_Position.xyz-gl_in[0].gl_Position.xyz,gl_in[2].gl_Position.xyz-gl_in[0].gl_Position.xyz);
  normal[1]=cross(gl_in[0].gl_Position.xyz-gl_in[2].gl_Position.xyz,gl_in[4].gl_Position.xyz-gl_in[2].gl_Position.xyz);
  normal[2]=cross(gl_in[2].gl_Position.xyz-gl_in[4].gl_Position.xyz,gl_in[0].gl_Position.xyz-gl_in[4].gl_Position.xyz);
  faceslight=false;
  if (dot(normal[0],lightdirection)>=0.0 || dot(normal[1],lightdirection)>=0.0 || dot(normal[2],lightdirection)>=0.0)
    faceslight=true;
    
  if (faceslight==false)
    {
    lightvec=lightdirection*vec3(-shadowlength);

    shadowvert[0]=viewprojectionmatrix*gl_in[0].gl_Position;
    shadowvert[1]=viewprojectionmatrix*gl_in[2].gl_Position;
    shadowvert[2]=viewprojectionmatrix*gl_in[4].gl_Position;
    //shadowvert[0]=projectionmatrix*viewmatrix*modelmatrix*gl_in[0].gl_Position;
    //shadowvert[1]=projectionmatrix*viewmatrix*modelmatrix*gl_in[2].gl_Position;
    //shadowvert[2]=projectionmatrix*viewmatrix*modelmatrix*gl_in[4].gl_Position;
    position=gl_in[0].gl_Position;
    position.xyz+=lightvec;
    shadowvert[3]=viewprojectionmatrix*position;
    //shadowvert[3]=projectionmatrix*viewmatrix*modelmatrix*position;
    position=gl_in[2].gl_Position;
    position.xyz+=lightvec;
    shadowvert[4]=viewprojectionmatrix*position;
    //shadowvert[4]=projectionmatrix*viewmatrix*modelmatrix*position;
    position=gl_in[4].gl_Position;
    position.xyz+=lightvec;
    shadowvert[5]=viewprojectionmatrix*position;
    //shadowvert[5]=projectionmatrix*viewmatrix*modelmatrix*position;

    edgenormal[0]=cross(gl_in[2].gl_Position