#version 330 core

uniform sampler2D tex_normal;
uniform sampler2D tex_detail;
uniform sampler2D tex_color;

uniform vec3 lightambient;
uniform vec3 lightdiffuse;

in vec2 texcoord0;
in vec2 texcoord1;
in vec3 lightvec;
in vec3 viewvec;

out vec4 fragcolor;

void main()
  {
  vec4 color;
  vec4 normal;
  vec3 lightnormal;
  vec3 viewnormal;
  vec3 reflectvec;
  float scale;
  float shine;
  float rho,k;
  
  rho=3.0;
  k=(1.0+rho)/rho;
  
  lightnormal=normalize(lightvec);
  viewnormal=normalize(viewvec);
  
  normal=texture(tex_normal,texcoord0);
  normal.xyz-=0.5;
  normal.xyz=normalize(normal.xyz);
  
  scale=dot(normal.xyz,lightnormal);
  scale=max(scale,0.0);
  scale=0.25+scale*0.75;
  scale=k*(1.0-1.0/(1.0+rho*scale));  
  
  color.rgb=texture(tex_detail,texcoord1).rgb;
  //color.rgb*=vec3(0.5,0.25,0.125);
  color.rgb*=texture(tex_color,texcoord0).rgb;
  color.rgb=color.rgb*lightambient*0.5+color.rgb*lightdiffuse*scale*0.75;
  //color.rgb*=scale;
  
  shine=0.0625+(1.0-texture(tex_detail,texcoord1.st).r)*0.5;
  scale=abs(dot(viewnormal,normal.xyz));
  scale=1.0-scale;
  scale*=scale;
  scale*=scale;
  shine=shine+(1.0-shine)*scale;
  shine*=0.5;

  reflectvec=reflect(lightnormal,normal.xyz);
  reflectvec=normalize(reflectvec);
  scale=dot(reflectvec,viewnormal);
  scale=max(scale,0.0);
  scale=pow(scale,2.0);
  scale=scale*shine;
  color.rgb+=scale*lightdiffuse;    

  color.a=1.0;  
    
  fragcolor=color;
  }