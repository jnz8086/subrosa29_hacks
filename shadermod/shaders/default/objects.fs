#version 330 core

uniform sampler2D tex_color;
uniform sampler2D tex_shadowmap;
uniform samplerCube tex_cubemap;

uniform vec2 ooresolution;

uniform vec3 lightambient;
uniform vec3 lightdiffuse;

in vec3 lightvec;
in vec3 viewvec;
in vec3 normalvec;
in vec2 texcoord;

out vec4 fragcolor;

void main()
  {
  int count;
  vec3 lightnormal;
  vec3 viewnormal;
  vec3 reflectvec;
  vec4 color;
  vec3 ambientcolor;
  vec3 diffusecolor;
  vec4 normal;
  float scale;
  float rho,k;
  float height;
  vec2 shadowmaptexcoord;
  float shadow;
  float occlusion;
  float shine;
  float fresnel;

  shadowmaptexcoord=gl_FragCoord.xy*ooresolution;
  shadow=texture(tex_shadowmap,shadowmaptexcoord).r;
  occlusion=0.5;//texture(tex_shadowmap,shadowmaptexcoord).g;

  lightnormal=normalize(lightvec);
  viewnormal=normalize(viewvec);

  ambientcolor=texture(tex_color,texcoord).rgb;
  shine=texture(tex_color,texcoord).a;
  diffusecolor=ambientcolor;
  
  normal.xyz=normalize(normalvec);
  
  //shine=0.125;

  scale=abs(dot(viewnormal,normal.xyz));
  scale=1.0-scale;
  scale*=scale;
  shine=shine+(1.0-shine)*scale;
  
  scale=dot(lightnormal,normal.xyz);
  color.rgb=ambientcolor*vec3(1.5)*occlusion*lightambient*vec3(scale*0.125+0.875);
  scale=max(scale,0.0);
  scale=shadow*scale;//*0.25;
  color.rgb+=diffusecolor*lightdiffuse*vec3(scale);
  
  reflectvec=reflect(lightnormal,normal.xyz);
  reflectvec=normalize(reflectvec);
  scale=dot(reflectvec,viewnormal);
  scale=max(scale,0.0);
  scale=pow(scale,32.0);
  scale=shadow*scale*shine;
  //color.rgb+=scale*lightdiffuse;  
  
  
  reflectvec=reflect(viewnormal,normal.xyz);
  reflectvec.x=-reflectvec.x;
  reflectvec=normalize(reflectvec);
  scale=clamp(reflectvec.y+0.25,0.25,1.0);
  //color.rgb=mix(texture(tex_cubemap,reflectvec).rgb,color.rgb,scale);

  //shine=0.5+shine*0.5;
  //color.rgb+=texture(tex_cubemap,reflectvec).rgb*(1.0-scale)*shine;

  color.a=1.0;

  fragcolor=color;
  }