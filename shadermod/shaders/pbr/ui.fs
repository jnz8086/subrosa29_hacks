#version 330 core
uniform sampler2D tex_color;
uniform float time;

in vec2 texcoord;
in vec4 color;

out vec4 fragcolor;

void main()
  {
	vec2 tc = texcoord;
	//tc.x += sin(gl_FragCoord.y*10.0*time)*0.002;
	float osc = sin(gl_FragCoord.y*200.0-time*0.01);
	float osc2 = sin(time*0.05+gl_FragCoord.y*200.0);
	tc.x += sin(gl_FragCoord.y*150.0+time*2.0)*0.0012;
	float op = osc*0.275;
  fragcolor = vec4( texture(tex_color,tc+vec2(-0.003-0.0008*osc2,0.0005)).rg, texture(tex_color,tc).ba);
  fragcolor *= color*vec4(0.7,0.8,1.0,1.0 - op);
  //fragcolor=texture(tex_color,texcoord)*color;
  }