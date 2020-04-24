#version 330
in vec4 color;
out vec4 outputColor;
void main(){
	outputColor = vec4(color.x*5,color.y*5,color.z*5,1);
}