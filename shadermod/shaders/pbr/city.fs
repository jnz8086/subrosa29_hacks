#version 330 core
#extension GL_NV_shadow_samplers_cube : enable

uniform sampler2D tex_color;
uniform sampler2D tex_shadowmap;
uniform sampler2D tex_gloss;
uniform samplerCube tex_cubemap;
uniform sampler2D tex_pbr;

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

const float PI = 3.14159265359;



mat3 cotangent_frame( vec3 N, vec3 p, vec2 uv )
{
    // get edge vectors of the pixel triangle
    vec3 dp1 = dFdx( p );
    vec3 dp2 = dFdy( p );
    vec2 duv1 = dFdx( uv );
    vec2 duv2 = dFdy( uv );
 
    // solve the linear system
    vec3 dp2perp = cross( dp2, N );
    vec3 dp1perp = cross( N, dp1 );
    vec3 T = dp2perp * duv1.x + dp1perp * duv2.x;
    vec3 B = dp2perp * duv1.y + dp1perp * duv2.y;
 
    // construct a scale-invariant frame 
    float invmax = inversesqrt( max( dot(T,T), dot(B,B) ) );
    return mat3( T * invmax, B * invmax, N );
}


void main()
  {
	vec3 lightambient2 = lightambient*vec3(0.8);
	vec3 lightdiffuse2 = lightdiffuse*vec3(0.8);
	
	//glfragcolor = texture(tex_gloss, texcoord);
	//glfragcolor = mix( texture(tex_gloss, texcoord), texture(tex_color,texcoord), 0.5 );
	//glfragcolor = texture(tex_pbr, texcoord);
	//return;
	
	vec3 albedo = texture(tex_color, texcoord).rgb;
	//vec3 albedo = vec3(0.8);
	vec4 pbr = texture(tex_pbr, texcoord);
	
	float ao = pbr.a   *0.25+0.5;
	
	vec3 V = normalize(viewvec);
	vec3 L = normalize(lightvec);
	
	vec3 N0 = normalize(normalvec);
	vec3 NM = normalize( texture( tex_gloss, texcoord ).rgb * 2.0 - 1.0 );
	//vec2 nm_tc = sec_tc+vec2(0.5,0.5);
	//vec3 NM = normalize( textureGrad( tex_gloss, nm_tc, dFdx(texcoord)*0.5, dFdy(texcoord)*0.5 ).rgb * 2.0 - 1.0 );
		 NM.y = -NM.y;
	mat3 TBN = cotangent_frame(N0, -V, texcoord);
	vec3 N = normalize(TBN * NM);
	//vec3 N = N0;
		N = mix(N0,N,0.5);
	
	float roughness = pbr.g;
	float metallic = pbr.b;
	
	vec2 shadowmaptexcoord=gl_FragCoord.xy*ooresolution;
	float shadow=texture(tex_shadowmap,shadowmaptexcoord).r;
	
		//L = -L;
	
	
  const vec3 color0=vec3( 0.04,0.04,0.04 );
	float glosiness=1.0-roughness;
	vec3 diffuse=albedo; //* (1.0-metallic);
	vec3 specular=(albedo-color0) * metallic + color0;
	//vec3 vvec=normalize( -wsposition );
	vec3 vvec = -V;
	float ndotv=dot( N,vvec );
	
   vec3 color = vec3(0.0);
	
	vec3 rvec=reflect(V,N);
	rvec=normalize(rvec);
	rvec.y=-rvec.y;
	rvec.z=-rvec.z;
	
	vec3 ambEnv=pow( textureCube(tex_cubemap,rvec).rgb,vec3( 2.2 ) );
	//ambEnv = mix(lightdiffuse, ambEnv, (metallic+roughness)*0.5);
	ambEnv = mix( ambEnv, lightdiffuse, (roughness)*0.5+0.35 );
	
	ambEnv *= min(shadow+0.3, 1.0);
	//ambEnv *= shadow;
	
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
	
	
	glfragcolor = vec4( color, 1.0 );
  }











