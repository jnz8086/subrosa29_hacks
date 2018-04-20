#version 330 core

uniform sampler2D tex_color;
uniform sampler2D tex_shadowmap;
uniform samplerCube tex_cubemap;

uniform vec3 lightambient;
uniform vec3 lightdiffuse;

uniform vec2 ooresolution;
uniform vec4 atmoconstant;
uniform vec3 atmowavelength;

uniform int isreflection;
uniform vec3 planeposition;
uniform vec3 planenormal;

in vec3 texcoord;
in vec3 lightvec;
in vec3 viewvec;
in vec3 normalvec;
in vec3 wsposition;
  
out vec4 glfragcolor;

vec4 texturefourtap(in vec4 gltexcoord)
  {
  int dx,dy;
  vec4 color;
  vec4 colortemp;
  vec2 offset;
  vec2 taptexcoord;
  float totalweight;
  float weight;
  float nearweight;
  
  color=vec4(0.0);  
  totalweight=0.0;
  nearweight=0.0;
  for (dy=0;dy<2;dy++)
  for (dx=0;dx<2;dx++)
    {
    offset=vec2(dx,dy);
    offset+=vec2(0.5);
    taptexcoord=2.0*fract((gltexcoord.xy+offset)*0.5);
    weight=pow(1.0-max(abs(taptexcoord.x-1.0),abs(taptexcoord.y-1.0)),16.0);
    taptexcoord=taptexcoord*0.0625*0.5;
    taptexcoord+=gltexcoord.zw;
    
    colortemp=texture(tex_color,taptexcoord);
	  if (nearweight<weight)
      {
      color=colortemp;
      nearweight=weight;      
      }
    }

  return(color);
  }
  
void main()
  {
  int count;
  vec3 lightnormal;
  vec3 viewnormal;
  vec3 reflectvec;
  vec4 color;
  vec4 color2;
  vec4 color3;
  vec3 texcolor;
  vec4 normal;
  vec3 position;
  vec4 taptexcoord;
  float scale,scale2;
  float rho,k;
  float height;
  vec2 shadowmaptexcoord;
  float shadow;
  float occlusion;
  float shine;
  float shine2;
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

  lightnormal=normalize(lightvec);
  viewnormal=normalize(viewvec);
  
  taptexcoord.xy=texcoord.xy;
  taptexcoord.z=fract(texcoord.z);
  taptexcoord.w=floor(texcoord.z)*0.0625;
  color=texturefourtap(taptexcoord);
  texcolor=color.rgb;
  shine=color.a;

  normal.xyz=normalize(normalvec);
  
  scale=dot(lightnormal,normal.xyz);
  color.rgb=texcolor*occlusion*lightambient*vec3(scale*0.25+0.75);
  scale=max(scale,0.0);
  scale=shadow*scale*0.75;
  color.rgb+=(texcolor*vec3(0.25)+vec3(0.25))*lightdiffuse*vec3(scale);

  if (shine>0.0)
    {
    color3.rgb=color.rgb;
	
    scale2=1.0-abs(dot(viewnormal,normal.xyz));
    scale2=pow(scale2,2.5);  
    scale2=0.5+scale2*(1.0-0.5);    

    reflectvec=reflect(viewnormal,normal.xyz);
    //reflectvec.x=-reflectvec.x;
    reflectvec.y=-reflectvec.y;
    reflectvec.z=-reflectvec.z;
    reflectvec=normalize(reflectvec);
      
    color2.rgb=texture(tex_cubemap,reflectvec).rgb;
    
    color2.rgb=color2.rgb*vec3(scale2)+lightambient*vec3((1.0-scale2)*0.5);
    
      
    shine2=(shine-0.5)*2.0;
    //shine+=0.5;
    //shine=min(shine,1.0);
    //shine=1.0;
    //color.rgb=vec3(0.125);
    //color.rgb=mix(color.rgb,vec3(0.25),shine2);
    //color.rgb=mix(color.rgb,color2.rgb,shine2);

    color.rgb=mix(color.rgb,color2.rgb,shine);
    //color.rgb=mix(color.rgb,color3.rgb,1.0-shine);

    //color.rgb=vec3(0.0,1.0,1.0);
    }

  color.a=1.0;

  glfragcolor=color;
  }