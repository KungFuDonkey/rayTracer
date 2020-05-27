#version 430
layout(local_size_x = 1, local_size_y = 1) in;
layout(rgba32f, binding = 0) uniform image2D img_output;
uniform vec3 circles[];
uniform vec2 boxes[];
uniform vec2 lightsources[];
uniform vec3 colors[];

ivec2 dims;
void hitLine(vec2 ray_o, vec2 ray_d, vec2 point1, vec2 point2, float tmax, inout bool hit);
bool hitObjects(vec2 ray_o, vec2 ray_d, float tmax);
void lightsource(vec2 ray_o, inout vec4 pixel);

void main(){
	vec4 pixel = vec4(0.0, 0.0, 0.0, 1.0);
	ivec2 pixel_coords = ivec2(gl_GlobalInvocationID.xy);

	float max_x = 1;
	float max_y = 1;
	dims = imageSize(img_output);
	float x = (float(pixel_coords.x * 2 - dims.x) / dims.x);
	float y = (float(pixel_coords.y * 2 - dims.y) / dims.x);

	vec2 ray_o = vec2(x* max_x, y * max_y);

	lightsource(ray_o, pixel);
	pixel.x = (-1/ (1 + pixel.x)) + 1;
	pixel.y = (-1/ (1 + pixel.y)) + 1;
	pixel.z = (-1/ (1 + pixel.z)) + 1;
	imageStore(img_output, pixel_coords, pixel);
}