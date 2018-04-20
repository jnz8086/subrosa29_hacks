#version 330 core

uniform sampler2D tex_depth;

uniform vec4 atmoconstant;
uniform vec3 atmowavelength;
uniform vec2 ooresolution;
uniform vec3 worldposition;

in vec3 lightvec;
in vec3 viewvec;

out vec4 fragcolor;

float raydistancesphere(in vec3 position,in vec3 direction,in float radius)
  {
  float scale;
  float a,b2;

  scale=dot(position,direction);
  b2=dot(position,position)-scale*scale;
  a=sqrt(radius*radius-b2);
  a-=scale;

  return a;
  }

float raydistancesphere2(in vec3 position,in vec3 direction,in float radius)
  {
  float scale;
  float a,b2;

  scale=-dot(position,direction);
  if (scale<0.0)
    return(0.0);
  b2=dot(position,position)-scale*scale;
  if (b2>radius*radius)
    return(0.0);

  a=sqrt(radius*radius-b2);
  a=scale-a;

  return(a);
  }  

void atmosphericscattering3(inout vec4 color,in vec3 position,in vec3 viewnormal,in vec3 lightnormal)
  {
  int count;
  vec3 rayposition;
  vec3 inscatter;
  vec3 mieinscatter;
  vec3 rayleigh;
  vec3 mie;
  vec3 attenuation;
  vec3 outscatter;
  vec3 mieoutscatter;
  vec3 worldvec;
  vec3 ext;
  float viewlength;
  float raylength;
  float phase;
  float miephase;
  float miecoef;
  float dist;
  float height;
  float density;
  float miedensity;  
  float oosamples;
  float lengthtemp;
  float g;
  float totaloutscatter;
  float scale;
  
  worldvec=worldposition;
  
  oosamples=1.0/4.0;
  
  miecoef=4.0;
  
  color.rgb=vec3(0.0);
  viewlength=length(position);
  //if (viewlength>1.0)
  //  viewlength=1.0;
  phase=dot(viewnormal,lightnormal);
  miephase=1.0+phase*0.125;
  g=0.1;
  miephase=(3.0*((1.0-g*g)*(1.0+phase*phase)))/(((2.0+g*g)*pow(1.0+g*g-2.0*g*phase,1.5))*(8.0*3.14159265359));
  //miephase*=0.5;
  //miephase*=0.75;
  //miephase=0.75+abs(phase)*phase*0.25;//*0.75;
  //miephase*=0.5*0.125;
  phase=3.0/(16.0*3.14159265359)*(1.0+phase*phase);
  //phase=0.75+phase*phase*0.25;
  //phase*=0.4375*0.25;
  
  //miephase=phase;
  //phase=1.0+phase*phase*0.5;//*0.75;
  //phase=max(phase,1.0);
  dist=1.0;//-oosamples*0.5;
  
  outscatter=1.0*viewlength*oosamples*atmowavelength*vec3(atmoconstant.x);
  mieoutscatter=viewlength*oosamples*vec3(atmoconstant.x);
  inscatter=1.0*1.0*viewlength*oosamples*phase*atmowavelength*vec3(atmoconstant.x);
  mieinscatter=1.5*viewlength*oosamples*miephase*vec3(atmoconstant.x);
  
  color.rgb=vec3(0.0);
  
  if (viewlength>4.0)
	{
	scale=dot(viewnormal,lightnormal)-0.9998;
	scale=max(scale,0.0)*262144.0;
	scale=min(scale,16.0);
	color.rgb=vec3(scale);
	}
  totaloutscatter=1.0;
  
  for (count=0;count<4;count++)
    {
    rayposition=worldvec+viewnormal*viewlength*dist;

    raylength=raydistancesphere(rayposition,lightnormal,atmoconstant.w+atmoconstant.z);
    
    //height=(length(rayposition+lightnormal*raylength*0.0625)-atmoconstant.w)/atmoconstant.z;
	height=(length(rayposition)-atmoconstant.w)/atmoconstant.z;
    height=1.0-height;
    height=clamp(height,0.0,1.0);
    height*=height;
    //height*=height;
	//height=pow(height,1.5);
	//height=exp(-height);
    density=height*8.0;
	
    height=(length(rayposition)-atmoconstant.w)/(atmoconstant.z*0.0625);//+lightnormal*raylength*0.0625
    //height=clamp(height,0.0,1.0);
    //height*=height;
    //height=0.125*0.25-height;
	//height*=0.125*0.25;
  	//height*=256.0;
    
    height=clamp(height,0.0,1.0);
    height=1.0-height;
    height*=height;
    height*=height;
	//height=pow(height,1.5);
    miedensity=1.0*height*miecoef*16.0;	

    rayleigh=12.0*exp(-raylength*density*0.25*atmoconstant.x*atmowavelength);
      
    //color.rgb*=(1.0-outscatter*density);
    color.rgb*=exp(-outscatter*density);
    
    ext=vec3(atmoconstant.x)*miedensity;
    mieoutscatter=viewlength*oosamples*ext;
    
    //color.rgb*=max(1.0-mieoutscatter,0.0);
    color.rgb*=exp(-mieoutscatter);
    //totaloutscatter*=(1.0-outscatter.b*density);
    totaloutscatter*=exp(outscatter.b*density);
    //totaloutscatter*=max(1.0-mieoutscatter.b,0.75);
    totaloutscatter*=exp(-mieoutscatter.b);
    //color.rgb+=rayleigh*mieinscatter*miedensity;//*exp(-mieoutscatter*miedensity*0.3125);
    //color.rgb+=min(rayleigh*mieinscatter*miedensity,0.5);//*exp(-mieoutscatter*miedensity*0.125);
    //color.rgb+=(1.0-exp(-viewlength*oosamples*vec3(atmoconstant.x)*miedensity*0.125))*((rayleigh*mieinscatter*miedensity)/(vec3(atmoconstant.x)*miedensity*0.125));
    mie=1.0-exp(-viewlength*oosamples*ext);
    mie*=((1.5*rayleigh*miedensity*miephase*vec3(atmoconstant.x))/ext);
    if (miedensity>0.0000001)
    color.rgb+=mie;
    
    //if (count<15)
    color.rgb+=rayleigh*inscatter*density*exp(-outscatter*density*0.25)*exp(-mieoutscatter);
    
    dist-=oosamples;
    }

  //outscatter=1.0*atmowavelength*atmoconstant.x;
  
  color.a=totaloutscatter;
  }
  
