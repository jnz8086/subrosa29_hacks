#version 330 core
#extension GL_NV_shadow_samplers_cube : enable

uniform sampler2D tex_color;
uniform sampler2D tex_shadowmap;
uniform samplerCube tex_cubemap;

uniform vec2 ooresolution;

uniform vec3 lightambient;
uniform vec3 lightdiffuse;

uniform float metallic;

in vec3 lightvec;
in vec3 viewvec;
in vec3 normalvec;
in vec2 texcoord;

out vec4 fragcolor;

void main()
  {
	vec3 lightambient2 = lightambient*vec3(0.8);
	vec3 lightdiffuse2 = lightdiffuse*vec3(0.8);
	
	vec3 albedo = texture(tex_color, texcoord).rgb;
	float ao = 0.8;
	
	vec3 V = normalize(viewvec);
	vec3 L = normalize(lightvec);
	
	vec3 N0 = normalize(normalvec);
	vec3 N = N0;
	
	float roughness = 0.5;
	//float metallic = 0.25;
	
	vec2 shadowmaptexcoord=gl_FragCoord.xy*ooresolution;
	float shadow=texture(tex_shadowmap,shadowmaptexcoord).r;
	
	
	
	
	
	
	
  const vec3 color0=vec3( 0.04,0.04,0.04 );
	float glosiness=1.0-roughness;
	vec3 diffuse=albedo; //* (1.0-metallic);
	vec3 specular=(albedo-color0) * metallic + color0;
	//vec3 vvec=normalize( -wsposition );
	vec3 vvec = -V;
	float ndotv=dot( N,vvec );
	
   vec3 color = vec3(0.0);
	
	/*vec3 rvec=reflect(V,N);
	rvec=normalize(rvec);
	rvec.y=-rvec.y;
	rvec.z=-rvec.z;*/
	vec3 rvec=reflect(V,N);
	rvec.x=-rvec.x;
	rvec=normalize(rvec);
	
	vec3 ambEnv=pow( textureCube(tex_cubemap,rvec).rgb,vec3( 2.2 ) );
	//ambEnv = mix(lightdiffuse, ambEnv, (metallic+roughness)*0.5);
	//ambEnv = mix( ambEnv, lightdiffuse2, (metallic) );
	
	
	//ambEnv *= min(shadow+0.2, 1.0);
	ambEnv *= 0.07 * metallic;
	
	vec3 fschlick0=specular + (1.0-specular) * pow( 1.0-ndotv,5.0 ) * glosiness;
	vec3 ambDiffuse=diffuse * lightambient2;
	vec3 ambSpecular=fschlick0 * ambEnv;
	color+=( ambDiffuse + ambSpecular ) * ao; //+ emissive;
	
	
	
	float spow=pow( 2.0,glosiness * 12.0 );
	float fnorm=(spow+2.0)/8.0;
	
	//vec3 lvec=normalize( -lightvec );
	float atten=1.0;
	
	vec3 H = normalize(V + L);
	
	float ndotl=max( dot( N,L ),0.0 );
	float ndoth=max( dot( N,H ),0.0 );
	float hdotl=max( dot( H,L ),0.0 );
	vec3 fschlick = specular + (1.0-specular) * pow( 1.0-hdotl,5.0 ) * glosiness;
	vec3 fspecular = fschlick * pow( ndoth,spow ) * fnorm;
	vec3 light=lightdiffuse2 * ndotl * atten;
	light=(diffuse+fspecular) * light;
	
	light *= shadow;
	
	color += light;
	
	
	
	
	

  fragcolor=vec4(color,1.0);
  }