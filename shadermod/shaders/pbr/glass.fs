#version 330 core
#extension GL_NV_shadow_samplers_cube : enable

uniform sampler2D tex_color;
uniform samplerCube tex_cubemap;

uniform vec3 lightambient;
uniform vec3 lightdiffuse;

in vec2 texcoord;
in vec3 lightvec;
in vec3 viewvec;
in vec3 normalvec;
  
out vec4 glfragcolor;

void main()
  {
  int count;
  vec3 lightnormal;
  vec3 viewnormal;
  vec3 reflectvec;
  vec3 position;
  vec4 color;
  vec3 texcolor;
  vec3 normal;
  float scale,scale2;
  float rho,k;
  float height;
  float shine;
  float dist;

  lightnormal=normalize(lightvec);
  viewnormal=normalize(viewvec);

  //texcolor=vec3(0.5,0.5,0.5);
  texcolor=texture(tex_color,texcoord).rgb;
  shine=texture(tex_color,texcoord).a;
  //shine=0.5;
  normal=normalize(normalvec);
  
  color.rgb=texcolor*vec3(0.25)*lightambient;
  
  //reflectvec=reflect(lightnormal,normal);
  //reflectvec=normalize(reflectvec);
  //scale=dot(reflectvec,viewnormal);
  //scale=max(scale,0.0);
  //scale=pow(scale,8.0);
  
  reflectvec=reflect(viewnormal,normal);
  reflectvec=normalize(reflectvec);

  scale=(length(viewvec)-32.0)*0.125*0.125*0.75;
  if (scale<0.0)
    scale=0.0;
  if (scale>1.0)
    scale=1.0;
    
    
  scale2=1.0-abs(dot(viewnormal,normal));
  scale2=pow(scale2,3.0);
  scale2=0.5+scale2*0.5;
  if (scale2>1.0)
    scale2=1.0;
  if (scale2+scale>1.0)
    scale=1.0-scale2;
  
  
  reflectvec.y=-reflectvec.y;
  reflectvec.z=-reflectvec.z;
  //reflectvec.x=0.0;
  //reflectvec.y=-1.0;
  //reflectvec.z=0.0;
  //color.rgb=texture(tex_cubemap,reflectvec).rgb;

  
  
	vec3 ref;
	vec3 ref_kern[6] = vec3[]( vec3(-0.0037, 0.00022, 0.00018),
							   vec3(-0.0004, 0.00028, -0.0003),
							   vec3(0.00046, -0.0001, 0.00023),
							   vec3(0.00027, -0.1896, -0.0002),
							   vec3(0.00044, 0.00043, -0.8096),
							   vec3(0.00035, 0.00046, -0.0002));
	for(int i=0; i<6; i++){
		ref += texture(tex_cubemap, reflectvec+ref_kern[i]*0.02 ).rgb;
	}
	ref /= 6.0;
  
  color.rgb = mix(ref, lightambient, dot(lightnormal,viewnormal)*0.5+0.5 );
  
  
  
  
  
  
  
  color.rgb=color.rgb*vec3(scale2)+lightambient*vec3(scale*0.5);

  scale2+=scale;

  color.a=scale2;

  glfragcolor=color*vec4(1.03,1.1,0.9,1.0);
  }