void main()
  {
  vec4 color;
  vec3 viewnormal;
  vec3 lightnormal;
  vec3 position;
  vec2 depthmaptexcoord;
  float fragdepth;

  //depthmaptexcoord=gl_FragCoord.xy*ooresolution;
  depthmaptexcoord.x=(gl_FragCoord.x+0.125)*ooresolution.x;
  depthmaptexcoord.y=(gl_FragCoord.y+0.125)*ooresolution.y;

  viewnormal=normalize(viewvec);
  lightnormal=normalize(lightvec);

  color.rgb=texture(tex_depth,depthmaptexcoord).rgb;
  color.a=0.0;
  fragdepth=color.r*65536.0+color.g*256.0+color.b;

  if (fragdepth>65536.0)
    {
    fragdepth=raydistancesphere2(worldposition,viewnormal,atmoconstant.w);
    if (fragdepth<=0.0)    	
	  fragdepth=raydistancesphere(worldposition,viewnormal,atmoconstant.w+atmoconstant.z);
    
    //if (fragdepth>12.0)
    //  fragdepth=12.0;
    }
  else
    fragdepth*=atmoconstant.y;//*10.0;

  position=viewnormal*vec3(fragdepth);
  atmosphericscattering3(color,position,viewnormal,lightnormal);

  fragcolor=color;
  }