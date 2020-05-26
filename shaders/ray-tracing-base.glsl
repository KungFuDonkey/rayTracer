#version 430
//tells shader how many pixels it has to edit
layout(local_size_x = 1, local_size_y = 1) in;

//image with dimensions
layout(rgba32f, binding = 0) uniform image2D img_output;
ivec2 dims;

//object arrays
uniform vec3 spheres[];
uniform vec3 areaLightsources[];
uniform vec3 directionalLightsources[];
uniform vec4 planes[];
uniform vec3 vertices[];
uniform vec3 colors[];

//function for calculating collisions for normal rays
void calcObjects(vec3 ray_origin, vec3 ray_direction, inout float t, inout float col, inout float absorption, inout vec3 normal);

//function for calculating collisions for light rays
bool calcObjects(vec3 ray_origin, vec3 ray_direction, float tmax);

//function for area lightsources
void calcAreaLightSources(vec3 ray_origin, float absorption, vec3 normal);
//functions for getting the objects out of the arrays
void getColor(int index, out vec3 color);
void getVertice(int index, out vec3 vertice);

vec3 ray_origin;
vec3 color = vec3(0.0, 0.0, 0.0);
vec3 energy;
//main
void main(){

	//initialize the color for this pixel and get invocationID for the ray

	ivec2 pixel_coords = ivec2(gl_GlobalInvocationID.xy);

	//initialization for the ray
	dims = imageSize(img_output);

	for(int y = 0; y < 2; ++y){
		for(int x = 0; x < 2; ++x){
			ray_origin = vec3((float(pixel_coords.x * 2 + 0.5 * x - dims.x) / dims.x), (float(pixel_coords.y * 2 + 0.5 * y - dims.y) / dims.x), 1);
			vec3 ray_direction = ray_origin;
			ray_direction = normalize(ray_direction);
			float t = 100000;
			vec3 next_color = vec3(0, 0, 0);
			vec3 normal = vec3(0, 0, 0);
			float absorption = 0;
			float refraction =0;
			energy = vec3(1, 1, 1);
			float col = 19;
			for(int i = 0; i < 5; ++i){
				calcObjects(ray_origin, ray_direction, t, col, absorption, normal);

				if(t != 100000){
					//update the ray energy, if energy == 0 then stop the loop as there will be no colors left to write 
					getColor(int(col), next_color);
					energy *= next_color;
					if(energy == vec3(0,0,0)) break;
					//create point of intersection 
					ray_origin = ray_origin + t * ray_direction;

					//arealightsources, gets arealightsource and calculates the luminance


					//if absorption == 1 then the ray will be absorped otherwise reflect the ray
					if(refraction != 0){
						ray_direction = refract(ray_direction,normal,refraction);
					}
					else if(absorption != 1){
						calcAreaLightSources(ray_origin, absorption, normal);
						ray_direction = reflect(ray_direction,normal);
					}
					else
						break;
					ray_origin += ray_direction * 0.0001;
					t = 100000;
				}
				else
					break;
			}
		}
	}
	//make sure the colors are within the rgb range
    const float gamma = 2.2;
  
    // exposure tone mapping
    color = vec3(1.0) - exp(-color * 5);
    // gamma correction 
    color = pow(color, vec3(1.0 / gamma));
  
	//store the pixel color in the image
	imageStore(img_output, pixel_coords, vec4(color,1));
}


//void for getting the colors out of the array, as GLSL doesn't have dynamic looping it has to be done like this
void getColor(int index, out vec3 color){
	if(index == 0){
		color = colors[0];
	}
	else if(index == 1){
		color = colors[1];
	}
	else if(index == 2){
		color = colors[2];
	}
	else if(index == 3){
		color = colors[3];
	}
	else if(index == 4){
		color = colors[4];
	}
	else if(index == 5){
		color = colors[5];
	}
	else if(index == 6){
		color = colors[6];
	}
	else if(index == 7){
		color = colors[7];
	}
	else if(index == 8){
		color = colors[8];
	}
	else if(index == 9){
		color = colors[9];
	}
	else if(index == 10){
		color = colors[10];
	}
	else if(index == 11){
		color = colors[11];
	}
	else if(index == 12){
		color = colors[12];
	}
	else if(index == 13){
		color = colors[13];
	}
	else if(index == 14){
		color = colors[14];
	}
	else if(index == 15){
		color = colors[15];
	}
	else if(index == 16){
		color = colors[16];
	}
	else if(index == 17){
		color = colors[17];
	}
	else if(index == 18){
		color = colors[18];
	}
	else if(index == 19){
		color = colors[19];
	}
}
