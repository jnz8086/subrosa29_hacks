#version 330 core
uniform sampler2D tex_color;

in vec2 texcoord;
in vec4 color;

out vec4 fragcolor;

void main()
  {
	/*vec2 tc = texcoord;
	tc.x += sin(gl_FragCoord.y*10.0*time)*0.005;
  fragcolor=texture(tex_color,tc)*color*vec4(0.7,0.8,1.0,1.0);*/
  fragcolor=texture(tex_color,texcoord)*color;
  }