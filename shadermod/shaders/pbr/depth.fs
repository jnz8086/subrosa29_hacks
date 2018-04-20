#version 330 core

in vec3 viewvec;

out vec4 fragcolor;

void main()
  {
  vec4 color;
  float fragdepth;

  fragdepth=length(viewvec);
  color.rgb=vec3(fragdepth*0.0000152587890625,fragdepth*0.00390625,fragdepth);
  color.rgb=fract(color.rgb);
  color.rgb*=256.0;
  color.rgb=floor(color.rgb);
  color.rgb/=255.0;

  color.a=1.0;

  fragcolor=color;
  }