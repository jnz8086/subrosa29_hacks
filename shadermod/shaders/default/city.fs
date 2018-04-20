#version 330 core

uniform sampler2D tex_color;
uniform sampler2D tex_shadowmap;
uniform sampler2D tex_gloss;
uniform samplerCube tex_cubemap;

uniform vec3 lightambient;
uniform vec3 lightdiffuse;

uniform vec2 ooresolution;
uniform vec4 atmoconstant;
uniform vec3 atmowavelength;

uniform int isreflection;
uniform vec3 planeposition;
uniform vec3 planenormal;

in vec2 texcoord;
in vec3 lightvec;
in vec3 viewvec;
in vec3 normalvec;
in vec3 wsposition;

out vec4 glfragcolor;
  
void main()
  {
  int count;
  vec3 lightnormal;
  vec3 viewnormal;
  vec3 reflectvec;
  vec4 color;
  vec4 color2;
  vec3 texcolor;
  vec4 normal;
  vec3 position;
  float scale,scale2;
  float rho,k;
  float height;
  vec2 shadowmaptexcoord;
  float shadow;
  float occlusion;
  float shine;
  float specpow;
  float dist;
  
  if (isreflection==1)
    {
    position=wsposition-planeposition;
    if (dot(position,planenormal)<0.0)
      discard;
    }

  shadowmaptexcoord=gl_FragCoord.xy*ooresolution;
  shadow=texture(tex_shadowmap,shadowmaptexcoord).r;
  occlusion=0.5;//texture(tex_shadowmap,shadowmaptexcoord).g;
  if (isreflection==1)
    {
    shadow=0.5;
    occlusion=0.5;
    }

  lightnormal=normalize(lightvec);
  viewnormal=normalize(viewvec);

  texcolor=texture(tex_color,texcoord).rgb;
  //texcolor*=vec3(1.0,0.5,0.5);
  shine=texture(tex_color,texcoord).a;
  normal.xyz=normalize(normalvec);
  
  scale=dot(lightnormal,normal.xyz);
  color.rgb=texcolor*occlusion*lightambient*vec3(scale*0.25+0.75);
  scale=max(scale,0.0);
  scale=shadow*scale*0.75;
  color.rgb+=(texcolor*vec3(0.25)+vec3(0.25))*lightdiffuse*vec3(scale);

  shine=texture(tex_gloss,texcoord).r*2.0;
  //shine*=0.5;
  //shine=0.125;


  specpow=1.0/(shine+0.0625);
  specpow=32.0/specpow;
  
  shine=0.5;
  specpow=16.0;

  scale=abs(dot(viewnormal,normal.xyz));
  scale=1.0-scale;
  scale*=scale;
  //shine=shine+(1.0-shine)*scale*shine;

  
  
  //reflectvec=reflect(lightnormal,normal.xyz);
  reflectvec=lightnormal-viewnormal;
  reflectvec=normalize(reflectvec);
  //scale=dot(reflectvec,viewnormal);
  scale=dot(reflectvec,normal.xyz);
  scale=max(scale,0.0);
  scale=pow(scale,specpow);
  scale=shadow*scale*shine;
  if (dot(lightnormal,normal.xyz)<=0.0)
    scale=0.0;
//color.rgb+=scale*lightdiffuse;  
//glfragcolor = vec4(1.0,1.0,1.0,1.0);//ctvec).rgb*shine;

  color.a=1.0;

  glfragcolor=color;
  }