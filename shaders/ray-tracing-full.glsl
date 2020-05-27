#version 430
//tells shader how many pixels it has to edit
layout(local_size_x = 1, local_size_y = 1) in;

//image with dimensions
layout(rgba32f, binding = 0) uniform image2D img_output;
layout(binding = 1)uniform sampler2D skydome;
ivec2 dims;

//object arrays
uniform vec3 spheres[];
uniform vec3 areaLightsources[];
uniform vec3 directionalLightsources[];
uniform vec4 planes[];
uniform vec3 vertices[];
uniform vec3 skydomeDirection;

//function for calculating collisions for normal rays
void calcObjects(vec3 ray_origin, vec3 ray_direction, inout float t, inout vec3 col, inout float absorption, inout float refraction, inout vec3 normal);

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
			for(int i = 0; i < 5; ++i){
				calcObjects(ray_origin, ray_direction, t, next_color, absorption, refraction, normal);

				if(t != 100000){
					//update the ray energy, if energy == 0 then stop the loop as there will be no colors left to write 
					energy *= next_color;
					if(energy == vec3(0,0,0)) break;
					//create point of intersection 
					ray_origin = ray_origin + t * ray_direction;

					//arealightsources, gets arealightsource and calculates the luminance

					if(refraction != 0){
						ray_direction = normalize(ray_direction * refraction - normal * (-dot(normal, ray_direction) + refraction * dot(normal, ray_direction)));
						ray_origin += ray_direction * 0.0002;
						t = 100000;
					}
					else{
						calcAreaLightSources(ray_origin, absorption, normal);


						//if absorption == 1 then the ray will be absorped otherwise reflect the ray

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
	//make sure the colors are within the rgb range
    const float gamma = 2.2;
  
    // exposure tone mapping
    color = vec3(1.0) - exp(-color * 5);
    // gamma correction 
    color = pow(color, vec3(1.0 / gamma));
	//store the pixel color in the image
	imageStore(img_output, pixel_coords, vec4(color,1));
}


bool calcObjects(vec3 ray_origin, vec3 ray_direction, float tmax){
    float d;
    float discriminant;
    float s;
    d = 2.0 * dot(ray_origin - spheres[0], ray_direction);
    discriminant = d * d - 4 * (dot(ray_origin - spheres[0], ray_origin - spheres[0]) - 0.25);
    if(discriminant >= 0) {
        s = (-d - sqrt(discriminant)) / 2;
        s = s < 0 ? (-d + sqrt(discriminant)) / 2 : s;
        if(s > 0 && s < tmax) return true;
    }
    vec3 object_position;
    return false;
}

void calcObjects(vec3 ray_origin, vec3 ray_direction, inout float t, inout vec3 col, inout float absorption, inout float refraction, inout vec3 normal){
    float d;
    float discriminant;
    float s;
    d = 2.0 * dot(ray_origin - spheres[0], ray_direction);
    discriminant = d * d - 4 * (dot(ray_origin - spheres[0], ray_origin - spheres[0]) - 0.25);
    if(discriminant >= 0) {
        s = (-d - sqrt(discriminant)) / 2;
        s = s < 0 ? (-d + sqrt(discriminant)) / 2 : s;
        if(s > 0 && s < t) {
            t = s;
            col = vec3(1, 0, 1); 
            normal = normalize(ray_origin + s * ray_direction - spheres[0]);
            refraction = 0;
            absorption = 0.5;
        }
    }
    if(dot(ray_direction, planes[0].xyz) > 0){
        s = -(dot(ray_origin, planes[0].xyz) + planes[0].w) / dot(ray_direction, planes[0].xyz);
        if(s > 0 && s < t){
            t = s;
            col = vec3(0.2, 0.8, 0.2); 
            normal = -planes[0].xyz;
            refraction = 0;
            absorption = 0.7;
        }
    }
    if(dot(ray_direction, planes[1].xyz) > 0){
        s = -(dot(ray_origin, planes[1].xyz) + planes[1].w) / dot(ray_direction, planes[1].xyz);
        if(s > 0 && s < t){
            t = s;
            col = vec3(0, 0, 1); 
            normal = -planes[1].xyz;
            refraction = 0;
            absorption = 0.7;
        }
    }
    if(dot(ray_direction, planes[2].xyz) > 0){
        s = -(dot(ray_origin, planes[2].xyz) + planes[2].w) / dot(ray_direction, planes[2].xyz);
        if(s > 0 && s < t){
            t = s;
            col = vec3(1, 1, 1); 
            normal = -planes[2].xyz;
            refraction = 0;
            absorption = 0.7;
        }
    }
    if(dot(ray_direction, planes[3].xyz) > 0){
        s = -(dot(ray_origin, planes[3].xyz) + planes[3].w) / dot(ray_direction, planes[3].xyz);
        if(s > 0 && s < t){
            t = s;
            col = vec3(0.25, 0.875, 0.8125); 
            normal = -planes[3].xyz;
            refraction = 0;
            absorption = 0.7;
        }
    }
    if(dot(ray_direction, planes[4].xyz) > 0){
        s = -(dot(ray_origin, planes[4].xyz) + planes[4].w) / dot(ray_direction, planes[4].xyz);
        if(s > 0 && s < t){
            t = s;
            col = vec3(1, 0, 1); 
            normal = -planes[4].xyz;
            refraction = 0;
            absorption = 0.7;
        }
    }
    if(dot(ray_direction, planes[5].xyz) > 0){
        s = -(dot(ray_origin, planes[5].xyz) + planes[5].w) / dot(ray_direction, planes[5].xyz);
        if(s > 0 && s < t){
            t = s;
            col = vec3(1, 0, 0); 
            normal = -planes[5].xyz;
            refraction = 0;
            absorption = 0.7;
        }
    }
    vec3 object_position;
    d = 2.0 * dot(ray_origin - areaLightsources[0], ray_direction);
    discriminant = d * d - 4 * (dot(ray_origin - areaLightsources[0], ray_origin - areaLightsources[0]) - 0.04);
    if(discriminant >= 0) {
        s = (-d - sqrt(discriminant)) / 2;
        s = s < 0 ? (-d + sqrt(discriminant)) / 2 : s;
        if(s > 0 && s < t) {
            t = s;
            col = vec3(1, 1, 1);
            normal = vec3(0,0,0);
            absorption = 1;
        }
    }
    d = 2.0 * dot(ray_origin, ray_direction);
    discriminant = d * d - 4 * (dot(ray_origin, ray_origin) - 10000);
    if(discriminant >= 0) {
        s = (-d + sqrt(discriminant)) / 2;
        if(s > 0 && s < t) {
            t = s;
            normal = vec3(0,0,0);
            refraction = 0;
            absorption = 1;
            col = texture(skydome, vec2((ray_direction.x + 1)/2, (ray_direction.y + 1)/2)).xyz * 0.1;
        }
    }
}

void calcAreaLightSources(vec3 ray_origin, float absorption, vec3 normal){
    float lightsource_emittance;
    vec3 light_direction;
    float tmax;
    float angle;
    bool collision;
    vec3 object_color;
    vec3 point_of_intersection;
    light_direction = areaLightsources[0] - ray_origin;
    tmax = length(light_direction) - 0.0002;
    lightsource_emittance = 1 / (12.456 * length(light_direction) * length(light_direction));
    light_direction = normalize(light_direction);
    if(normal == vec3(0,0,0))
        angle = 1;
    else
        angle = dot(normal, light_direction);
    point_of_intersection = ray_origin + light_direction * 0.0001;
    collision = false;
    if(angle < 0) collision = true;
    if(!collision){
        collision = calcObjects(point_of_intersection, light_direction, tmax);
        if(!collision){
            color += vec3(1, 1, 1) * lightsource_emittance * energy * angle * absorption * 0.25;
        }
    }
}

