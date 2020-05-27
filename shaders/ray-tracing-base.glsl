#version 430
//Tells shader how many pixels it has to edit
layout(local_size_x = 1, local_size_y = 1) in;

//Image with dimensions
layout(rgba32f, binding = 0) uniform image2D img_output;
ivec2 dims;

//Object arrays
uniform vec3 spheres[];
uniform vec3 areaLightsources[];
uniform vec3 directionalLightsources[];
uniform vec4 planes[];
uniform vec3 vertices[];

//Function for calculating collisions for normal rays
void calcObjects(vec3 ray_origin, vec3 ray_direction, inout float t, inout vec3 col, inout float absorption, inout float refraction, inout vec3 normal);

//Function for calculating collisions for light rays
bool calcObjects(vec3 ray_origin, vec3 ray_direction, float tmax);

//Function for area lightsources
void calcAreaLightSources(vec3 ray_origin, float absorption, vec3 normal);

//Functions for getting the objects out of the arrays
void getColor(int index, out vec3 color);
void getVertice(int index, out vec3 vertice);

vec3 ray_origin;
vec3 color = vec3(0.0, 0.0, 0.0);
vec3 energy;

//Main
void main(){
	//Initialize the color for this pixel and get invocationID for the ray
	ivec2 pixel_coords = ivec2(gl_GlobalInvocationID.xy);

	//Initialization for the ray
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

			for(int i = 0; i < 5; ++i){
				calcObjects(ray_origin, ray_direction, t, next_color, absorption, refraction, normal);

				if(t != 100000){
					//Update the ray energy, if energy == 0 then stop the loop as there will be no colors left to write 
					energy *= next_color;
					if(energy == vec3(0,0,0)) break;

					//Create point of intersection 
					ray_origin = ray_origin + t * ray_direction;

					//Arealightsources, gets arealightsource and calculates the luminance
					if(refraction != 0){
						ray_direction = normalize(ray_direction * refraction - normal * (-dot(normal, ray_direction) + refraction * dot(normal, ray_direction)));
						ray_origin += ray_direction * 0.0002;
						t = 100000;
					}
					else{
						calcAreaLightSources(ray_origin, absorption, normal);

						//If absorption == 1 then the ray will be absorped otherwise reflect the ray
						if(absorption != 1){
							ray_direction = normalize(reflect(ray_direction,normal));
							ray_origin += ray_direction * 0.0001;
							t = 100000;
						}
						else
							break;
					}
				}
				else
					break;
			}
		}
	}

	//Make sure the colors are within the rgb range
    const float gamma = 2.2;
  
    //Exposure tone mapping
    color = vec3(1.0) - exp(-color * 5);

    //Gamma correction 
    color = pow(color, vec3(1.0 / gamma));

	//Store the pixel color in the image
	imageStore(img_output, pixel_coords, vec4(color,1));
}

